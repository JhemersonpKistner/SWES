using System;

namespace SWES.Models
{
    public class ProfessorOrientador : Usuario
    {
        public string Curso {get; private set; }
        public int Registro { get; private set; }

        private ProfessorOrientador() { }

        public ProfessorOrientador(
            string userId,
            string nome,
            string email,
            string telefone,
            int cpf,
            string curso,
            int registro)
            : base(userId, nome, email, telefone, cpf)
        {
            Registro = registro;
            Curso = curso;
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

        public void AtualizarRegistro(Int16 registro)
        {
            Registro = registro;
        }
    }
}