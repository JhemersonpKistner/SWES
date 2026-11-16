using System;

namespace SWES.Models
{
    public class Documento
    {
        public int Id { get; private set; }
        public string Tipo { get; private set; }

        public string? Nome { get; private set; }
        public string? NomeArquivo { get; private set; }
        public string? CaminhoArquivo { get; private set; }

        public string Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }
        public DateTime? DataEnvio { get; private set; }

        public int EstagioId { get; private set; }
        public Estagio Estagio { get; private set; }

        private Documento() { }

        public Documento(string tipo, int estagioId)
        {
            Tipo = tipo;
            Status = "Pendente";
            DataCriacao = DateTime.Now;
            EstagioId = estagioId;
        }


        public Documento(
            string tipo,
            int estagioId,
            string nome,
            string nomeArquivo,
            string caminhoArquivo)
            : this(tipo, estagioId)
        {
            Nome = nome;
            NomeArquivo = nomeArquivo;
            CaminhoArquivo = caminhoArquivo;
            Status = "Enviado";
            DataEnvio = DateTime.Now;
        }


        public void Atualizar(string novoStatus)
        {
            Status = novoStatus;
            DataAtualizacao = DateTime.Now;
        }

        public void Enviar()
        {
            Status = "Enviado";
            DataEnvio = DateTime.Now;
        }

        public void Visualizar()
        {
            // Ação de visualização — lógica específica a definir com o time
        }
    }
}