using System;
using System.ComponentModel.DataAnnotations;

namespace SWES.Models
{
    public class Documento
    {
        public int Id { get; private set; }
        public string Tipo { get; private set; } = string.Empty;
        public string Status { get; private set; } = string.Empty;
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }
        public DateTime? DataEnvio { get; private set; }

        // Nome explicativo dado pelo usuário no modal (ex.: "Contrato")
        [MaxLength(150)]
        public string NomeExplicativo { get; private set; } = string.Empty;

        // Nome do arquivo como o usuário enviou (ex.: contrato_final.pdf)
        [MaxLength(255)]
        public string NomeArquivoOriginal { get; private set; } = string.Empty;

        // Nome do arquivo guardado no servidor (GUID + extensão)
        [MaxLength(500)]
        public string CaminhoArquivo { get; private set; } = string.Empty;

        public long TamanhoBytes { get; private set; }

        public int EstagioId { get; private set; }
        public Estagio Estagio { get; private set; } = null!;

        private Documento() { }

        public Documento(string tipo, int estagioId)
        {
            Tipo = tipo;
            Status = "Pendente";
            DataCriacao = DateTime.Now;
            EstagioId = estagioId;
        }

        // Documento anexado pelo modal do cadastro de estágio.
        // O EstagioId é preenchido pelo EF quando o documento é adicionado em estagio.Documentos.
        public Documento(
            string nomeExplicativo,
            string nomeArquivoOriginal,
            string caminhoArquivo,
            long tamanhoBytes)
        {
            Tipo = "Anexo";
            Status = "Enviado";
            DataCriacao = DateTime.Now;
            DataEnvio = DateTime.Now;
            NomeExplicativo = nomeExplicativo;
            NomeArquivoOriginal = nomeArquivoOriginal;
            CaminhoArquivo = caminhoArquivo;
            TamanhoBytes = tamanhoBytes;
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