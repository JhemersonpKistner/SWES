
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SWES.Models;
using System.Collections.Generic;

namespace SWES.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UnidadeConcedente> UnidadesConcedentes { get; set; }
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

            builder.Entity<Aluno>()
                .HasOne(a => a.InstituicaoEnsino)
                .WithMany()
                .HasForeignKey(a => a.InstituicaoEnsinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProfessorOrientador>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProfessorOrientador>()
                .HasOne(p => p.InstituicaoEnsino)
                .WithMany()
                .HasForeignKey(p => p.InstituicaoEnsinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupervisorEstagio>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupervisorEstagio>()
                .HasOne(s => s.InstituicaoEnsino)
                .WithMany()
                .HasForeignKey(s => s.InstituicaoEnsinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Coordenador>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Coordenador>()
                .HasOne(c => c.InstituicaoEnsino)
                .WithMany()
                .HasForeignKey(c => c.InstituicaoEnsinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProfessorOrientador>()
                .HasMany(p => p.Cursos)
                .WithMany(c => c.Professores);

            builder.Entity<ProfessorOrientador>()
                .HasMany(p => p.InstituicoesEnsino)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ProfessorInstituicoesEnsino",
                    right => right
                        .HasOne<UnidadeConcedente>()
                        .WithMany()
                        .HasForeignKey("InstituicaoEnsinoId")
                        .OnDelete(DeleteBehavior.Restrict),
                    left => left
                        .HasOne<ProfessorOrientador>()
                        .WithMany()
                        .HasForeignKey("ProfessorOrientadorId")
                        .OnDelete(DeleteBehavior.Restrict),
                    join =>
                    {
                        join.ToTable("ProfessorInstituicoesEnsino");
                        join.HasKey("ProfessorOrientadorId", "InstituicaoEnsinoId");
                    });

            builder.Entity<Curso>()
                .HasOne(c => c.Instituicao)
                .WithMany()
                .HasForeignKey(c => c.InstituicaoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Aluno>()
                .HasOne(a => a.Curso)
                .WithMany()
                .HasForeignKey(a => a.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupervisorEstagio>()
                .HasOne(s => s.UnidadeConcedente)
                .WithMany()
                .HasForeignKey(s => s.UnidadeConcedenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.ProfessorOrientador)
                .WithMany()
                .HasForeignKey(e => e.ProfessorOrientadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.SupervisorEstagio)
                .WithMany()
                .HasForeignKey(e => e.SupervisorEstagioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.Coordenador)
                .WithMany()
                .HasForeignKey(e => e.CoordenadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.UnidadeConcedente)
                .WithMany(u => u.Estagios)
                .HasForeignKey(e => e.UnidadeConcedenteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasOne(e => e.InstituicaoEnsino)
                .WithMany(u => u.EstagiosComoInstituicao)
                .HasForeignKey(e => e.InstituicaoEnsinoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Estagio>()
                .HasMany(e => e.Alunos)
                .WithMany(a => a.Estagios)
                .UsingEntity<Dictionary<string, object>>(
                    "EstagioAlunos",
                    right => right
                        .HasOne<Aluno>()
                        .WithMany()
                        .HasForeignKey("AlunoId")
                        .OnDelete(DeleteBehavior.Restrict),
                    left => left
                        .HasOne<Estagio>()
                        .WithMany()
                        .HasForeignKey("EstagioId")
                        .OnDelete(DeleteBehavior.Restrict),
                    join =>
                    {
                        join.ToTable("EstagioAlunos");
                        join.HasKey("EstagioId", "AlunoId");
                    });
        }
    }
}