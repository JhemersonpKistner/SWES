using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SWES.Authorization;
using SWES.Data;

namespace SWES.Services
{
    
    /// Cria os perfis do sistema, sincroniza perfis de usuários já cadastrados
    /// e, quando configurado por variáveis de ambiente, cria a conta inicial
    /// de administrador.
    
    public class IdentitySeedService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IdentitySeedService> _logger;

        public IdentitySeedService(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            ILogger<IdentitySeedService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task InicializarAsync()
        {
            string[] perfis =
            {
                PerfisSistema.Administrador,
                PerfisSistema.Aluno,
                PerfisSistema.Professor,
                PerfisSistema.Supervisor,
                PerfisSistema.Coordenador
            };

            foreach (var perfil in perfis)
            {
                if (!await _roleManager.RoleExistsAsync(perfil))
                {
                    var resultado = await _roleManager.CreateAsync(new IdentityRole(perfil));
                    if (!resultado.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Não foi possível criar o perfil {perfil}: " +
                            string.Join("; ", resultado.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Migração lógica dos usuários que já existiam antes da gestão de perfis.
            var vínculos = new List<(string UserId, string Role)>();
            vínculos.AddRange(await _context.Alunos.AsNoTracking()
                .Select(x => new ValueTuple<string, string>(x.UserId, PerfisSistema.Aluno)).ToListAsync());
            vínculos.AddRange(await _context.ProfessoresOrientadores.AsNoTracking()
                .Select(x => new ValueTuple<string, string>(x.UserId, PerfisSistema.Professor)).ToListAsync());
            vínculos.AddRange(await _context.SupervisoresEstagio.AsNoTracking()
                .Select(x => new ValueTuple<string, string>(x.UserId, PerfisSistema.Supervisor)).ToListAsync());
            vínculos.AddRange(await _context.Coordenadores.AsNoTracking()
                .Select(x => new ValueTuple<string, string>(x.UserId, PerfisSistema.Coordenador)).ToListAsync());

            foreach (var vínculo in vínculos)
            {
                var user = await _userManager.FindByIdAsync(vínculo.UserId);
                if (user == null)
                    continue;

                var rolesAtuais = await _userManager.GetRolesAsync(user);
                var rolesDePerfil = rolesAtuais.Where(r => r != PerfisSistema.Administrador).ToList();

                if (rolesDePerfil.Any(r => r != vínculo.Role))
                {
                    var remover = await _userManager.RemoveFromRolesAsync(user, rolesDePerfil);
                    if (!remover.Succeeded)
                        throw new InvalidOperationException("Falha ao corrigir os perfis de um usuário existente.");
                }

                if (!await _userManager.IsInRoleAsync(user, vínculo.Role))
                {
                    var adicionar = await _userManager.AddToRoleAsync(user, vínculo.Role);
                    if (!adicionar.Succeeded)
                        throw new InvalidOperationException("Falha ao atribuir perfil a um usuário existente.");
                }
            }

            await CriarAdministradorInicialAsync();
        }

        private async Task CriarAdministradorInicialAsync()
        {
            var email = Environment.GetEnvironmentVariable("SWES_ADMIN_EMAIL");
            var senha = Environment.GetEnvironmentVariable("SWES_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                _logger.LogWarning(
                    "Conta inicial de administrador não configurada. Defina SWES_ADMIN_EMAIL e SWES_ADMIN_PASSWORD.");
                return;
            }

            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                usuario = new IdentityUser
                {
                    UserName = email.Trim(),
                    Email = email.Trim(),
                    EmailConfirmed = true
                };

                var criado = await _userManager.CreateAsync(usuario, senha);
                if (!criado.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Não foi possível criar o administrador inicial: " +
                        string.Join("; ", criado.Errors.Select(e => e.Description)));
                }
            }

            if (!await _userManager.IsInRoleAsync(usuario, PerfisSistema.Administrador))
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, PerfisSistema.Administrador);
                if (!resultado.Succeeded)
                    throw new InvalidOperationException("Não foi possível atribuir o perfil de administrador.");
            }
        }
    }
}
