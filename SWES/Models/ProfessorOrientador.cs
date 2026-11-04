using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class ProfessorOrientador : Usuario
    {
        public int Registro { get; private set; }

        public ICollection<Curso> Cursos { get; private set; } = new List<Curso>();

        private ProfessorOrientador() { }

        public ProfessorOrientador(
            string userId,
            string nome,
            string email,
            string telefone,
            string cpf,
            int registro)
            : base(userId, nome, email, telefone, cpf)
        {
            Registro = registro;
        }

        public void AdicionarCurso(Curso curso)
        {
            if (!Cursos.Contains(curso))
            {
                Cursos.Add(curso);
            }
        }

        public void RemoverCurso(Curso curso)
        {
            Cursos.Remove(curso);
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

        public void AtualizarRegistro(int registro)
        {
            Registro = registro;
        }

        public void DefinirCursos(IEnumerable<Curso> cursos)
        {
            Cursos.Clear();

            foreach (var curso in cursos)
            {
                Cursos.Add(curso);
            }

            Alteracao = DateTime.UtcNow;
        }
    }
}