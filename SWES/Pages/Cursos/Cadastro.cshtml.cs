using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Cursos;

public class CadastroModel : PageModel
{
    private readonly CursoService _cursoService;

    public CadastroModel(CursoService cursoService)
    {
        _cursoService = cursoService;
    }

    [BindProperty]
    public CursoCadastroViewModel Dados { get; set; } = new();

    public List<SelectListItem> Instituicoes { get; private set; } = new();


    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id.HasValue)
        {
            var curso = await _cursoService.ObterPorIdAsync(id.Value);

            if (curso == null)
            {
                return NotFound();
            }

            Dados = new CursoCadastroViewModel
            {
                Id = curso.Id,
                Nome = curso.Nome,
                QuantidadeSemestres = curso.QuantidadeSemestres,
                Periodo = curso.Periodo,
                InstituicaoId = curso.InstituicaoId
            };
        }

        await CarregarInstituicoesAsync();

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CarregarInstituicoesAsync();
            return Page();
        }

        bool sucesso;

        if (Dados.Id.HasValue)
        {
            sucesso = await _cursoService.AtualizarAsync(Dados.Id.Value, Dados);
        }
        else
        {
            sucesso = await _cursoService.CadastrarAsync(Dados);
        }

        if (!sucesso)
        {
            ModelState.AddModelError(
                "Dados.InstituicaoId",
                "Não foi possível salvar o curso. Verifique se a instituição selecionada está ativa."
            );

            await CarregarInstituicoesAsync();
            return Page();
        }

        return RedirectToPage("./Index");
    }

    private async Task CarregarInstituicoesAsync()
    {
        Instituicoes = await _cursoService.ListarInstituicoesAsync();
    }
}