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

        public List<InstituicaoEnsino> Instituicoes { get; set; }
            = new();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        public async Task OnGetAsync()
        {
            var consulta = _context.InstituicoesEnsino
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                consulta = consulta.Where(i =>
                    i.Nome.Contains(Pesquisa) ||
                    i.Cnpj.Contains(Pesquisa));
            }

            Instituicoes = await consulta
                .OrderBy(i => i.Nome)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostInativarAsync(Guid id)
        {
            var instituicao = await _context.InstituicoesEnsino
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instituicao == null)
                return NotFound();

            instituicao.Inativar();

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAtivarAsync(Guid id)
        {
            var instituicao = await _context.InstituicoesEnsino
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instituicao == null)
                return NotFound();

            instituicao.Ativar();

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
