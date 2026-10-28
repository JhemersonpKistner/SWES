
using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class UnidadeConcedenteViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "A razão social é obrigatória.")]
        [MaxLength(150, ErrorMessage = "A razão social deve ter no máximo 150 caracteres.")]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [RegularExpression(
    @"^(\d{14}|\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})$",
    ErrorMessage = "Informe um CNPJ válido no formato 00.000.000/0000-00.")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [MaxLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres.")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [MaxLength(12, ErrorMessage = "O telefone deve ter no máximo 12 caracteres.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [MaxLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;
        
        public bool InstituicaoEnsino { get; set; } = false;
    }
}
