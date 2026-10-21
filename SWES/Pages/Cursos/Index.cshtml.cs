
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SWES.Pages.Cursos;

public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Pesquisa { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Situacao { get; set; }

    private static readonly List<CursoItem> CursosDemonstracao = new()
    {
        new() { Nome = "Sistemas de Informação",
                QuantidadeProfessores = 4, Ativo = true },

        new() { Nome = "Administração",
                QuantidadeProfessores = 3, Ativo = true },

        new() { Nome = "Ciências Contábeis",
                QuantidadeProfessores = 2, Ativo = true },

        new() { Nome = "Análise e Desenvolvimento de Sistemas",
                QuantidadeProfessores = 2, Ativo = false }
    };

    public List<CursoItem> Cursos { get; private set; } = new();

    public void OnGet()
    {
        IEnumerable<CursoItem> consulta = CursosDemonstracao;

        if (!string.IsNullOrWhiteSpace(Pesquisa))
        {
            consulta = consulta.Where(c =>
                c.Nome.Contains(
                    Pesquisa,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (Situacao == "Ativo")
        {
            consulta = consulta.Where(c => c.Ativo);
        }
        else if (Situacao == "Inativo")
        {
            consulta = consulta.Where(c => !c.Ativo);
        }

        Cursos = consulta.ToList();
    }

    public class CursoItem
    {
        public string Nome { get; set; } = string.Empty;
        public int QuantidadeProfessores { get; set; }
        public bool Ativo { get; set; }
    }
}
