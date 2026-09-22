namespace SWES.Models
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string UserId { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public bool Ativo { get; private set; }

        protected Usuario() { }

        protected Usuario(string userId, string nome, string email, string telefone)
        {
            UserId = userId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Ativo = true;
        }

        public void AtualizarDados(string nome, string email, string telefone)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
        }

        public void Inativar()
        {
            Ativo = false;
        }

        public void Ativar()
        {
            Ativo = true;
        }

    }
}
