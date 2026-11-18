using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SWES.Data;
using SWES.Models;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SWES.Services;

namespace SWES.Pages.Documentos
{
    [AllowAnonymous]
    public class TermoCompromissoModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly EstagioService _estagioService;

        public TermoCompromissoModel(
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
        [Required(ErrorMessage = "Informe a instituição de ensino.")]
        public string InstituicaoEnsino { get; set; } = string.Empty;

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
        public string? Atividades { get; set; }

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
                    .ConsultarEstagiosPermitidos()
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
                    .ConsultarEstagiosPermitidos()
                    .AsNoTracking()
                    .OrderByDescending(e => e.Id)
                    .ToListAsync();

                return Page();
            }

            var dados = new
            {
                NomeAluno = NomeAluno.Trim(),
                InstituicaoEnsino = InstituicaoEnsino.Trim(),
                UnidadeConcedente = UnidadeConcedente.Trim(),
                DataInicio,
                DataFim,
                Atividades = Atividades?.Trim()
            };


            var documento = new DocumentoPreenchido
            {
                Tipo = "Termo de Compromisso",
                DadosJson = JsonSerializer.Serialize(dados),
                DataCriacao = DateTime.UtcNow,
                UsuarioId = null,
                EstagioId = EstagioId
            };


            _context.DocumentosPreenchidos.Add(documento);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Termo de Compromisso salvo com sucesso!";

            return RedirectToPage("/Documentos/Index");
        }

    }
}
