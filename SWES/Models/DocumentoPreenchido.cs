
namespace SWES.Models
{
    public class DocumentoPreenchido
    {
        public int Id { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string DadosJson { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public string? UsuarioId { get; set; }

        public int? EstagioId { get; set; }

        public Estagio? Estagio { get; set; }

    }
}
