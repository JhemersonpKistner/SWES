using System;
using System.Collections.Generic;

namespace SWES.Models
{
    public class ProfessorOrientador
    {
        public int Id { get; private set; }

        public string UserId { get; private set; }

        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public string Registro { get; private set; }

        private ProfessorOrientador() { }

        public ProfessorOrientador(string userId, string nome, string email, string telefone, string registro)
        {
            UserId = userId;
            Nome = nome;
            Email = email;
            Telefone = telefone;
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