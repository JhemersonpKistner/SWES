using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SWES.Authorization;
using SWES.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Services;
using SWES.ViewModels;

namespace SWES.Pages.Estagios
{
    [Authorize(Roles = PerfisSistema.Todos)]
    public class IndexModel : PageModel
    {
        private readonly EstagioService _estagioService;
        private readonly ApplicationDbContext _context;

        public IndexModel(EstagioService estagioService, ApplicationDbContext context)
        {
            _estagioService = estagioService;
            _context = context;
        }

        public List<EstagioViewModel> Estagios { get; set; } = new();

        public bool PodeGerenciar => User.IsInRole(PerfisSistema.Administrador)
            || User.IsInRole(PerfisSistema.Coordenador);

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        [TempData]
        public string? Mensagem { get; set; }

        [TempData]
        public string? Erro { get; set; }

        public async Task OnGetAsync()
        {
            Estagios = await _estagioService.ListarEstagiosAsync(Pesquisa);

            // Administrador e coordenação podem consultar todos os estágios.
            // Os demais perfis visualizam somente os estágios aos quais estão vinculados.
            if (PodeGerenciar)
                return;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                Estagios.Clear();
                return;
            }

            var professor = User.IsInRole(PerfisSistema.Professor);
            var supervisor = User.IsInRole(PerfisSistema.Supervisor);
            var aluno = User.IsInRole(PerfisSistema.Aluno);

            var idsPermitidos = await _context.Estagios.AsNoTracking()
                .Where(e =>
                    (professor && e.ProfessorOrientador.UserId == userId) ||
                    (supervisor && e.SupervisorEstagio.UserId == userId) ||
                    (aluno && e.Alunos.Any(a => a.UserId == userId)))
                .Select(e => e.Id)
                .ToListAsync();

            Estagios = Estagios.Where(e => idsPermitidos.Contains(e.Id)).ToList();
        }

        public async Task<IActionResult> OnPostExcluirAsync(int id)
        {
            if (!PodeGerenciar)
                return Forbid();

            try
            {
                var excluido = await _estagioService.ExcluirAsync(id);

                Mensagem = excluido
                    ? "Estágio excluído com sucesso!"
                    : "Estágio não encontrado.";
            }
            catch (InvalidOperationException ex)
            {
                Erro = ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostInativarAsync(int id)
        {
            if (!PodeGerenciar)
                return Forbid();

            try
            {
                var inativado = await _estagioService.InativarAsync(id);

                Mensagem = inativado
                    ? "Estágio inativado com sucesso!"
                    : "Estágio não encontrado.";
            }
            catch (InvalidOperationException ex)
            {
                Erro = ex.Message;
            }

            return RedirectToPage();
        }
    }
}