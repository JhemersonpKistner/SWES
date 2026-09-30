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

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        public IndexModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public void OnGet()
        {
            Usuarios = _usuarioService.ListarUsuarios();

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                Usuarios = Usuarios
                .Where(u =>
                    u.Nome.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                    u.Tipo.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (u.Inscricao.HasValue && u.Inscricao.Value.ToString().Contains(Pesquisa)) ||
                    (Pesquisa.Equals("ativo", StringComparison.OrdinalIgnoreCase) && u.Ativo) ||
                    (Pesquisa.Equals("inativo", StringComparison.OrdinalIgnoreCase) && !u.Ativo))
                .ToList();
            }
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

        public async Task<IActionResult> OnPostAtivarAsync(
            int id,
            string tipo)
        {
            var ativado = await _usuarioService
                .ReativarUsuario(id, tipo);

            if (!ativado)
                return NotFound();

            return RedirectToPage("/Usuarios/Index");
        }

    }
}