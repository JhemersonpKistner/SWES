
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;

namespace SWES.Pages.InstituicoesEnsino
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<UnidadeConcedente> Instituicoes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        public async Task OnGetAsync()
        {
            var consulta = _context.UnidadesConcedentes
                .AsNoTracking()
                .Where(i => i.InstituicaoEnsino);

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                consulta = consulta.Where(i =>
                    i.RazaoSocial.Contains(Pesquisa) ||
                    i.Cnpj.Contains(Pesquisa));
            }

            Instituicoes = await consulta
                .OrderBy(i => i.RazaoSocial)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostInativarAsync(Guid id)
        {
            var instituicao = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(i =>
                    i.Id == id &&
                    i.InstituicaoEnsino);

            if (instituicao == null)
                return NotFound();

            instituicao.Inativar();

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAtivarAsync(Guid id)
        {
            var instituicao = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(i =>
                    i.Id == id &&
                    i.InstituicaoEnsino);

            if (instituicao == null)
                return NotFound();

            instituicao.Ativar();

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
