using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using SWES.Services;
using SWES.Authorization;
using System.Text.Json;

namespace SWES.Pages.Documentos
{
    [Authorize(Roles = PerfisSistema.Todos)]
    public class VisualizarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EstagioService _estagioService;

        public VisualizarModel(ApplicationDbContext context, EstagioService estagioService)
        {
            _context = context;
            _estagioService = estagioService;
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

            if (!User.IsInRole(PerfisSistema.Administrador) &&
                !User.IsInRole(PerfisSistema.Coordenador))
            {
                if (!documento.EstagioId.HasValue)
                    return Forbid();

                var idsPermitidos = await _estagioService
                    .ConsultarEstagiosPermitidos()
                    .Select(e => e.Id)
                    .ToListAsync();

                if (!idsPermitidos.Contains(documento.EstagioId.Value))
                    return Forbid();
            }

            Documento = documento;

            DadosDocumento = JsonDocument.Parse(documento.DadosJson);

            return Page();
        }
    }
}
