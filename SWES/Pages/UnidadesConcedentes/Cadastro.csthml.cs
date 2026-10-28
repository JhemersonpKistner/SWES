
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using SWES.ViewModels;

namespace SWES.Pages.UnidadesConcedentes
{
    public class CadastroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CadastroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UnidadeConcedenteViewModel UnidadeConcedente { get; set; }
            = new();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!id.HasValue)
                return Page();

            var existente = await _context.UnidadesConcedentes
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id.Value);

            if (existente == null)
                return NotFound();

            UnidadeConcedente = new UnidadeConcedenteViewModel
            {
                Id = existente.Id,
                RazaoSocial = existente.RazaoSocial,
                Cnpj = existente.Cnpj,
                Endereco = existente.Endereco,
                Telefone = existente.Telefone,
                Email = existente.Email,
                InstituicaoEnsino = existente.InstituicaoEnsino
            };

            return Page();
        }

        public async Task<IActionResult> OnPostSalvarAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var cnpj = new string(
                UnidadeConcedente.Cnpj
                    .Where(char.IsDigit)
                    .ToArray());

            if (cnpj.Length != 14)
            {
                ModelState.AddModelError(
                    "UnidadeConcedente.Cnpj",
                    "O CNPJ deve possuir 14 números.");

                return Page();
            }

            var cnpjEmUso = await _context.UnidadesConcedentes
                .AnyAsync(u =>
                    u.Cnpj == cnpj &&
                    (!UnidadeConcedente.Id.HasValue ||
                     u.Id != UnidadeConcedente.Id.Value));

            if (cnpjEmUso)
            {
                ModelState.AddModelError(
                    "UnidadeConcedente.Cnpj",
                    "Já existe um concedente cadastrado com esse CNPJ.");

                return Page();
            }

            var razaoSocial = UnidadeConcedente.RazaoSocial.Trim();
            var endereco = UnidadeConcedente.Endereco.Trim();
            var telefone = UnidadeConcedente.Telefone.Trim();
            var email = UnidadeConcedente.Email.Trim();

            if (UnidadeConcedente.Id.HasValue)
            {
                var existente = await _context.UnidadesConcedentes
                    .FirstOrDefaultAsync(u =>
                        u.Id == UnidadeConcedente.Id.Value);

                if (existente == null)
                    return NotFound();

                existente.AtualizarDados(
                    razaoSocial,
                    cnpj,
                    endereco,
                    telefone,
                    email,
                    UnidadeConcedente.InstituicaoEnsino);
            }
            else
            {
                var novaUnidade = new UnidadeConcedente(
                    razaoSocial,
                    cnpj,
                    endereco,
                    telefone,
                    email,
                    UnidadeConcedente.InstituicaoEnsino);

                _context.UnidadesConcedentes.Add(novaUnidade);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/UnidadesConcedentes/Index");
        }
    }
}
