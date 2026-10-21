using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using SWES.ViewModels;

namespace SWES.Pages.InstituicoesEnsino
{
    public class CadastroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CadastroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InstituicaoEnsinoViewModel Instituicao { get; set; }
            = new();

        public string? Erro { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!id.HasValue)
                return Page();

            var existente = await _context.InstituicoesEnsino
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id.Value);

            if (existente == null)
                return NotFound();

            Instituicao = new InstituicaoEnsinoViewModel
            {
                Id = existente.Id,
                Nome = existente.Nome,
                Cnpj = existente.Cnpj
            };

            return Page();
        }

        public async Task<IActionResult> OnPostSalvarAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var cnpj = new string(
                Instituicao.Cnpj.Where(char.IsDigit).ToArray());

            if (cnpj.Length != 14)
            {
                ModelState.AddModelError(
                    "Instituicao.Cnpj",
                    "O CNPJ deve possuir 14 números.");

                return Page();
            }

            var cnpjEmUso = await _context.InstituicoesEnsino
                .AnyAsync(i =>
                    i.Cnpj == cnpj &&
                    i.Id != Instituicao.Id);

            if (cnpjEmUso)
            {
                ModelState.AddModelError(
                    "Instituicao.Cnpj",
                    "Já existe uma instituição cadastrada com esse CNPJ.");

                return Page();
            }

            if (Instituicao.Id.HasValue)
            {
                var existente = await _context.InstituicoesEnsino
                    .FirstOrDefaultAsync(i =>
                        i.Id == Instituicao.Id.Value);

                if (existente == null)
                    return NotFound();

                existente.AtualizarDados(
                    Instituicao.Nome.Trim(),
                    cnpj);
            }
            else
            {
                var novaInstituicao = new InstituicaoEnsino(
                    Instituicao.Nome.Trim(),
                    cnpj);

                _context.InstituicoesEnsino.Add(novaInstituicao);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/InstituicoesEnsino/Index");
        }
    }
}
