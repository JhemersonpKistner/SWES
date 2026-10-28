using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class InstituicaoEnsinoViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "O nome da instituição é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [RegularExpression(
    @"^(\d{14}|\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})$",
    ErrorMessage = "Informe um CNPJ válido no formato 00.000.000/0000-00.")]
        public string Cnpj { get; set; } = string.Empty;
    }
}
