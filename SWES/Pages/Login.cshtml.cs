using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWES.Authorization;
using SWES.Data;
using SWES.Authorization;

namespace SWES.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [BindProperty]
        public string? ReturnUrl { get; set; }

        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return LocalRedirect("/");

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
                return LocalRedirect("/");

            if (!ModelState.IsValid)
                return Page();

            var email = Email.Trim();
            var usuario = await _userManager.FindByEmailAsync(email);

            // Mensagem genérica para não revelar se uma conta existe.
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return Page();
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario.UserName ?? email,
                Senha,
                isPersistent: false,
                lockoutOnFailure: true);

            if (resultado.Succeeded)
            {
                var temPerfil = await _context.Alunos.AnyAsync(x => x.UserId == usuario.Id)
                    || await _context.ProfessoresOrientadores.AnyAsync(x => x.UserId == usuario.Id)
                    || await _context.SupervisoresEstagio.AnyAsync(x => x.UserId == usuario.Id)
                    || await _context.Coordenadores.AnyAsync(x => x.UserId == usuario.Id);

                var administrador = await _userManager.IsInRoleAsync(
                    usuario, PerfisSistema.Administrador);

                if (!temPerfil && !administrador)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty,
                        "Esta conta não possui um perfil válido no sistema.");
                    return Page();
                }

                if (temPerfil)
                {
                    var ativo = await _context.Alunos.AnyAsync(x => x.UserId == usuario.Id && x.Ativo)
                        || await _context.ProfessoresOrientadores.AnyAsync(x => x.UserId == usuario.Id && x.Ativo)
                        || await _context.SupervisoresEstagio.AnyAsync(x => x.UserId == usuario.Id && x.Ativo)
                        || await _context.Coordenadores.AnyAsync(x => x.UserId == usuario.Id && x.Ativo);

                    if (!ativo)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty,
                            "Esta conta está inativa. Entre em contato com a administração.");
                        return Page();
                    }
                }

                if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return LocalRedirect(ReturnUrl);

                return RedirectToPage("/Index");
            }

            if (resultado.IsLockedOut)
                ModelState.AddModelError(string.Empty,
                    "A conta foi temporariamente bloqueada após várias tentativas.");
            else
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");

            return Page();
        }
    }
}
