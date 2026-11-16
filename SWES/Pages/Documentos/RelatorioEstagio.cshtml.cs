using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWES.Data;
using SWES.Models;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SWES.Services;

namespace SWES.Pages.Documentos
{
    [Authorize]
    public class RelatorioEstagioModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly EstagioService _estagioService;

        public RelatorioEstagioModel(
            ApplicationDbContext context,
            EstagioService estagioService)
        {
            _context = context;
            _estagioService = estagioService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Informe o nome do aluno.")]
        public string NomeAluno { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Informe a unidade concedente.")]
        public string UnidadeConcedente { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Informe a data de início.")]
        public DateTime? DataInicio { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Informe a data de término.")]
        public DateTime? DataFim { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Descreva as atividades desenvolvidas.")]
        public string AtividadesDesenvolvidas { get; set; } = string.Empty;

        [BindProperty]
        public string? ConsideracoesFinais { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Selecione o estágio.")]
        public int? EstagioId { get; set; }

        public List<Estagio> EstagiosDisponiveis { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
                return Challenge();

            EstagiosDisponiveis = await _estagioService
                .ConsultarEstagiosPermitidos(usuarioId)
                .AsNoTracking()
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
                return Challenge();

            if (DataInicio.HasValue &&
                DataFim.HasValue &&
                DataFim.Value.Date < DataInicio.Value.Date)
            {
                ModelState.AddModelError(
                    nameof(DataFim),
                    "A data de término não pode ser anterior à data de início.");
            }

            if (EstagioId.HasValue)
            {
                var estagioPermitido = await _estagioService
                    .ConsultarEstagiosPermitidos(usuarioId)
                    .AnyAsync(e => e.Id == EstagioId.Value);

                if (!estagioPermitido)
                {
                    ModelState.AddModelError(
                        nameof(EstagioId),
                        "O estágio selecionado não existe ou você não possui permissão para acessá-lo.");
                }
            }

            if (!ModelState.IsValid)
            {
                EstagiosDisponiveis = await _estagioService
                    .ConsultarEstagiosPermitidos(usuarioId)
                    .AsNoTracking()
                    .OrderByDescending(e => e.Id)
                    .ToListAsync();

                return Page();
            }

            var dados = new
            {
                NomeAluno = NomeAluno.Trim(),
                UnidadeConcedente = UnidadeConcedente.Trim(),
                DataInicio,
                DataFim,
                AtividadesDesenvolvidas = AtividadesDesenvolvidas.Trim(),
                ConsideracoesFinais = ConsideracoesFinais?.Trim()
            };

            var documento = new DocumentoPreenchido
            {
                Tipo = "Relatório de Estágio",
                DadosJson = JsonSerializer.Serialize(dados),
                DataCriacao = DateTime.UtcNow,
                UsuarioId = usuarioId,
                EstagioId = EstagioId
            };

            _context.DocumentosPreenchidos.Add(documento);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Relatório de Estágio salvo com sucesso!";

            return RedirectToPage("/Documentos/Index");
        }

    }
}
