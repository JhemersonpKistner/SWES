using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Estagios
{
    // Permite enviar vários documentos de até 10 MB no mesmo formulário (limite total: 100 MB).
    [RequestSizeLimit(104857600)]
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    public class CadastroModel : PageModel
    {
        private readonly EstagioService _estagioService;
        private readonly DocumentoEstagioService _documentoService;
        private readonly IWebHostEnvironment _environment;

        private const long TamanhoMaximoArquivo = 10 * 1024 * 1024;

        private readonly string[] extensoesPermitidas =
        {
            ".pdf", ".doc", ".docx", ".txt"
        };

        public CadastroModel(
            EstagioService estagioService,
            DocumentoEstagioService documentoService,
            IWebHostEnvironment environment)
        {
            _estagioService = estagioService;
            _documentoService = documentoService;
            _environment = environment;
        }

        [BindProperty]
        public EstagioCadastroViewModel Estagio { get; set; } = new();

        public OpcoesEstagioViewModel Opcoes { get; set; } = new();

        [TempData]
        public string? Mensagem { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var estagio = await _estagioService.ObterPorIdAsync(id.Value);

                if (estagio == null)
                    return NotFound();

                Estagio = estagio;
            }

            await CarregarOpcoesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ValidarDatas();
            ValidarArquivos();
            ValidarDocumentos();

            if (!ModelState.IsValid)
            {
                await CarregarOpcoesAsync();
                return Page();
            }

            string? caminhoEspecificacoes = null;
            string? nomeEspecificacoes = null;
            string? caminhoEdital = null;
            string? nomeEdital = null;

            var arquivosSalvos = new List<string>();
            var documentosSalvos = new List<DocumentoSalvo>();

            try
            {
                if (Estagio.ArquivoEspecificacoes != null)
                {
                    var arquivo = await SalvarArquivoAsync(
                        Estagio.ArquivoEspecificacoes);

                    caminhoEspecificacoes = arquivo.Caminho;
                    nomeEspecificacoes = arquivo.NomeOriginal;

                    arquivosSalvos.Add(arquivo.CaminhoFisico);
                }

                if (Estagio.ArquivoEdital != null)
                {
                    var arquivo = await SalvarArquivoAsync(
                        Estagio.ArquivoEdital);

                    caminhoEdital = arquivo.Caminho;
                    nomeEdital = arquivo.NomeOriginal;

                    arquivosSalvos.Add(arquivo.CaminhoFisico);
                }

                // Documentos anexados pelo modal
                foreach (var documento in ObterNovosDocumentosPreenchidos())
                {
                    var salvo = await _documentoService.SalvarArquivoAsync(
                        documento.NomeExplicativo!,
                        documento.Arquivo!);

                    documentosSalvos.Add(salvo);
                }

                if (Estagio.Id.HasValue)
                {
                    await _estagioService.AtualizarAsync(
                        Estagio,
                        caminhoEspecificacoes,
                        nomeEspecificacoes,
                        caminhoEdital,
                        nomeEdital,
                        documentosSalvos);

                    Mensagem = "Estágio atualizado com sucesso!";
                }
                else
                {
                    await _estagioService.CadastrarAsync(
                        Estagio,
                        caminhoEspecificacoes,
                        nomeEspecificacoes,
                        caminhoEdital,
                        nomeEdital,
                        documentosSalvos);

                    Mensagem = "Estágio cadastrado com sucesso!";
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                foreach (var caminho in arquivosSalvos)
                {
                    if (System.IO.File.Exists(caminho))
                        System.IO.File.Delete(caminho);
                }

                _documentoService.RemoverArquivosFisicos(documentosSalvos);

                ModelState.AddModelError(string.Empty,
                    $"Não foi possível salvar o estágio: {ex.Message}");

                await CarregarOpcoesAsync();
                return Page();
            }
        }

        // Baixar/visualizar um documento anexado: ?handler=Documento&id=5
        public async Task<IActionResult> OnGetDocumentoAsync(int id)
        {
            var arquivo = await _documentoService.ObterArquivoAsync(id);

            if (arquivo == null)
                return NotFound();

            return PhysicalFile(
                arquivo.CaminhoFisico,
                arquivo.ContentType,
                arquivo.NomeOriginal);
        }

        // Excluir um documento já salvo: ?handler=ExcluirDocumento (POST com id e estagioId)
        public async Task<IActionResult> OnPostExcluirDocumentoAsync(int id, int estagioId)
        {
            var excluido = await _documentoService.ExcluirAsync(id);

            Mensagem = excluido
                ? "Documento removido com sucesso!"
                : "Documento não encontrado.";

            return RedirectToPage(new { id = estagioId });
        }

        public async Task<IActionResult> OnGetProfessoresAsync(Guid instituicaoId)
        {
            if (instituicaoId == Guid.Empty)
                return BadRequest();

            var professores = await _estagioService
                .ObterProfessoresPorInstituicaoAsync(instituicaoId);

            return new JsonResult(professores);
        }

        public async Task<IActionResult> OnGetAlunosAsync(Guid instituicaoId)
        {
            if (instituicaoId == Guid.Empty)
                return BadRequest();

            var alunos = await _estagioService
                .ObterAlunosPorInstituicaoAsync(instituicaoId);

            return new JsonResult(alunos);
        }

        public async Task<IActionResult> OnGetCoordenadoresAsync(Guid instituicaoId)
        {
            if (instituicaoId == Guid.Empty)
                return BadRequest();

            var coordenadores = await _estagioService
                .ObterCoordenadoresPorInstituicaoAsync(instituicaoId);

            return new JsonResult(coordenadores);
        }

        public async Task<IActionResult> OnGetSupervisoresAsync(Guid unidadeConcedenteId)
        {
            if (unidadeConcedenteId == Guid.Empty)
                return BadRequest();

            var supervisores = await _estagioService
                .ObterSupervisoresPorUnidadeAsync(unidadeConcedenteId);

            return new JsonResult(supervisores);
        }

        private async Task CarregarOpcoesAsync()
        {
            Opcoes = await _estagioService.ObterOpcoesAsync();

            // Em caso de erro de validação, a lista de documentos já salvos
            // precisa continuar aparecendo na edição.
            if (Estagio.Id.HasValue && Estagio.DocumentosAtuais.Count == 0)
            {
                var atual = await _estagioService.ObterPorIdAsync(Estagio.Id.Value);

                if (atual != null)
                    Estagio.DocumentosAtuais = atual.DocumentosAtuais;
            }
        }

        private void ValidarDatas()
        {
            if (Estagio.DataInicio.HasValue &&
                Estagio.DataFim.HasValue &&
                Estagio.DataFim.Value.Date < Estagio.DataInicio.Value.Date)
            {
                ModelState.AddModelError(
                    "Estagio.DataFim",
                    "A data de término não pode ser anterior à data de início.");
            }
        }

        private void ValidarArquivos()
        {
            ValidarArquivo(
                Estagio.ArquivoEspecificacoes,
                "Estagio.ArquivoEspecificacoes",
                "especificações");

            ValidarArquivo(
                Estagio.ArquivoEdital,
                "Estagio.ArquivoEdital",
                "edital");
        }

        private void ValidarArquivo(
            IFormFile? arquivo,
            string campo,
            string descricao)
        {
            if (arquivo == null || arquivo.Length == 0)
                return;

            var extensao = Path.GetExtension(arquivo.FileName)
                .ToLowerInvariant();

            if (!extensoesPermitidas.Contains(extensao))
            {
                ModelState.AddModelError(
                    campo,
                    $"O arquivo de {descricao} deve estar em PDF, DOC, DOCX ou TXT.");

                return;
            }

            if (arquivo.Length > TamanhoMaximoArquivo)
            {
                ModelState.AddModelError(
                    campo,
                    $"O arquivo de {descricao} não pode ultrapassar 10 MB.");
            }
        }

        // Linhas do modal que têm nome e arquivo preenchidos
        private IEnumerable<DocumentoNovoViewModel> ObterNovosDocumentosPreenchidos()
        {
            return Estagio.NovosDocumentos.Where(d =>
                !string.IsNullOrWhiteSpace(d.NomeExplicativo) &&
                d.Arquivo != null &&
                d.Arquivo.Length > 0);
        }

        // Valida os documentos do modal. Linhas totalmente vazias são ignoradas.
        private void ValidarDocumentos()
        {
            var preenchidos = 0;

            for (var i = 0; i < Estagio.NovosDocumentos.Count; i++)
            {
                var documento = Estagio.NovosDocumentos[i];
                var chave = $"Estagio.NovosDocumentos[{i}]";

                var temNome = !string.IsNullOrWhiteSpace(documento.NomeExplicativo);
                var temArquivo = documento.Arquivo != null && documento.Arquivo.Length > 0;

                if (!temNome && !temArquivo)
                    continue;

                if (!temNome)
                {
                    ModelState.AddModelError(
                        $"{chave}.NomeExplicativo",
                        "Informe o nome do documento. Ex.: Contrato.");
                }
                else if (documento.NomeExplicativo!.Trim().Length < 3)
                {
                    ModelState.AddModelError(
                        $"{chave}.NomeExplicativo",
                        "O nome do documento deve possuir pelo menos 3 caracteres.");
                }

                if (!temArquivo)
                {
                    ModelState.AddModelError(
                        $"{chave}.Arquivo",
                        "Anexe o arquivo do documento.");
                }
                else
                {
                    var erro = _documentoService.ValidarArquivo(documento.Arquivo!);

                    if (erro != null)
                        ModelState.AddModelError($"{chave}.Arquivo", erro);
                }

                preenchidos++;
            }

            if (preenchidos > DocumentoEstagioService.MaximoPorEnvio)
            {
                ModelState.AddModelError(
                    "Estagio.NovosDocumentos",
                    $"Envie no máximo {DocumentoEstagioService.MaximoPorEnvio} documentos por vez.");
            }
        }

        private async Task<(string Caminho, string NomeOriginal, string CaminhoFisico)>
            SalvarArquivoAsync(IFormFile arquivo)
        {
            var pasta = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "estagios");

            Directory.CreateDirectory(pasta);

            var nomeOriginal = Path.GetFileName(arquivo.FileName);
            var extensao = Path.GetExtension(nomeOriginal).ToLowerInvariant();

            var nomeArmazenado = $"{Guid.NewGuid()}{extensao}";

            var caminhoFisico = Path.Combine(pasta, nomeArmazenado);

            await using (var stream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var caminhoWeb = $"/uploads/estagios/{nomeArmazenado}";

            return (caminhoWeb, nomeOriginal, caminhoFisico);
        }
    }
}