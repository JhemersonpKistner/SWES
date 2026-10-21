
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;

namespace SWES.Pages.UnidadesConcedentes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<UnidadeConcedente> UnidadesConcedentes { get; set; }
            = new();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        public async Task OnGetAsync()
        {
            var consulta = _context.UnidadesConcedentes
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                Pesquisa = Pesquisa.Trim();

                consulta = consulta.Where(u =>
                    u.RazaoSocial.Contains(Pesquisa) ||
                    u.Cnpj.Contains(Pesquisa));
            }

            UnidadesConcedentes = await consulta
                .OrderBy(u => u.RazaoSocial)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostInativarAsync(Guid id)
        {
            var unidade = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unidade == null)
                return NotFound();

            unidade.Inativar();

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAtivarAsync(Guid id)
        {
            var unidade = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(u => u.Id == id);

            if (unidade == null)
                return NotFound();

            unidade.Ativar();

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
