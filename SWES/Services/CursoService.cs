using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using SWES.ViewModels;

namespace SWES.Services
{
    public class CursoService
    {
        private readonly ApplicationDbContext _context;

        public CursoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CursoViewModel>> ListarAsync(
            string? pesquisa,
            string? situacao)
        {
            var consulta = _context.Cursos
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                consulta = consulta.Where(c =>
                    c.Nome.Contains(pesquisa));
            }

            if (situacao == "Ativo")
            {
                consulta = consulta.Where(c => c.Ativo);
            }
            else if (situacao == "Inativo")
            {
                consulta = consulta.Where(c => !c.Ativo);
            }

            return await consulta
                .OrderBy(c => c.Nome)
                .Select(c => new CursoViewModel
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    QuantidadeSemestres = c.QuantidadeSemestres,
                    Periodo = c.Periodo,
                    InstituicaoNome = c.Instituicao.RazaoSocial,
                    QuantidadeProfessores = c.Professores.Count,
                    Ativo = c.Ativo
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>>
            ListarInstituicoesAsync()
        {
            var instituicoes = await _context.UnidadesConcedentes
                .AsNoTracking()
                .Where(i => i.Ativo && i.InstituicaoEnsino)
                .OrderBy(i => i.RazaoSocial)
                .ToListAsync();

            return instituicoes.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.RazaoSocial
            }).ToList();
        }

        public async Task<Curso?> ObterPorIdAsync(Guid id)
        {
            return await _context.Cursos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> CadastrarAsync(
            CursoCadastroViewModel dados)
        {
            bool instituicaoValida =
                await InstituicaoValidaAsync(dados.InstituicaoId);

            if (!instituicaoValida)
            {
                return false;
            }

            var curso = new Curso(
                dados.Nome.Trim(),
                dados.QuantidadeSemestres,
                dados.Periodo.Trim(),
                dados.InstituicaoId);

            _context.Cursos.Add(curso);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AtualizarAsync(
            Guid id,
            CursoCadastroViewModel dados)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null)
            {
                return false;
            }

            bool instituicaoValida =
                await InstituicaoValidaAsync(dados.InstituicaoId);

            if (!instituicaoValida)
            {
                return false;
            }

            curso.AtualizarDados(
                dados.Nome.Trim(),
                dados.QuantidadeSemestres,
                dados.Periodo.Trim(),
                dados.InstituicaoId);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AlterarSituacaoAsync(
            Guid id,
            bool ativar)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null)
            {
                return false;
            }

            if (ativar)
            {
                curso.Ativar();
            }
            else
            {
                curso.Desativar();
            }

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> InstituicaoValidaAsync(
            Guid instituicaoId)
        {
            return await _context.UnidadesConcedentes
                .AnyAsync(i =>
                    i.Id == instituicaoId &&
                    i.Ativo &&
                    i.InstituicaoEnsino);
        }
    }
}