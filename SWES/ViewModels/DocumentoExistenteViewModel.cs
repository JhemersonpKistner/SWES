namespace SWES.ViewModels
{
    public class DocumentoExistenteViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? NomeArquivo { get; set; }

        public string? CaminhoArquivo { get; set; }
    }
}