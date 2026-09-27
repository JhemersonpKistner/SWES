using System;
using System.Collections.Generic;

namespace SWES.Models
{
    public class SupervisorEstagio : Usuario
    {
        public int CNPJ {get; private set; }
        public string Cargo { get; private set; }
        public string Empresa { get; private set; }

        private SupervisorEstagio() { }

        public SupervisorEstagio(
            string userId,
            string nome,
            string email,
            string telefone,
            int cpf,
            int cnpj,
            string cargo,
            string empresa)
            : base(userId, nome, email, telefone, cpf)
        {
            CNPJ = cnpj;
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

        public void AtualizarDadosProfissionais(string cargo, string empresa)
        {
            Cargo = cargo;
            Empresa = empresa;
        }
    }
}