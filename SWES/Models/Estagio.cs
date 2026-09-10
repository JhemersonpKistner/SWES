using System;
using System.Collections.Generic;
using System.Linq;

namespace SWES.Models
{
    public class Estagio
    {
        public int Id { get; private set; }
        public string Tipo { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public string Horario { get; private set; }
        public string Situacao { get; private set; }
        public string Observacao { get; private set; }

        public int AlunoId { get; private set; }
        public Aluno Aluno { get; private set; }

        public int ProfessorOrientadorId { get; private set; }
        public ProfessorOrientador ProfessorOrientador { get; private set; }

        public int SupervisorEstagioId { get; private set; }
        public SupervisorEstagio SupervisorEstagio { get; private set; }

        public int? CoordenadorId { get; private set; }
        public Coordenador Coordenador { get; private set; }

        public int UnidadeConcedenteId { get; private set; }
        public UnidadeConcedente UnidadeConcedente { get; private set; }

        public int InstituicaoEnsinoId { get; private set; }
        public InstituicaoEnsino InstituicaoEnsino { get; private set; }

        public ICollection<Documento> Documentos { get; private set; } = new List<Documento>();
        public ICollection<Atividade> Atividades { get; private set; } = new List<Atividade>();
        public ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();

        private Estagio() { }

        public Estagio(string tipo, DateTime dataInicio, DateTime dataFim, string horario,
            int alunoId, int professorOrientadorId, int supervisorEstagioId,
            int unidadeConcedenteId, int instituicaoEnsinoId)
        {
            Tipo = tipo;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Horario = horario;
            Situacao = "Em andamento";
            AlunoId = alunoId;
            ProfessorOrientadorId = professorOrientadorId;
            SupervisorEstagioId = supervisorEstagioId;
            UnidadeConcedenteId = unidadeConcedenteId;
            InstituicaoEnsinoId = instituicaoEnsinoId;
        }

        public void AlterarSituacao(string novaSituacao)
        {
            Situacao = novaSituacao;
        }

        public int CalcularCargaHoraria()
        {
            return Atividades?.Sum(a => a.Horas) ?? 0;
        }

        public bool EstaAtivo()
        {
            return Situacao == "Em andamento" && DateTime.Now <= DataFim;
        }
    }
}