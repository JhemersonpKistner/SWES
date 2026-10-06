namespace SWES.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public int? Inscricao { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        // Dados do aluno
        public int? Matricula { get; set; }

        public string? Curso { get; set; }

        public string? Turno { get; set; }

        public string? Semestre { get; set; }

        public DateTime? DataNasc { get; set; }

        // Dados do professor
        public int? Registro { get; set; }

        public List<string> CursosProfessor { get; set; } = new();

        // Dados do supervisor
        public string? Cargo { get; set; }

        public string? Empresa { get; set; }

        public string? CNPJ { get; set; }

        // Dados do coordenador
        public string? Setor { get; set; }
    }
}