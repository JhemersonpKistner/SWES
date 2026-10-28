namespace SWES.ViewModels
{
    public class CursoViewModel
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int QuantidadeSemestres { get; set; }

        public string Periodo { get; set; } = string.Empty;

        public string InstituicaoNome { get; set; } = string.Empty;

        public int QuantidadeProfessores { get; set; }

        public bool Ativo { get; set; }
    }
}