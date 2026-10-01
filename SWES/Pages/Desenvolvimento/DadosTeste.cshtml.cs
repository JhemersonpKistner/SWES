using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;

namespace SWES.Pages.Desenvolvimento
{
    public class DadosTesteModel : PageModel
    {
        private readonly DadosTesteService _dadosTesteService;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public int Quantidade { get; set; } = 250;

        public int QuantidadeAtual => _dadosTesteService.QuantidadeAtual;
        public string? Mensagem { get; set; }
        public string? Erro { get; set; }

        public DadosTesteModel(
            DadosTesteService dadosTesteService,
            IWebHostEnvironment environment)
        {
            _dadosTesteService = dadosTesteService;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            if (Quantidade < 1 || Quantidade > 5000)
            {
                ModelState.AddModelError(
                    nameof(Quantidade),
                    "Informe uma quantidade entre 1 e 5.000.");
                return Page();
            }

            try
            {
                var criados = await _dadosTesteService.GerarAsync(Quantidade);
                Mensagem = criados == 0
                    ? $"A base já possui {QuantidadeAtual} registros fictícios. Nenhum registro duplicado foi criado."
                    : $"Foram criados {criados} novos usuários fictícios. A base agora possui {QuantidadeAtual} registros fictícios.";
            }
            catch (Exception ex)
            {
                Erro = $"Não foi possível gerar os dados de teste: {ex.Message}";
            }

            return Page();
        }
    }
}
