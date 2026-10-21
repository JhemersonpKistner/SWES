using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Usuario: Base
    {
        
        public string UserId { get; private set; }
        [MaxLength(60)]
        public string Nome { get; private set; }
        [MaxLength(150)]
        public string Email { get; private set; }
        [MaxLength(12)]
        public string Telefone { get; private set; }

        [MaxLength(11)]
        public String CPF {get; private set; }

        protected Usuario() { }

        protected Usuario(string userId, string nome, string email, string telefone, String cpf)

        {
            UserId = userId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Ativo = true;
            CPF = cpf;
        }

        public void AtualizarDados(string nome, string email, string telefone, string cpf)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            CPF = cpf;
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
