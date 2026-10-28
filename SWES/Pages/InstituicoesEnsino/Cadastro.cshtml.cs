
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SWES.Pages.InstituicoesEnsino
{
    public class CadastroModel : PageModel
    {
        public IActionResult OnGet(Guid? id)
        {
            if (id.HasValue)
            {
                return RedirectToPage(
                    "/UnidadesConcedentes/Cadastro",
                    new { id = id.Value, instituicao = true });
            }

            return RedirectToPage(
                "/UnidadesConcedentes/Cadastro",
                new { instituicao = true });
        }
    }
}
