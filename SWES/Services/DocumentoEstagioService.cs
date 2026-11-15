using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using SWES.Data;

namespace SWES.Services
{
    // Resultado de um arquivo já gravado em disco
    public record DocumentoSalvo(
        string NomeExplicativo,
        string NomeOriginal,
        string CaminhoRelativo,
        long Tamanho,
        string CaminhoFisico);

    public record ArquivoDocumento(
        string CaminhoFisico,
        string NomeOriginal,
        string ContentType);

    public class DocumentoEstagioService
    {
        public const long TamanhoMaximo = 10L * 1024 * 1024;
        public const int MaximoPorEnvio = 10;

        private static readonly string[] ExtensoesPermitidas =
        {
            ".pdf", ".doc", ".docx", ".txt", ".png", ".jpg", ".jpeg"
        };

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentoEstagioService(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Fora do wwwroot: os arquivos não ficam acessíveis por link público.
        private string PastaArmazenamento =>
            Path.Combine(_environment.ContentRootPath, "App_Data", "documentos-estagio");

        // Retorna a mensagem de erro, ou null se o arquivo estiver válido.
        public string? ValidarArquivo(IFormFile arquivo)
        {
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

            if (!ExtensoesPermitidas.Contains(extensao))
                return "O documento deve estar em PDF, DOC, DOCX, TXT, PNG, JPG ou JPEG.";

            if (arquivo.Length > TamanhoMaximo)
                return "O documento não pode ultrapassar 10 MB.";

            return null;
        }

        public async Task<DocumentoSalvo> SalvarArquivoAsync(
            string nomeExplicativo,
            IFormFile arquivo)
        {
            Directory.CreateDirectory(PastaArmazenamento);

            var nomeOriginal = Path.GetFileName(arquivo.FileName);
            var extensao = Path.GetExtension(nomeOriginal).ToLowerInvariant();
            var nomeArmazenado = $"{Guid.NewGuid()}{extensao}";
            var caminhoFisico = Path.Combine(PastaArmazenamento, nomeArmazenado);

            await using (var stream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return new DocumentoSalvo(
                nomeExplicativo.Trim(),
                nomeOriginal,
                nomeArmazenado,
                arquivo.Length,
                caminhoFisico);
        }

        // Usado para desfazer o upload quando o salvamento do estágio falha.
        public void RemoverArquivosFisicos(IEnumerable<DocumentoSalvo> arquivos)
        {
            foreach (var arquivo in arquivos)
            {
                if (File.Exists(arquivo.CaminhoFisico))
                    File.Delete(arquivo.CaminhoFisico);
            }
        }

        public async Task<ArquivoDocumento?> ObterArquivoAsync(int documentoId)
        {
            var documento = await _context.Documentos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == documentoId);

            if (documento == null || string.IsNullOrWhiteSpace(documento.CaminhoArquivo))
                return null;

            // Path.GetFileName impede qualquer tentativa de sair da pasta.
            var caminhoFisico = Path.Combine(
                PastaArmazenamento,
                Path.GetFileName(documento.CaminhoArquivo));

            if (!File.Exists(caminhoFisico))
                return null;

            var provedor = new FileExtensionContentTypeProvider();

            if (!provedor.TryGetContentType(documento.NomeArquivoOriginal, out var contentType))
                contentType = "application/octet-stream";

            return new ArquivoDocumento(
                caminhoFisico,
                documento.NomeArquivoOriginal,
                contentType);
        }

        public async Task<bool> ExcluirAsync(int documentoId)
        {
            var documento = await _context.Documentos
                .FirstOrDefaultAsync(d => d.Id == documentoId);

            if (documento == null)
                return false;

            var caminhoFisico = string.IsNullOrWhiteSpace(documento.CaminhoArquivo)
                ? null
                : Path.Combine(
                    PastaArmazenamento,
                    Path.GetFileName(documento.CaminhoArquivo));

            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();

            if (caminhoFisico != null && File.Exists(caminhoFisico))
                File.Delete(caminhoFisico);

            return true;
        }
    }
}