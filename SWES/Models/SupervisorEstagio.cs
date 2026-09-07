using System;
using System.Collections.Generic;

namespace SWES.Models
{
    public class SupervisorEstagio
    {
        public int Id { get; private set; }

        public string UserId { get; private set; }

        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public string Cargo { get; private set; }
        public string Empresa { get; private set; }

        private SupervisorEstagio() { }

        public SupervisorEstagio(string userId, string nome, string email, string telefone,
            string cargo, string empresa)
        {
            UserId = userId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Cargo = cargo;
            Empresa = empresa;
        }

        public void AcompanharEstagio(Estagio estagio)
        {
            // Registrar acompanhamento pelo supervisor
        }

        public bool ValidarAtividade(Atividade atividade)
        {
            return atividade.Horas > 0;
        }

        public bool AnalisarDocumento(Documento documento)
        {
            return documento.Status == "Enviado";
        }

        public void EmitirParecer()
        {
            // Gerar parecer do supervisor sobre o estágio
        }
    }
}