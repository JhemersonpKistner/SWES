using System;

namespace SWES.Models
{
    public class Atividade
    {
        public int Id { get; private set; }
        public string Descricao { get; private set; }
        public DateTime Data { get; private set; }
        public int Horas { get; private set; }

        public int EstagioId { get; private set; }
        public Estagio Estagio { get; private set; }

        private Atividade() { }

        public Atividade(string descricao, DateTime data, int horas, int estagioId)
        {
            Descricao = descricao;
            Data = data;
            Horas = horas;
            EstagioId = estagioId;
        }

        public void Atualizar(string novaDescricao, int novasHoras)
        {
            Descricao = novaDescricao;
            Horas = novasHoras;
        }
    }
}