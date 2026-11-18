using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;

namespace SWES.Pages.Documentos
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DocumentoPreenchido> DocumentosSalvos { get; set; } = new();

        public async Task OnGetAsync()
        {
            DocumentosSalvos = await _context.DocumentosPreenchidos
                .AsNoTracking()
                .Include(d => d.Estagio)
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();
        }
    }
}