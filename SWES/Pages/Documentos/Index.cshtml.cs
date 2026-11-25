using Microsoft.AspNetCore.Authorization;
using SWES.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;

namespace SWES.Pages.Documentos
{
    [Authorize(Roles = PerfisSistema.Todos)]
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
            var consulta = _context.DocumentosPreenchidos
                .AsNoTracking()
                .Include(d => d.Estagio)
                .AsQueryable();

            if (!User.IsInRole(PerfisSistema.Administrador) &&
                !User.IsInRole(PerfisSistema.Coordenador))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    DocumentosSalvos.Clear();
                    return;
                }

                var professor = User.IsInRole(PerfisSistema.Professor);
                var supervisor = User.IsInRole(PerfisSistema.Supervisor);
                var aluno = User.IsInRole(PerfisSistema.Aluno);

                consulta = consulta.Where(d =>
                    (professor && d.Estagio.ProfessorOrientador.UserId == userId) ||
                    (supervisor && d.Estagio.SupervisorEstagio.UserId == userId) ||
                    (aluno && d.Estagio.Alunos.Any(a => a.UserId == userId)));
            }

            DocumentosSalvos = await consulta
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();
        }
    }
}