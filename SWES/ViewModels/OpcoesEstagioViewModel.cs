namespace SWES.ViewModels
{
    public class OpcoesEstagioViewModel
    {
        public List<OpcaoEstagioViewModel> Professores { get; set; } = new();

        public List<OpcaoEstagioViewModel> Supervisores { get; set; } = new();

        public List<OpcaoEstagioViewModel> UnidadesConcedentes { get; set; } = new();

        public List<OpcaoEstagioViewModel> InstituicoesEnsino { get; set; } = new();

        public List<OpcaoEstagioViewModel> Coordenadores { get; set; } = new();

        public List<OpcaoEstagioViewModel> Alunos { get; set; } = new();
    }

    public class OpcaoEstagioViewModel
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}