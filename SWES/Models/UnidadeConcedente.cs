using System.Collections.Generic;

namespace SWES.Models
{
    public class UnidadeConcedente
    {
        public int Id { get; private set; }
        public string RazaoSocial { get; private set; }
        public string Cnpj { get; private set; }
        public string Endereco { get; private set; }
        public string Telefone { get; private set; }
        public string Email { get; private set; }

        public ICollection<Estagio> Estagios { get; private set; } = new List<Estagio>();

        private UnidadeConcedente() { } // construtor exigido pelo Entity Framework Core

        public UnidadeConcedente(string razaoSocial, string cnpj, string endereco, string telefone, string email)
        {
            RazaoSocial = razaoSocial;
            Cnpj = cnpj;
            Endereco = endereco;
            Telefone = telefone;
            Email = email;
        }
    }
}