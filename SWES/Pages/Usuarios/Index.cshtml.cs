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
            // Define "Ativo" como filtro padrão ao abrir a página.
            // Se o usuário já tiver escolhido outro filtro, ele será mantido.
            if (string.IsNullOrWhiteSpace(Situacao))
            {
                Situacao = "Ativo";
            }

            Usuarios = _usuarioService.ListarUsuarios();

            // Filtro de pesquisa
            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                Usuarios = Usuarios
                    .Where(u =>
                        u.Nome.Contains(
                            Pesquisa,
                            StringComparison.OrdinalIgnoreCase) ||

                        u.Email.Contains(
                            Pesquisa,
                            StringComparison.OrdinalIgnoreCase) ||

                        u.Tipo.Contains(
                            Pesquisa,
                            StringComparison.OrdinalIgnoreCase) ||

                        (u.Inscricao.HasValue &&
                         u.Inscricao.Value
                             .ToString()
                             .Contains(Pesquisa)) ||

                        (Pesquisa.Equals(
                            "ativo",
                            StringComparison.OrdinalIgnoreCase) &&
                         u.Ativo) ||

                        (Pesquisa.Equals(
                            "inativo",
                            StringComparison.OrdinalIgnoreCase) &&
                         !u.Ativo))
                    .ToList();
            }

            // Filtro de situação
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

            // Filtro de perfil
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

            // Ordenação
            Usuarios = Usuarios
                .OrderBy(u => u.Nome)
                .ThenBy(u => u.Tipo)
                .ThenBy(u => u.Id)
                .ToList();

            // Calcula o total de páginas
            var totalUsuarios = Usuarios.Count;

            TotalPaginas = (int)Math.Ceiling(
                (double)totalUsuarios / TamanhoPagina);

            // Caso não existam usuários
            if (TotalPaginas == 0)
            {
                PaginaAtual = 1;
                return;
            }

            // Garante que a página seja válida
            if (pagina < 1)
            {
                pagina = 1;
            }

            if (pagina > TotalPaginas)
            {
                pagina = TotalPaginas;
            }

            PaginaAtual = pagina;

            // Paginação
            Usuarios = Usuarios
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();
        }

        public IActionResult OnGetExportarExcel(int pagina = 1)
        {
            // Define "Ativo" como filtro padrão para a exportação
            // caso nenhuma situação tenha sido informada.
            if (string.IsNullOrWhiteSpace(Situacao))
            {
                Situacao = "Ativo";
            }

            var usuarios = _usuarioService.ListarUsuarios();

            // Filtro de pesquisa
            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                usuarios = usuarios
                    .Where(u =>
                        u.Nome.Contains(
                            Pesquisa,
                            StringComparison.OrdinalIgnoreCase) ||

                        u.Email.Contains(
                            Pesquisa,
                            StringComparison.OrdinalIgnoreCase) ||

                        u.Tipo.Contains(
                            Pesquisa,
                            StringComparison.OrdinalIgnoreCase) ||

                        (u.Inscricao.HasValue &&
                         u.Inscricao.Value
                             .ToString()
                             .Contains(Pesquisa)) ||

                        (Pesquisa.Equals(
                            "ativo",
                            StringComparison.OrdinalIgnoreCase) &&
                         u.Ativo) ||

                        (Pesquisa.Equals(
                            "inativo",
                            StringComparison.OrdinalIgnoreCase) &&
                         !u.Ativo))
                    .ToList();
            }

            // Filtro de situação
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

            // Filtro de perfil
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

            // Mesma ordenação utilizada na tela
            usuarios = usuarios
                .OrderBy(u => u.Nome)
                .ThenBy(u => u.Tipo)
                .ThenBy(u => u.Id)
                .ToList();

            // Garante que a página seja válida
            if (pagina < 1)
            {
                pagina = 1;
            }

            // Exporta somente os registros visíveis na página atual.
            usuarios = usuarios
                .Skip((pagina - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToList();

            // Gera o arquivo Excel
            var arquivo = _exportacaoService
                .ExportarUsuariosParaExcel(usuarios);

            var nomeArquivo =
                $"usuarios_pagina_{pagina}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
            {
                return NotFound();
            }

            return RedirectToPage("/Usuarios/Index");
        }

        public async Task<IActionResult> OnPostAtivarAsync(
            int id,
            string tipo)
        {
            var ativado = await _usuarioService
                .ReativarUsuario(id, tipo);

            if (!ativado)
            {
                return NotFound();
            }

            return RedirectToPage("/Usuarios/Index");
        }
    }
}