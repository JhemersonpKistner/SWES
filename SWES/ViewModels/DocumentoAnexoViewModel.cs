using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class DocumentoAnexoViewModel
    {
        [Required(ErrorMessage = "Informe o nome do documento.")]
        [StringLength(150, ErrorMessage = "O nome do documento deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um arquivo.")]
        public IFormFile? Arquivo { get; set; }
    }
}