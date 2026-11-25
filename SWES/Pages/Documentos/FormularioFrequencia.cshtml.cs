using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using SWES.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Data;
using SWES.Models;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SWES.Services;

namespace SWES.Pages.Documentos
{
    [Authorize(Roles = PerfisSistema.Todos)]
    public class FormularioFrequenciaModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly EstagioService _estagioService;

        public FormularioFrequenciaModel(
            ApplicationDbContext context,
            EstagioService estagioService)
        {
            _context = context;
            _estagioService = estagioService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Informe o nome completo do aluno.")]
        [StringLength(150, ErrorMessage = "O nome do aluno deve ter no máximo 150 caracteres.")]
        public string NomeAluno { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Informe a data da atividade.")]
        public DateTime? DataAtividade { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Informe o horário de entrada.")]
        public TimeSpan? HorarioEntrada { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Informe o horário de saída.")]
        public TimeSpan? HorarioSaida { get; set; }

        [BindProperty]
        public string? AtividadesRealizadas { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Selecione o estágio.")]
        public int? EstagioId { get; set; }

        public List<Estagio> EstagiosDisponiveis { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            EstagiosDisponiveis = await _estagioService
                .ConsultarEstagiosPermitidos()
                .AsNoTracking()
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (HorarioEntrada.HasValue &&
                HorarioSaida.HasValue &&
                HorarioSaida.Value <= HorarioEntrada.Value)
            {
                ModelState.AddModelError(
                    nameof(HorarioSaida),
                    "O horário de saída deve ser posterior ao horário de entrada.");
            }

            if (EstagioId.HasValue)
            {
                var estagioPermitido = await _estagioService
                    .ConsultarEstagiosPermitidos()
                    .AnyAsync(e => e.Id == EstagioId.Value);

                if (!estagioPermitido)
                {
                    ModelState.AddModelError(
                        nameof(EstagioId),
                        "O estágio selecionado não existe.");
                }
            }

            if (!ModelState.IsValid)
            {
                EstagiosDisponiveis = await _estagioService
    .ConsultarEstagiosPermitidos()
    .AsNoTracking()
    .OrderByDescending(e => e.Id)
    .ToListAsync();

                return Page();
            }

            var dados = new
            {
                NomeAluno = NomeAluno.Trim(),
                DataAtividade,
                HorarioEntrada,
                HorarioSaida,
                AtividadesRealizadas = AtividadesRealizadas?.Trim()
            };

            var documento = new DocumentoPreenchido
            {
                Tipo = "Formulário de Frequência",
                DadosJson = JsonSerializer.Serialize(dados),
                DataCriacao = DateTime.UtcNow,
                UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                EstagioId = EstagioId
            };

            _context.DocumentosPreenchidos.Add(documento);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Formulário de Frequência salvo com sucesso!";

            return RedirectToPage("/Documentos/Index");
        }
    }
}
