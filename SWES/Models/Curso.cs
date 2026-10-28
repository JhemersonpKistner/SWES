using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Curso : Base
    {

        [MaxLength(100)]
        public string Nome { get; private set; }

        public int QuantidadeSemestres { get; private set; }

        [MaxLength(20)]
        public string Periodo { get; private set; } = string.Empty;

        public Guid InstituicaoId { get; private set; }

        public UnidadeConcedente Instituicao { get; private set; } = null!;

        public ICollection<ProfessorOrientador> Professores { get; private set; } = new List<ProfessorOrientador>();

        private Curso() { }

        public Curso(
            string nome,
            int quantidadeSemestres,
            string periodo,
            Guid instituicaoId)
        {
            Nome = nome;
            QuantidadeSemestres = quantidadeSemestres;
            Periodo = periodo;
            InstituicaoId = instituicaoId;
            Ativo = true;
        }

        public void AtualizarDados(
            string nome,
            int quantidadeSemestres,
            string periodo,
            Guid instituicaoId)
        {
            Nome = nome;
            QuantidadeSemestres = quantidadeSemestres;
            Periodo = periodo;
            InstituicaoId = instituicaoId;
            Alteracao = DateTime.UtcNow;
        }

        public void Desativar()
        {
            Ativo = false;
            Alteracao = DateTime.UtcNow;
        }

        public void Ativar()
        {
            Ativo = true;
            Alteracao = DateTime.UtcNow;
        }
    }
}