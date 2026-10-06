namespace SWES.ViewModels
{
    public class UsuarioExportacaoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty;

        // Aluno
        public string Matricula { get; set; } = string.Empty;
        public string Curso { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public string DataNasc { get; set; } = string.Empty;

        // Professor
        public string Registro { get; set; } = string.Empty;

        // Supervisor
        public string Cargo { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;

        // Coordenador
        public string Setor { get; set; } = string.Empty;
    }
}