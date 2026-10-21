using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Base
    {
        public Guid Id { get; set ; } = Guid.NewGuid();
        public DateTime Cadastro { get; set ; } = DateTime.UtcNow;
        public DateTime? Alteracao { get; set ; }
        public bool Ativo { get; set ; }
    }

}
