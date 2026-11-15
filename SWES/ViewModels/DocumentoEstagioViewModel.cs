using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SWES.ViewModels
{
    // Documento novo enviado pelo modal
    public class DocumentoNovoViewModel
    {
        [StringLength(150, ErrorMessage = "O nome do documento deve possuir no máximo 150 caracteres.")]
        public string? NomeExplicativo { get; set; }

        public IFormFile? Arquivo { get; set; }
    }

    // Documento já salvo no estágio
    public class DocumentoEstagioViewModel
    {
        public int Id { get; set; }
        public string NomeExplicativo { get; set; } = string.Empty;
        public string NomeArquivoOriginal { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime? DataEnvio { get; set; }
    }
}