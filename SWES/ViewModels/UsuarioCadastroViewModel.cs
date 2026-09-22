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

        
        public string? Matricula { get; set; }
        public string? Curso { get; set; }
        public string? Turno { get; set; }
        public string? Semestre { get; set; }
        public DateTime? DataNasc { get; set; }

        
        public string? Registro { get; set; }

        
        public string? Cargo { get; set; }
        public string? Empresa { get; set; }

        
        public string? Setor { get; set; }
    }
}