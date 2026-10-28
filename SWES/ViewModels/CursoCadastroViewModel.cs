using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class CursoCadastroViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do curso.")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a quantidade de semestres.")]
        [Range(1, 20, ErrorMessage = "A quantidade deve estar entre 1 e 20.")]
        public int QuantidadeSemestres { get; set; }

        [Required(ErrorMessage = "Selecione o período.")]
        [MaxLength(20, ErrorMessage = "O período deve ter no máximo 20 caracteres.")]
        public string Periodo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione uma instituição de ensino.")]
        public Guid InstituicaoId { get; set; }
    }
}