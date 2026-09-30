using Microsoft.AspNetCore.Identity;
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
                var usuario = _usuarioService.ObterUsuarioParaEdicao(id.Value, tipo);

                if (usuario == null)
                    return NotFound();

                Usuario = usuario;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove as máscaras antes das validações do backend
            Usuario.Telefone = ApenasNumeros(Usuario.Telefone);
            Usuario.CPF = ApenasNumeros(Usuario.CPF);

            if (!string.IsNullOrWhiteSpace(Usuario.CNPJ))
            {
                Usuario.CNPJ = ApenasNumeros(Usuario.CNPJ);
            }

            // Validação do nome
            ValidarNome();

            // Cadastro de novo usuário
            if (!Usuario.Id.HasValue)
            {
                // Senha
                if (string.IsNullOrWhiteSpace(Usuario.Senha))
                {
                    ModelState.AddModelError(
                        "Usuario.Senha",
                        "A senha é obrigatória.");
                }

                // Confirmação da senha
                if (string.IsNullOrWhiteSpace(Usuario.ConfirmacaoSenha))
                {
                    ModelState.AddModelError(
                        "Usuario.ConfirmacaoSenha",
                        "A confirmação de senha é obrigatória.");
                }

                // Campos específicos de cada perfil
                ValidarCamposEspecificos();
            }

            // Data de nascimento
            ValidarDataNascimento();

            // Se houver algum erro, permanece na página
            if (!ModelState.IsValid)
            {
                Erro = "Verifique os campos destacados.";
                return Page();
            }

            // EDIÇÃO
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

            // CADASTRO
            var resultado = await _usuarioService.CadastrarUsuario(Usuario);

            if (!resultado.Succeeded)
            {
                Erro = string.Join(
                    " ",
                    resultado.Errors.Select(TraduzirErroIdentity));

                return Page();
            }

            return RedirectToPage("/Usuarios/Index");
        }

        // ============================================================
        // VALIDAÇÃO DO NOME
        // ============================================================

        private void ValidarNome()
        {
            if (string.IsNullOrWhiteSpace(Usuario.Nome))
            {
                ModelState.AddModelError(
                    "Usuario.Nome",
                    "O nome é obrigatório.");

                return;
            }

            // Não permite números
            if (Usuario.Nome.Any(char.IsDigit))
            {
                ModelState.AddModelError(
                    "Usuario.Nome",
                    "O nome deve conter apenas letras.");
            }

            // Não permite símbolos
            if (Usuario.Nome.Any(c =>
                !char.IsLetter(c) &&
                !char.IsWhiteSpace(c)))
            {
                ModelState.AddModelError(
                    "Usuario.Nome",
                    "O nome não pode conter números ou símbolos.");
            }

            // Exige pelo menos nome e sobrenome
            var partes = Usuario.Nome
                .Trim()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length < 2)
            {
                ModelState.AddModelError(
                    "Usuario.Nome",
                    "Informe o primeiro e o último nome.");
            }
        }

        // ============================================================
        // REMOVE MÁSCARAS
        // ============================================================

        private string ApenasNumeros(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return new string(
                valor.Where(char.IsDigit).ToArray());
        }

        // ============================================================
        // VALIDAÇÕES ESPECÍFICAS DOS PERFIS
        // ============================================================

        private void ValidarCamposEspecificos()
        {
            if (Usuario.Tipo == "Aluno")
            {
                if (Usuario.Matricula == null)
                {
                    ModelState.AddModelError(
                        "Usuario.Matricula",
                        "A matrícula é obrigatória.");
                }

                if (string.IsNullOrWhiteSpace(Usuario.Curso))
                {
                    ModelState.AddModelError(
                        "Usuario.Curso",
                        "O curso é obrigatório.");
                }

                if (string.IsNullOrWhiteSpace(Usuario.Turno))
                {
                    ModelState.AddModelError(
                        "Usuario.Turno",
                        "O turno é obrigatório.");
                }

                if (string.IsNullOrWhiteSpace(Usuario.Semestre))
                {
                    ModelState.AddModelError(
                        "Usuario.Semestre",
                        "O semestre é obrigatório.");
                }

                if (!Usuario.DataNasc.HasValue)
                {
                    ModelState.AddModelError(
                        "Usuario.DataNasc",
                        "A data de nascimento é obrigatória.");
                }
            }
            else if (Usuario.Tipo == "Professor")
            {
                if (Usuario.Registro == null)
                {
                    ModelState.AddModelError(
                        "Usuario.Registro",
                        "O registro é obrigatório.");
                }

                if (Usuario.CursosProfessor == null ||
                    Usuario.CursosProfessor.Count == 0)
                {
                    ModelState.AddModelError(
                        "Usuario.CursosProfessor",
                        "Selecione pelo menos um curso.");
                }
            }
            else if (Usuario.Tipo == "Supervisor")
            {
                if (string.IsNullOrWhiteSpace(Usuario.Cargo))
                {
                    ModelState.AddModelError(
                        "Usuario.Cargo",
                        "O cargo é obrigatório.");
                }

                if (string.IsNullOrWhiteSpace(Usuario.Empresa))
                {
                    ModelState.AddModelError(
                        "Usuario.Empresa",
                        "A empresa é obrigatória.");
                }

                if (string.IsNullOrWhiteSpace(Usuario.CNPJ))
                {
                    ModelState.AddModelError(
                        "Usuario.CNPJ",
                        "O CNPJ é obrigatório.");
                }
            }
            else if (Usuario.Tipo == "Coordenador")
            {
                if (string.IsNullOrWhiteSpace(Usuario.Setor))
                {
                    ModelState.AddModelError(
                        "Usuario.Setor",
                        "O setor é obrigatório.");
                }
            }
        }

        // ============================================================
        // VALIDAÇÃO DA DATA DE NASCIMENTO
        // ============================================================

        private void ValidarDataNascimento()
        {
            if (!Usuario.DataNasc.HasValue)
                return;

            var data = Usuario.DataNasc.Value;
            var hoje = DateTime.Today;

            if (data.Year < 1900)
            {
                ModelState.AddModelError(
                    "Usuario.DataNasc",
                    "A data de nascimento não pode ser anterior ao ano 1900.");
            }

            if (data.Date > hoje)
            {
                ModelState.AddModelError(
                    "Usuario.DataNasc",
                    "A data de nascimento não pode ser futura.");
            }
        }

        // ============================================================
        // TRADUÇÃO DOS ERROS DO ASP.NET IDENTITY
        // ============================================================

        private string TraduzirErroIdentity(IdentityError erro)
        {
            return erro.Code switch
            {
                "PasswordTooShort" =>
                    "A senha deve possuir pelo menos 6 caracteres.",

                "PasswordRequiresDigit" =>
                    "A senha deve possuir pelo menos um número.",

                "PasswordRequiresUpper" =>
                    "A senha deve possuir pelo menos uma letra maiúscula.",

                "PasswordRequiresLower" =>
                    "A senha deve possuir pelo menos uma letra minúscula.",

                "PasswordRequiresNonAlphanumeric" =>
                    "A senha deve possuir pelo menos um caractere especial, como !, @ ou #.",

                "DuplicateUserName" =>
                    "Já existe um usuário cadastrado com este e-mail.",

                "DuplicateEmail" =>
                    "Já existe um usuário cadastrado com este e-mail.",

                _ =>
                    "Não foi possível cadastrar o usuário. Verifique os dados informados."
            };
        }
    }
}