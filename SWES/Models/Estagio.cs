using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SWES.Models
{
    public class Estagio
    {
        public int Id { get; private set; }

        [MaxLength(50)]
        public string Tipo { get; private set; } = string.Empty;

        [MaxLength(20)]
        public string Etapa { get; private set; } = "Estágio I";

        public DateTime DataInicio { get; private set; }

        public DateTime DataFim { get; private set; }

        [MaxLength(100)]
        public string Horario { get; private set; } = string.Empty;

        [MaxLength(50)]
        public string Situacao { get; private set; } = "Em andamento";

        [MaxLength(500)]
        public string Observacao { get; private set; } = string.Empty;

        [MaxLength(100)]
        public string ApoliceSeguro { get; private set; } = string.Empty;

        [MaxLength(150)]
        public string Seguradora { get; private set; } = string.Empty;

        public string? EspecificacoesHtml { get; private set; }

        [MaxLength(500)]
        public string? EspecificacoesArquivo { get; private set; }

        [MaxLength(255)]
        public string? NomeArquivoEspecificacoes { get; private set; }

        [MaxLength(500)]
        public string? EditalArquivo { get; private set; }

        [MaxLength(255)]
        public string? NomeArquivoEdital { get; private set; }

        public Guid ProfessorOrientadorId { get; private set; }
        public ProfessorOrientador ProfessorOrientador { get; private set; } = null!;

        public Guid SupervisorEstagioId { get; private set; }
        public SupervisorEstagio SupervisorEstagio { get; private set; } = null!;

        public Guid? CoordenadorId { get; private set; }
        public Coordenador? Coordenador { get; private set; }

        public Guid UnidadeConcedenteId { get; private set; }
        public UnidadeConcedente UnidadeConcedente { get; private set; } = null!;

        public Guid InstituicaoEnsinoId { get; private set; }
        public UnidadeConcedente InstituicaoEnsino { get; private set; } = null!;

        public ICollection<Aluno> Alunos { get; private set; } = new List<Aluno>();

        public ICollection<Documento> Documentos { get; private set; } = new List<Documento>();

        public ICollection<Atividade> Atividades { get; private set; } = new List<Atividade>();

        public ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();

        private Estagio() { }

        public Estagio(
            string tipo,
            string etapa,
            DateTime dataInicio,
            DateTime dataFim,
            string horario,
            Guid professorOrientadorId,
            Guid supervisorEstagioId,
            Guid unidadeConcedenteId,
            Guid instituicaoEnsinoId)
        {
            Tipo = tipo;
            Etapa = etapa;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Horario = horario;
            Situacao = "Em andamento";
            ProfessorOrientadorId = professorOrientadorId;
            SupervisorEstagioId = supervisorEstagioId;
            UnidadeConcedenteId = unidadeConcedenteId;
            InstituicaoEnsinoId = instituicaoEnsinoId;
        }

        public void AtualizarInformacoes(
            string tipo,
            string etapa,
            DateTime dataInicio,
            DateTime dataFim,
            string horario,
            string situacao,
            string observacao,
            string apoliceSeguro,
            string seguradora,
            string? especificacoesHtml,
            string? especificacoesArquivo,
            string? nomeArquivoEspecificacoes,
            string? editalArquivo,
            string? nomeArquivoEdital)
        {
            Tipo = tipo;
            Etapa = etapa;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Horario = horario;
            Situacao = situacao;
            Observacao = observacao;
            ApoliceSeguro = apoliceSeguro;
            Seguradora = seguradora;
            EspecificacoesHtml = especificacoesHtml;
            EspecificacoesArquivo = especificacoesArquivo;
            NomeArquivoEspecificacoes = nomeArquivoEspecificacoes;
            EditalArquivo = editalArquivo;
            NomeArquivoEdital = nomeArquivoEdital;
        }

        public void AlterarSituacao(string novaSituacao)
        {
            Situacao = novaSituacao;
        }

        public void AdicionarAluno(Aluno aluno)
        {
            if (!Alunos.Contains(aluno))
                Alunos.Add(aluno);
        }

        public void RemoverAluno(Aluno aluno)
        {
            Alunos.Remove(aluno);
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