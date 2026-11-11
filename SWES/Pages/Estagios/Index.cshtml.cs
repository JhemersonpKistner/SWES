using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Estagios
{
    public class IndexModel : PageModel
    {
        private readonly EstagioService _estagioService;

        public IndexModel(EstagioService estagioService)
        {
            _estagioService = estagioService;
        }

        public List<EstagioViewModel> Estagios { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        [TempData]
        public string? Mensagem { get; set; }

        [TempData]
        public string? Erro { get; set; }

        public async Task OnGetAsync()
        {
            Estagios = await _estagioService.ListarEstagiosAsync(Pesquisa);
        }

        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            try
            {
                var excluido = await _estagioService.ExcluirAsync(id);

                Mensagem = excluido
                    ? "Estágio excluído com sucesso!"
                    : "Estágio não encontrado.";
            }
            catch (InvalidOperationException ex)
            {
                Erro = ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostInativarAsync(int id)
        {
            try
            {
                var inativado = await _estagioService.InativarAsync(id);

                Mensagem = inativado
                    ? "Estágio inativado com sucesso!"
                    : "Estágio não encontrado.";
            }
            catch (InvalidOperationException ex)
            {
                Erro = ex.Message;
            }

            return RedirectToPage();
        }
    }
}