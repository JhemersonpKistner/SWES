using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

Console.WriteLine($"ID RECEBIDO: {Usuario.Id}");
Console.WriteLine($"TIPO RECEBIDO: {Usuario.Tipo}");            
            // A validação acontece com os valores mascarados. Isso evita que a tela
            // perca as máscaras quando o servidor precisar devolver o formulário.
            CorrigirErroDeConversaoDaMatricula();

            // Validação do nome
            ValidarNome();
Console.WriteLine($"ID: {Usuario.Id}");
Console.WriteLine($"É cadastro novo? {!Usuario.Id.HasValue}");
            // Cadastro de novo usuário
            if (!Usuario.Id.HasValue)
            {
                Console.WriteLine("ENTROU NO BLOCO DE CADASTRO");
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
            CorrigirErroDeConversaoDataNascimento();
            ValidarDataNascimento();

            // Se houver algum erro, permanece na página
            if (!ModelState.IsValid)
            {
                Erro = "Verifique os campos destacados.";
                RestaurarValoresMascaradosNoModelState();
                return Page();
            }

            // Remove as máscaras somente depois que todas as validações foram concluídas.
            Usuario.Telefone = ApenasNumeros(Usuario.Telefone);
            Usuario.CPF = ApenasNumeros(Usuario.CPF);
            Usuario.CNPJ = ApenasNumeros(Usuario.CNPJ);

            // EDIÇÃO
            if (Usuario.Id.HasValue)
            {
                var atualizado = await _usuarioService.AtualizarUsuario(Usuario);

                if (!atualizado)
                {
                    Erro = "Não foi possível atualizar o usuário.";
                    RestaurarValoresMascaradosNoModelState();
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

                RestaurarValoresMascaradosNoModelState();
                return Page();
            }

            return RedirectToPage("/Usuarios/Index");
        }

        private void CorrigirErroDeConversaoDaMatricula()
        {
            const string chave = "Usuario.Matricula";

            if (!ModelState.TryGetValue(chave, out var entrada))
                return;

            var valor = entrada.AttemptedValue;
            if (string.IsNullOrWhiteSpace(valor))
                return;

            if (valor.All(char.IsDigit) && valor.Length <= 10 &&
                !int.TryParse(valor, out _))
            {
                ModelState.Remove(chave);
                ModelState.AddModelError(
                    chave,
                    "A matrícula deve estar entre 1 e 2.147.483.647.");
            }
        }

        private void CorrigirErroDeConversaoDataNascimento()
        {
            const string chave = "Usuario.DataNasc";

            if (!ModelState.TryGetValue(chave, out var entrada))
                return;

            var valor = entrada.AttemptedValue;
            if (string.IsNullOrWhiteSpace(valor))
                return;

            var partes = valor.Split('-');
            if (partes.Length >= 1 && partes[0].All(char.IsDigit) && partes[0].Length > 4)
            {
                ModelState.Remove(chave);
                ModelState.AddModelError(
                    chave,
                    "O ano da data de nascimento deve possuir no máximo 4 dígitos.");
            }
        }

        private void RestaurarValoresMascaradosNoModelState()
        {
            DefinirValorNoModelState("Usuario.Telefone", FormatarTelefone(Usuario.Telefone));
            DefinirValorNoModelState("Usuario.CPF", FormatarCpf(Usuario.CPF));
            DefinirValorNoModelState("Usuario.CNPJ", FormatarCnpj(Usuario.CNPJ));
        }

        private void DefinirValorNoModelState(string chave, string? valor)
        {
            if (ModelState.ContainsKey(chave))
                ModelState.SetModelValue(chave, new ValueProviderResult(valor ?? string.Empty));
        }

        private string FormatarTelefone(string? valor)
        {
            var numeros = ApenasNumeros(valor);

            if (numeros.Length <= 2) return numeros.Length == 0 ? string.Empty : $"({numeros}";
            if (numeros.Length <= 6) return $"({numeros[..2]}) {numeros[2..]}";
            if (numeros.Length <= 10) return $"({numeros[..2]}) {numeros[2..6]}-{numeros[6..]}";
            return $"({numeros[..2]}) {numeros[2..7]}-{numeros[7..]}";
        }

        private string FormatarCpf(string? valor)
        {
            var numeros = ApenasNumeros(valor);
            if (numeros.Length <= 3) return numeros;
            if (numeros.Length <= 6) return $"{numeros[..3]}.{numeros[3..]}";
            if (numeros.Length <= 9) return $"{numeros[..3]}.{numeros[3..6]}.{numeros[6..]}";
            return $"{numeros[..3]}.{numeros[3..6]}.{numeros[6..9]}-{numeros[9..]}";
        }

        private string FormatarCnpj(string? valor)
        {
            var numeros = ApenasNumeros(valor);
            if (numeros.Length <= 2) return numeros;
            if (numeros.Length <= 5) return $"{numeros[..2]}.{numeros[2..]}";
            if (numeros.Length <= 8) return $"{numeros[..2]}.{numeros[2..5]}.{numeros[5..]}";
            if (numeros.Length <= 12) return $"{numeros[..2]}.{numeros[2..5]}.{numeros[5..8]}/{numeros[8..]}";
            return $"{numeros[..2]}.{numeros[2..5]}.{numeros[5..8]}/{numeros[8..12]}-{numeros[12..]}";
        }

        // ============================================================
        // VALIDAÇÃO DO NOME
        // ============================================================

        private void ValidarNome()
        {
            foreach (var erro in ModelState)
{
    foreach (var mensagem in erro.Value.Errors)
    {
        Console.WriteLine($"ERRO FINAL: {erro.Key} - {mensagem.ErrorMessage}");
    }
}
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
                if (Usuario.Matricula == null &&
                    !(ModelState["Usuario.Matricula"]?.Errors.Any() ?? false))
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

                if (!Usuario.DataNasc.HasValue &&
                    !(ModelState["Usuario.DataNasc"]?.Errors.Any() ?? false))
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

            if (data.Year < 1900 || data.Year > 9999)
            {
                ModelState.AddModelError(
                    "Usuario.DataNasc",
                    "Informe uma data de nascimento com ano de 4 dígitos, entre 1900 e 9999.");
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