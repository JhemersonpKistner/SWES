namespace SWES.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public int? Inscricao { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }
}