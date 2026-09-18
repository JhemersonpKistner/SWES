using System;
using System.Collections.Generic;
using System.Linq;

namespace SWES.Models
{
    public class Aluno : Usuario
    {
        public string Matricula { get; private set; }
        public string Curso { get; private set; }
        public string Turno { get; private set; }
        public string Semestre { get; private set; }
        public DateTime DataNasc { get; private set; }

        public ICollection<Estagio> Estagios { get; private set; } = new List<Estagio>();

        private Aluno() { }

        public Aluno(
            string userId,
            string nome,
            string email,
            string telefone,
            string matricula,
            string curso,
            string turno,
            string semestre,
            DateTime dataNasc)
            : base(userId, nome, email, telefone)
        {
            Matricula = matricula;
            Curso = curso;
            Turno = turno;
            Semestre = semestre;
            DataNasc = dataNasc;
        }

        public string AcompanharSituacao()
        {
            var estagioAtivo = Estagios.FirstOrDefault(e => e.EstaAtivo());
            return estagioAtivo?.Situacao ?? "Sem estágio ativo";
        }
    }
}
