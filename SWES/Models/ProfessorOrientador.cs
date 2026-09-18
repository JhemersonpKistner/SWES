using System;

namespace SWES.Models
{
    public class ProfessorOrientador : Usuario
    {
        public string Registro { get; private set; }

        private ProfessorOrientador() { }

        public ProfessorOrientador(
            string userId,
            string nome,
            string email,
            string telefone,
            string registro)
            : base(userId, nome, email, telefone)
        {
            Registro = registro;
        }

        public void AcompanharEstagio(Estagio estagio)
        {
            // Registrar acompanhamento pelo professor orientador
        }

        public bool AnalisarDocumento(Documento documento)
        {
            return documento.Status == "Enviado";
        }

        public void EmitirParecer()
        {
            // Gerar parecer do professor orientador sobre o estágio
        }

        public void RegistrarAvaliacao(Estagio estagio, Avaliacao avaliacao)
        {
            estagio.Avaliacoes.Add(avaliacao);
        }
    }
}