using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Curso
    {
        public int Id { get; private set; }

        [MaxLength(100)]
        public string Nome { get; private set; }

        public bool Ativo { get; private set; }

        public ICollection<ProfessorOrientador> Professores { get; private set; } = new List<ProfessorOrientador>();

        private Curso() { }

        public Curso(string nome)
        {
            Nome = nome;
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }

        public void Ativar()
        {
            Ativo = true;
        }
    }
}