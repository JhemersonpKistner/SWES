using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class UsuarioCadastroViewModel
    {
        
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? Senha { get; set; }
        public string Telefone { get; set; }
        public string Tipo { get; set; }
        [Required(ErrorMessage = "Informe o CPF.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 números.")]
        public string CPF { get; set; }


        public int? Matricula { get; set; }
        public string? Curso { get; set; }
        public string? Turno { get; set; }
        public string? Semestre { get; set; }
        public DateTime? DataNasc { get; set; }

        
        public int? Registro { get; set; }        

        
        public string? Cargo { get; set; }
        public string? Empresa { get; set; }
        [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter exatamente 14 números.")]
        public string? CNPJ { get; set; }


        public string? Setor { get; set; }
    }
}