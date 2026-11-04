using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class SupervisorEstagio : Usuario
    {
        [MaxLength(60)]
        public string Cargo { get; private set; }

        public Guid? UnidadeConcedenteId { get; private set; }
        public UnidadeConcedente? UnidadeConcedente { get; private set; }

        private SupervisorEstagio() { }

        public SupervisorEstagio(
            string userId,
            string nome,
            string email,
            string telefone,
            string cpf,
            string cargo,
            Guid unidadeConcedenteId)
            : base(userId, nome, email, telefone, cpf)
        {
            Cargo = cargo;
            UnidadeConcedenteId = unidadeConcedenteId;
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

        public void AtualizarDadosProfissionais(string cargo, Guid unidadeConcedenteId)
        {
            Cargo = cargo;
            UnidadeConcedenteId = unidadeConcedenteId;
            Alteracao = DateTime.UtcNow;
        }
    }
}