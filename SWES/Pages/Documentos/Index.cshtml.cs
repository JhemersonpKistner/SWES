using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using System.Security.Claims;

namespace SWES.Pages.Documentos
{
    [Authorize]
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
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DocumentosSalvos = await _context.DocumentosPreenchidos
                .AsNoTracking()
                .Include(d => d.Estagio)
                .Where(d => d.UsuarioId == usuarioId)
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();
        }

    }
}