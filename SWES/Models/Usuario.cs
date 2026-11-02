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

        [MaxLength(20)]
        public string? RG { get; private set; }
        [MaxLength(20)]
        public string? OrgaoExpedidor { get; private set; }
        [MaxLength(20)]
        public string? EstadoCivil { get; private set; }
        [MaxLength(50)]
        public string? Nacionalidade { get; private set; }
        [MaxLength(150)]
        public string? Endereco { get; private set; }
        [MaxLength(80)]
        public string? Bairro { get; private set; }
        [MaxLength(80)]
        public string? Cidade { get; private set; }
        [MaxLength(2)]
        public string? Estado { get; private set; }
        [MaxLength(8)]
        public string? CEP { get; private set; }
        [MaxLength(12)]
        public string? TelefoneSecundario { get; private set; }

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

        public void AtualizarDadosPessoais(
            string rg,
            string orgaoExpedidor,
            string estadoCivil,
            string nacionalidade,
            string endereco,
            string bairro,
            string cidade,
            string estado,
            string cep,
            string? telefoneSecundario)
        {
            RG = rg;
            OrgaoExpedidor = orgaoExpedidor;
            EstadoCivil = estadoCivil;
            Nacionalidade = nacionalidade;
            Endereco = endereco;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            CEP = cep;
            TelefoneSecundario = telefoneSecundario;
        }

    }
}
