namespace SWES.ViewModels
{
    public class EstagioViewModel
    {
        public int Id { get; set; }

        public string Etapa { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public string Horario { get; set; } = string.Empty;

        public string Situacao { get; set; } = string.Empty;

        public string ProfessorOrientador { get; set; } = string.Empty;

        public string Supervisor { get; set; } = string.Empty;

        public string UnidadeConcedente { get; set; } = string.Empty;

        public string InstituicaoEnsino { get; set; } = string.Empty;

        public List<string> Alunos { get; set; } = new();

        public string ApoliceSeguro { get; set; } = string.Empty;

        public string Seguradora { get; set; } = string.Empty;
    }
}