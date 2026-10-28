using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SWES.Models;

namespace SWES.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UnidadeConcedente> UnidadesConcedentes { get; set; }
        public DbSet<InstituicaoEnsino> InstituicoesEnsino { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Estagio> Estagios { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<ProfessorOrientador> ProfessoresOrientadores { get; set; }
        public DbSet<SupervisorEstagio> SupervisoresEstagio { get; set; }
        public DbSet<Coordenador> Coordenadores { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Curso> Cursos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Avaliacao>()
                .Property(a => a.Nota)
                .HasPrecision(4, 2);

            builder.Entity<Avaliacao>()
                .HasOne(a => a.ProfessorOrientador)
                .WithMany()
                .HasForeignKey(a => a.ProfessorOrientadorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Aluno>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProfessorOrientador>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupervisorEstagio>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Coordenador>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProfessorOrientador>()
                .HasMany(p => p.Cursos)
                .WithMany(c => c.Professores);

            builder.Entity<Curso>()
                .HasOne(c => c.Instituicao)
                .WithMany()
                .HasForeignKey(c => c.InstituicaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.UnidadeConcedente)
                .WithMany(u => u.Estagios)
                .HasForeignKey(e => e.UnidadeConcedenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.InstituicaoEnsino)
                .WithMany()
                .HasForeignKey(e => e.InstituicaoEnsinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
