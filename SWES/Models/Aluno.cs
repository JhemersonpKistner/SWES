using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SWES.Models
{
    public class Aluno : Usuario
    {
        public int Matricula { get; private set; }
        [MaxLength(100)]

        public string Curso { get; private set; }
        [MaxLength(20)]
        public string Turno { get; private set; }
        [MaxLength(20)]
        public string Semestre { get; private set; }
        public DateTime DataNasc { get; private set; }

        public ICollection<Estagio> Estagios { get; private set; } = new List<Estagio>();

        private Aluno() { }

        public Aluno(
            string userId,
            string nome,
            string email,
            string telefone,
            string cpf,
            int matricula,
            string curso,
            string turno,
            string semestre,
            DateTime dataNasc)
            : base(userId, nome, email, telefone, cpf)
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

        public void AtualizarDadosAcademicos(int matricula, string curso, string turno, string semestre, DateTime dataNasc)
        {
            Matricula = matricula;
            Curso = curso;
            Turno = turno;
            Semestre = semestre;
            DataNasc = dataNasc;
        }

        internal void AtualizarDadosAcademicos(int? matricula, string? curso, string? turno, string? semestre, DateTime value)
        {
            throw new NotImplementedException();
        }
    }
}
