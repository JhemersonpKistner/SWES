using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class UsuarioCadastroViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(60, MinimumLength = 5, ErrorMessage = "O nome deve possuir entre 5 e 60 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ]+(\s+[A-Za-zÀ-ÖØ-öø-ÿ]+)+$", ErrorMessage = "Informe nome e sobrenome utilizando apenas letras.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [StringLength(100, ErrorMessage = "O e-mail deve possuir no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "A senha deve possuir no máximo 100 caracteres.")]
        public string? Senha { get; set; }

        
        [Compare(nameof(Senha), ErrorMessage = "As senhas não coincidem.")]
        public string? ConfirmacaoSenha { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [RegularExpression(@"^(\d{10,11}|\(\d{2}\) \d{4,5}-\d{4})$", ErrorMessage = "O telefone deve possuir 10 ou 11 números.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [CustomValidation(typeof(UsuarioCadastroViewModel), nameof(ValidarCPF))]
        public string CPF { get; set; } = string.Empty;


        

        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "A matrícula deve conter somente números e ter no máximo 10 dígitos.")]
        [Range(1, int.MaxValue, ErrorMessage = "A matrícula deve estar entre 1 e 2.147.483.647.")]
        public int? Matricula { get; set; }

        [StringLength(100, ErrorMessage = "O curso deve possuir no máximo 100 caracteres.")]
        public string? Curso { get; set; }

        [StringLength(30, ErrorMessage = "O turno deve possuir no máximo 30 caracteres.")]
        public string? Turno { get; set; }

        [StringLength(30, ErrorMessage = "O semestre deve possuir no máximo 30 caracteres.")]
        public string? Semestre { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNasc { get; set; }


        

        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "O registro deve conter somente números e ter no máximo 10 dígitos.")]
        public int? Registro { get; set; }

        public List<string> CursosProfessor { get; set; } = new();


        

        [StringLength(60, ErrorMessage = "O cargo deve possuir no máximo 60 caracteres.")]
        public string? Cargo { get; set; }


        [StringLength(100, ErrorMessage = "A empresa deve possuir no máximo 100 caracteres.")]
        public string? Empresa { get; set; }

        [RegularExpression(@"^(\d{14}|\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})$", ErrorMessage = "O CNPJ deve possuir 14 números.")]
        public string? CNPJ { get; set; }


        

        [StringLength(100, ErrorMessage = "O setor deve possuir no máximo 100 caracteres.")]
        public string? Setor { get; set; }

        public static ValidationResult? ValidarCPF(
            string? cpf,
            ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return new ValidationResult(
                    "O CPF é obrigatório.");
            }

            cpf = new string(
                cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
            {
                return new ValidationResult(
                    "O CPF deve possuir 11 números.");
            }

            if (cpf.Distinct().Count() == 1)
            {
                return new ValidationResult(
                    "Informe um CPF válido.");
            }

            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += (cpf[i] - '0') * (10 - i);
            }

            int resto = soma % 11;

            int primeiroDigito =
                resto < 2 ? 0 : 11 - resto;

            if (primeiroDigito != cpf[9] - '0')
            {
                return new ValidationResult(
                    "Informe um CPF válido.");
            }

            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += (cpf[i] - '0') * (11 - i);
            }

            resto = soma % 11;

            int segundoDigito =
                resto < 2 ? 0 : 11 - resto;

            if (segundoDigito != cpf[10] - '0')
            {
                return new ValidationResult(
                    "Informe um CPF válido.");
            }

            return ValidationResult.Success;
        }
    }
}