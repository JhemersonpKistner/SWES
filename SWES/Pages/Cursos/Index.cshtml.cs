using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Cursos;

public class IndexModel : PageModel
{
    private readonly CursoService _cursoService;

    public IndexModel(CursoService cursoService)
    {
        _cursoService = cursoService;
    }

    [BindProperty(SupportsGet = true)]
    public string? Pesquisa { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Situacao { get; set; }

    public List<CursoViewModel> Cursos { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Cursos = await _cursoService.ListarAsync(Pesquisa, Situacao);
    }

    public async Task<IActionResult> OnPostAlterarSituacaoAsync(
        Guid id,
        bool ativar)
    {
        bool alterado = await _cursoService.AlterarSituacaoAsync(id, ativar);

        if (!alterado)
        {
            return NotFound();
        }

        return RedirectToPage(new
        {
            Pesquisa,
            Situacao
        });
    }
}