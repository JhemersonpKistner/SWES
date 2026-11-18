using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using System.Text.Json;

namespace SWES.Pages.Documentos
{
    [AllowAnonymous]
    public class VisualizarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public VisualizarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public DocumentoPreenchido Documento { get; set; } = null!;

        public JsonDocument? DadosDocumento { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var documento = await _context.DocumentosPreenchidos
                .AsNoTracking()
                .Include(d => d.Estagio)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
                return NotFound();

            Documento = documento;

            DadosDocumento = JsonDocument.Parse(documento.DadosJson);

            return Page();
        }
    }
}