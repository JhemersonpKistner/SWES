using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Usuarios
{
    public class CadastroModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public UsuarioCadastroViewModel Usuario { get; set; } = new();

        public string? Erro { get; set; }

        public CadastroModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult OnGet(int? id, string? tipo)
        {
            if (id.HasValue && !string.IsNullOrEmpty(tipo))
            {
                var usuario = _usuarioService
                    .ObterUsuarioParaEdicao(id.Value, tipo);

                if (usuario == null)
                {
                    return NotFound();
                }

                Usuario = usuario;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Usuario.Id.HasValue && string.IsNullOrWhiteSpace(Usuario.Senha))
            {
                Erro = "A senha é obrigatória para cadastrar um novo usuário.";

                return Page();
            }

            if (!Usuario.Id.HasValue)
            {
                ValidarCamposEspecificos();
            }

            if (!ModelState.IsValid)
            {
                Erro = "O formulário possui dados inválidos.";

                return Page();
            }

            if (Usuario.Id.HasValue)
            {
                var atualizado = await _usuarioService.AtualizarUsuario(Usuario);

                if (!atualizado)
                {
                    Erro = "Não foi possível atualizar o usuário.";

                    return Page();
                }

                return RedirectToPage("/Usuarios/Index");
            }

            var resultado = await _usuarioService.CadastrarUsuario(Usuario);

            if (!resultado.Succeeded)
            {
                Erro = string.Join(
                    " ",
                    resultado.Errors.Select(e => e.Description)
                );

                return Page();
            }

            return RedirectToPage("/Usuarios/Index");
        }

        private void ValidarCamposEspecificos()
        {
            if (Usuario.Tipo == "Aluno")
            {
                if (Usuario.Matricula == null)
                    ModelState.AddModelError(
                        "Usuario.Matricula",
                        "A matrícula é obrigatória para alunos.");

                if (string.IsNullOrWhiteSpace(Usuario.Curso))
                    ModelState.AddModelError(
                        "Usuario.Curso",
                        "O curso é obrigatório para alunos.");

                if (string.IsNullOrWhiteSpace(Usuario.Turno))
                    ModelState.AddModelError(
                        "Usuario.Turno",
                        "O turno é obrigatório para alunos.");

                if (string.IsNullOrWhiteSpace(Usuario.Semestre))
                    ModelState.AddModelError(
                        "Usuario.Semestre",
                        "O semestre é obrigatório para alunos.");

                if (!Usuario.DataNasc.HasValue)
                    ModelState.AddModelError(
                        "Usuario.DataNasc",
                        "A data de nascimento é obrigatória para alunos.");
            }
            else if (Usuario.Tipo == "Professor")
            {
                if (Usuario.Registro == null)
                    ModelState.AddModelError(
                        "Usuario.Registro",
                        "O registro é obrigatório para professores.");
            }
            else if (Usuario.Tipo == "Supervisor")
            {
                if (string.IsNullOrWhiteSpace(Usuario.Cargo))
                    ModelState.AddModelError(
                        "Usuario.Cargo",
                        "O cargo é obrigatório para supervisores.");

                if (string.IsNullOrWhiteSpace(Usuario.Empresa))
                    ModelState.AddModelError(
                        "Usuario.Empresa",
                        "A empresa é obrigatória para supervisores.");
            }
            else if (Usuario.Tipo == "Coordenador")
            {
                if (string.IsNullOrWhiteSpace(Usuario.Setor))
                    ModelState.AddModelError(
                        "Usuario.Setor",
                        "O setor é obrigatório para coordenadores.");
            }
        }
    }
}