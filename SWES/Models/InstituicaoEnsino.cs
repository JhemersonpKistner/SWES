using System.Collections.Generic;

namespace SWES.Models
{
    public class InstituicaoEnsino
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Cnpj { get; private set; }

        public ICollection<Estagio> Estagios { get; private set; } = new List<Estagio>();

        private InstituicaoEnsino() { }

        public InstituicaoEnsino(string nome, string cnpj)
        {
            Nome = nome;
            Cnpj = cnpj;
        }
    }
}