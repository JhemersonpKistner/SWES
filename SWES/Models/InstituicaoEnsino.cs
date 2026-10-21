using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class InstituicaoEnsino : Base
    {
        [MaxLength(150)]
        public string Nome { get; private set; } = string.Empty;

        [MaxLength(14)]
        public string Cnpj { get; private set; } = string.Empty;

        public ICollection<Estagio> Estagios { get; private set; } = new List<Estagio>();

        private InstituicaoEnsino() { }

        public InstituicaoEnsino(string nome, string cnpj)
        {
            Nome = nome;
            Cnpj = cnpj;
        }
    }
}