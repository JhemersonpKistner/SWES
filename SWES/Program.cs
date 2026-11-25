using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SWES.Authorization;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Services;

namespace SWES
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    // O projeto acadêmico não possui fluxo de confirmação de e-mail.
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/AcessoNegado";
                options.SlidingExpiration = true;
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(PerfisSistema.Administrador));

                options.AddPolicy("Gestores", policy =>
                    policy.RequireRole(PerfisSistema.Administrador, PerfisSistema.Coordenador));

                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToPage("/Index");
                options.Conventions.AllowAnonymousToPage("/Login");
                options.Conventions.AllowAnonymousToPage("/AcessoNegado");
                options.Conventions.AllowAnonymousToPage("/Error");
                options.Conventions.AuthorizeFolder("/Usuarios", "AdminOnly");
                options.Conventions.AuthorizeFolder("/Cursos", "Gestores");
                options.Conventions.AuthorizeFolder("/InstituicoesEnsino", "Gestores");
                options.Conventions.AuthorizeFolder("/UnidadesConcedentes", "Gestores");
                options.Conventions.AuthorizeFolder("/Desenvolvimento", "AdminOnly");

            });
            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<ExportacaoService>();
            builder.Services.AddScoped<DadosTesteService>();
            builder.Services.AddScoped<CursoService>();
            builder.Services.AddScoped<EstagioService>();
            builder.Services.AddScoped<IdentitySeedService>();

            var app = builder.Build();

            // Garante que os perfis existam e associa usuários já cadastrados.
            using (var scope = app.Services.CreateScope())
            {
                var inicializador = scope.ServiceProvider.GetRequiredService<IdentitySeedService>();
                await inicializador.InicializarAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Serve os arquivos estáticos físicos de wwwroot (CSS, JavaScript, imagens e bibliotecas).
            // Mantém MapStaticAssets abaixo para os assets otimizados do Razor Pages.
            app.UseStaticFiles();

            app.UseRouting();

            // Mantém a ordem correta do pipeline para autenticação/autorização do Identity.
            app.UseAuthentication();
            app.UseAuthorization();

            // Os arquivos estáticos (CSS, JS, imagens e assets fingerprinted)
            // precisam ser acessíveis antes do login para renderizar a Home e o Login.
            app.MapStaticAssets().AllowAnonymous();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
