using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly UsuarioService _usuarioService;
        private readonly ExportacaoService _exportacaoService;

        public List<UsuarioViewModel> Usuarios { get; set; } = new();

        public int PaginaAtual { get; set; }

        public int TotalPaginas { get; set; }

        public int TamanhoPagina { get; set; } = 100;

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        public IndexModel(UsuarioService usuarioService, ExportacaoService exportacaoService)
        {
            _usuarioService = usuarioService;
            _exportacaoService = exportacaoService;
        }

        public void OnGet(int pagina = 1)
        {
            Usuarios = _usuarioService.ListarUsuarios();

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                Usuarios = Usuarios
                    .Where(u =>
                        u.Nome.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                        u.Tipo.Contains(Pesquisa,  StringComparison.OrdinalIgnoreCase) ||
                        (u.Inscricao.HasValue && u.Inscricao.Value.ToString().Contains(Pesquisa)) ||
                        (Pesquisa.Equals("ativo", StringComparison.OrdinalIgnoreCase) &&
                         u.Ativo) ||
                        (Pesquisa.Equals("inativo", StringComparison.OrdinalIgnoreCase) &&
                         !u.Ativo))
                    .ToList();
            }

            var totalUsuarios = Usuarios.Count;

            TotalPaginas = (int)Math.Ceiling((double)totalUsuarios / TamanhoPagina);

            if (TotalPaginas == 0)
            {
                PaginaAtual = 1;
                return;
            }

            if (pagina < 1)
                pagina = 1;

            if (pagina > TotalPaginas)
                pagina = TotalPaginas;

            PaginaAtual = pagina;

            Usuarios = Usuarios
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();
        }

        public IActionResult OnGetExportarExcel()
        {
            var usuarios = _usuarioService.ListarUsuarios();

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                usuarios = usuarios
                    .Where(u =>
                        u.Nome.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                        u.Email.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                        u.Tipo.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                        (u.Inscricao.HasValue && u.Inscricao.Value.ToString().Contains(Pesquisa)) ||
                        (Pesquisa.Equals("ativo", StringComparison.OrdinalIgnoreCase) &&
                         u.Ativo) ||
                        (Pesquisa.Equals("inativo", StringComparison.OrdinalIgnoreCase) &&
                         !u.Ativo))
                    .ToList();
            }

            var arquivo = _exportacaoService.ExportarUsuariosParaExcel(usuarios);

            var nomeArquivo = $"usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                arquivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nomeArquivo);
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