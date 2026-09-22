using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public List<UsuarioViewModel> Usuarios { get; set; } = new();

        public IndexModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public void OnGet()
        {
            Usuarios = _usuarioService.ListarUsuarios();
        }

        public async Task<IActionResult> OnPostInativarAsync(
    int id,
    string tipo)
        {
            var inativado = await _usuarioService
                .InativarUsuario(id, tipo);

            if (!inativado)
                return NotFound();

            return RedirectToPage("/Usuarios/Index");
        }

    }
}