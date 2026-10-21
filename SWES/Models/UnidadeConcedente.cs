using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class UnidadeConcedente : Base
    {
        public bool InstituicaoEnsino { get;  set; } = false;

        [MaxLength(150)]
        public string RazaoSocial { get; private set; } = string.Empty;

        [MaxLength(14)]
        public string Cnpj { get; private set; } = string.Empty;

        [MaxLength(200)]
        public string Endereco { get; private set; } = string.Empty;

        [MaxLength(12)]
        public string Telefone { get; private set; } = string.Empty;

        [MaxLength(150)]
        public string Email { get; private set; } = string.Empty;

        public ICollection<Estagio> Estagios { get; private set; } = new List<Estagio>();

        private UnidadeConcedente() { } // construtor exigido pelo Entity Framework Core

        
        public UnidadeConcedente(
            string razaoSocial,
            string cnpj,
            string endereco,
            string telefone,
            string email)
        {
            RazaoSocial = razaoSocial;
            Cnpj = cnpj;
            Endereco = endereco;
            Telefone = telefone;
            Email = email;
            Ativo = true;
        }

        public void AtualizarDados(
            string razaoSocial,
            string cnpj,
            string endereco,
            string telefone,
            string email)
        {
            RazaoSocial = razaoSocial;
            Cnpj = cnpj;
            Endereco = endereco;
            Telefone = telefone;
            Email = email;
            Alteracao = DateTime.UtcNow;
        }
         
        public void Inativar()
        {
            Ativo = false;
            Alteracao = DateTime.UtcNow;
        }

        public void Ativar()
        {
            Ativo = true;
            Alteracao = DateTime.UtcNow;
        }

    }
}