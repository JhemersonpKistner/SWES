using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SWES.Pages
{
    [AllowAnonymous]
    public class AcessoNegadoModel : PageModel
    {
        public void OnGet() { }
    }
}
