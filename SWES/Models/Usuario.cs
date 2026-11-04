using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Usuario : Base
    {
        public string UserId { get; private set; } = string.Empty;

        [MaxLength(60)]
        public string Nome { get; private set; } = string.Empty;

        [MaxLength(150)]
        public string Email { get; private set; } = string.Empty;

        [MaxLength(12)]
        public string Telefone { get; private set; } = string.Empty;

        [MaxLength(12)]
        public string? TelefoneSecundario { get; private set; }

        [MaxLength(11)]
        public string CPF { get; private set; } = string.Empty;

        [MaxLength(20)]
        public string RG { get; private set; } = string.Empty;

        [MaxLength(20)]
        public string OrgaoExpedidor { get; private set; } = string.Empty;

        [MaxLength(30)]
        public string EstadoCivil { get; private set; } = string.Empty;

        [MaxLength(60)]
        public string Nacionalidade { get; private set; } = string.Empty;

        [MaxLength(200)]
        public string Endereco { get; private set; } = string.Empty;

        [MaxLength(100)]
        public string Bairro { get; private set; } = string.Empty;

        [MaxLength(100)]
        public string Cidade { get; private set; } = string.Empty;

        [MaxLength(2)]
        public string Estado { get; private set; } = string.Empty;

        [MaxLength(9)]
        public string CEP { get; private set; } = string.Empty;

        protected Usuario() { }

        protected Usuario(
            string userId,
            string nome,
            string email,
            string telefone,
            string cpf)
        {
            UserId = userId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            CPF = cpf;
            Ativo = true;
        }

        public void AtualizarDados(
            string nome,
            string email,
            string telefone,
            string cpf)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            CPF = cpf;
            Alteracao = DateTime.UtcNow;
        }
        public void AtualizarDados(
            string nome,
            string email,
            string telefone,
            string cpf,
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
            Nome = nome;
            Email = email;
            Telefone = telefone;
            CPF = cpf;
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
            Alteracao = DateTime.UtcNow;
        }
    }
}