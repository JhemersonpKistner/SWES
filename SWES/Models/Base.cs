using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Base
    {
        public Guid Id { get; set ; }
        public DateTime Cadastro { get; set ; }
        public DateTime? Alteracao { get; set ; }
        public bool Ativo { get; set ; }

}
