using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Estagios
{
    public class CadastroModel : PageModel
    {
        private readonly EstagioService _estagioService;
        private readonly IWebHostEnvironment _environment;

        private const long TamanhoMaximoArquivo = 10 * 1024 * 1024;

        private readonly string[] extensoesPermitidas =
        {
            ".pdf", ".doc", ".docx", ".txt"
        };

        public CadastroModel(
            EstagioService estagioService,
            IWebHostEnvironment environment)
        {
            _estagioService = estagioService;
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

                if (Estagio.Id.HasValue)
                {
                    await _estagioService.AtualizarAsync(
                        Estagio,
                        caminhoEspecificacoes,
                        nomeEspecificacoes,
                        caminhoEdital,
                        nomeEdital);

                    Mensagem = "Estágio atualizado com sucesso!";
                }
                else
                {
                    await _estagioService.CadastrarAsync(
                        Estagio,
                        caminhoEspecificacoes,
                        nomeEspecificacoes,
                        caminhoEdital,
                        nomeEdital);

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

                ModelState.AddModelError(string.Empty,
                    $"Não foi possível salvar o estágio: {ex.Message}");

                await CarregarOpcoesAsync();
                return Page();
            }
        }

        private async Task CarregarOpcoesAsync()
        {
            Opcoes = await _estagioService.ObterOpcoesAsync();
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