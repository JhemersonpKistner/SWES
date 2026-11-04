using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class UsuarioCadastroViewModel
    {
        public Guid? Id { get; set; }

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

        [Required(ErrorMessage = "O RG é obrigatório.")]
        [RegularExpression(@"^[0-9A-Za-z.\-]{5,20}$", ErrorMessage = "Informe um RG válido (5 a 20 caracteres, só letras, números, ponto ou hífen).")]
        public string RG { get; set; } = string.Empty;

        [Required(ErrorMessage = "O órgão expedidor é obrigatório.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ/\-\s]{2,20}$", ErrorMessage = "Informe um órgão expedidor válido. Ex.: SSP/GO.")]
        public string OrgaoExpedidor { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado civil é obrigatório.")]
        [RegularExpression(@"^(Solteiro|Casado|União estável|Separado|Divorciado|Viúvo)$", ErrorMessage = "Selecione um estado civil válido.")]
        public string EstadoCivil { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nacionalidade é obrigatória.")]
        [StringLength(50, ErrorMessage = "A nacionalidade deve possuir no máximo 50 caracteres.")]
        public string Nacionalidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(150, ErrorMessage = "O endereço deve possuir no máximo 150 caracteres.")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(80, ErrorMessage = "O bairro deve possuir no máximo 80 caracteres.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [StringLength(80, ErrorMessage = "A cidade deve possuir no máximo 80 caracteres.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [RegularExpression(@"^(AC|AL|AP|AM|BA|CE|DF|ES|GO|MA|MT|MS|MG|PA|PB|PR|PE|PI|RJ|RN|RS|RO|RR|SC|SP|SE|TO)$", ErrorMessage = "Selecione uma UF válida.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [RegularExpression(@"^(\d{8}|\d{5}-\d{3})$", ErrorMessage = "O CEP deve possuir 8 números.")]
        public string CEP { get; set; } = string.Empty;

        [RegularExpression(@"^(\d{10,11}|\(\d{2}\) \d{4,5}-\d{4})$", ErrorMessage = "O telefone secundário deve possuir 10 ou 11 números.")]
        public string? TelefoneSecundario { get; set; }

        public static readonly string[] EstadosCivis =
            { "Solteiro", "Casado", "União estável", "Separado", "Divorciado", "Viúvo" };

        public static readonly string[] Ufs =
            { "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO" };

        [RegularExpression(@"^\d{1,10}$", ErrorMessage = "A matrícula deve conter somente números e ter no máximo 10 dígitos.")]
        [Range(1, int.MaxValue, ErrorMessage = "A matrícula deve estar entre 1 e 2.147.483.647.")]
        public int? Matricula { get; set; }

        [StringLength(100, ErrorMessage = "O curso deve possuir no máximo 100 caracteres.")]
        public string? Curso { get; set; }

        public Guid? CursoId { get; set; }

        public List<Guid> CursosProfessorIds { get; set; } = new();

        public Guid? UnidadeConcedenteId { get; set; }

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