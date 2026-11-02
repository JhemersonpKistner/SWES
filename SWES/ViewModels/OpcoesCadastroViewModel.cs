namespace SWES.ViewModels
{
    public class OpcaoViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class OpcoesCadastroViewModel
    {
        public List<OpcaoViewModel> Cursos { get; set; } = new();
        public List<OpcaoViewModel> UnidadesConcedentes { get; set; } = new();
    }
}