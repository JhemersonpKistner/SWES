using System;

namespace SWES.Models
{
    public class Avaliacao
    {
        public int Id { get; private set; }
        public decimal Nota { get; private set; }
        public string Observacao { get; private set; }
        public DateTime DataAvaliacao { get; private set; }

        public int EstagioId { get; private set; }
        public Estagio Estagio { get; private set; }

        public int ProfessorOrientadorId { get; private set; }
        public ProfessorOrientador ProfessorOrientador { get; private set; }

        private Avaliacao() { }

        public Avaliacao(int estagioId, int professorOrientadorId, decimal nota, string observacao)
        {
            EstagioId = estagioId;
            ProfessorOrientadorId = professorOrientadorId;
            Nota = nota;
            Observacao = observacao;
            DataAvaliacao = DateTime.Now;
        }

        public void Registrar()
        {
            DataAvaliacao = DateTime.Now;
        }

        public void Atualizar(decimal nota, string observacao)
        {
            Nota = nota;
            Observacao = observacao;
        }

        public void Remover()
        {
            // Marca/realiza remoção da avaliação (regra de negócio na camada de serviço)
        }
    }
}