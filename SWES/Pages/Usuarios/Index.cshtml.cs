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
        private readonly IWebHostEnvironment _environment;

        public List<UsuarioViewModel> Usuarios { get; set; } = new();

        public int PaginaAtual { get; set; }

        public int TotalPaginas { get; set; }

        public int TamanhoPagina { get; set; } = 100;

        public bool EmDesenvolvimento => _environment.IsDevelopment();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Situacao { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Perfil { get; set; }

        public IndexModel(
            UsuarioService usuarioService,
            ExportacaoService exportacaoService,
            IWebHostEnvironment environment)
        {
            _usuarioService = usuarioService;
            _exportacaoService = exportacaoService;
            _environment = environment;
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
                        u.Tipo.Contains(Pesquisa, StringComparison.OrdinalIgnoreCase) ||
                        (u.Inscricao.HasValue && u.Inscricao.Value.ToString().Contains(Pesquisa)) ||
                        (Pesquisa.Equals("ativo", StringComparison.OrdinalIgnoreCase) &&
                         u.Ativo) ||
                        (Pesquisa.Equals("inativo", StringComparison.OrdinalIgnoreCase) &&
                         !u.Ativo))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(Situacao) &&
    Situacao != "Todos")
            {
                if (Situacao == "Ativo")
                {
                    Usuarios = Usuarios
                        .Where(u => u.Ativo)
                        .ToList();
                }
                else if (Situacao == "Inativo")
                {
                    Usuarios = Usuarios
                        .Where(u => !u.Ativo)
                        .ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(Perfil) &&
                Perfil != "Todos")
            {
                Usuarios = Usuarios
                    .Where(u =>
                        u.Tipo.Equals(
                            Perfil,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Usuarios = Usuarios
                .OrderBy(u => u.Nome)
                .ThenBy(u => u.Tipo)
                .ThenBy(u => u.Id)
                .ToList();

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
                        (u.Inscricao.HasValue &&
                        u.Inscricao.Value.ToString().Contains(Pesquisa)) ||
                        (Pesquisa.Equals("ativo", StringComparison.OrdinalIgnoreCase) &&
                        u.Ativo) ||
                        (Pesquisa.Equals("inativo", StringComparison.OrdinalIgnoreCase) &&
                        !u.Ativo))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(Situacao) &&
                Situacao != "Todos")
            {
                if (Situacao == "Ativo")
                {
                    usuarios = usuarios
                        .Where(u => u.Ativo)
                        .ToList();
                }
                else if (Situacao == "Inativo")
                {
                    usuarios = usuarios
                        .Where(u => !u.Ativo)
                        .ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(Perfil) &&
                Perfil != "Todos")
            {
                usuarios = usuarios
                    .Where(u =>
                        u.Tipo.Equals(
                            Perfil,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var usuariosParaExportacao =
                _usuarioService.ListarUsuariosParaExportacao();

            var idsFiltrados = usuarios
                .Select(u => new { u.Id, u.Tipo })
                .ToHashSet();

            usuariosParaExportacao = usuariosParaExportacao
                .Where(u => idsFiltrados.Contains(new { u.Id, u.Tipo }))
                .ToList();

            var arquivo = _exportacaoService
                .ExportarUsuariosParaExcel(usuariosParaExportacao);

            var nomeArquivo =
                $"usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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