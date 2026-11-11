using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Models;
using SWES.ViewModels;

namespace SWES.Services
{
    public class EstagioService
    {
        private readonly ApplicationDbContext _context;

        public EstagioService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR ESTÁGIOS
        // =====================================================


        public async Task<List<EstagioViewModel>> ListarEstagiosAsync(string? pesquisa = null)
        {
            IQueryable<Estagio> consulta = _context.Estagios
                .Include(e => e.ProfessorOrientador)
                .Include(e => e.SupervisorEstagio)
                .Include(e => e.UnidadeConcedente)
                .Include(e => e.InstituicaoEnsino)
                .Include(e => e.Alunos);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                var termo = pesquisa.Trim();

                consulta = consulta.Where(e =>
                    e.Etapa.Contains(termo) ||
                    e.Tipo.Contains(termo) ||
                    e.Situacao.Contains(termo) ||
                    (e.ProfessorOrientador != null &&
                        e.ProfessorOrientador.Nome.Contains(termo)) ||
                    (e.SupervisorEstagio != null &&
                        e.SupervisorEstagio.Nome.Contains(termo)) ||
                    (e.UnidadeConcedente != null &&
                        e.UnidadeConcedente.RazaoSocial.Contains(termo)) ||
                    (e.InstituicaoEnsino != null &&
                        e.InstituicaoEnsino.RazaoSocial.Contains(termo)) ||
                    e.Alunos.Any(a => a.Nome.Contains(termo))
                );
            }

            var estagios = await consulta
                .OrderByDescending(e => e.DataInicio)
                .ToListAsync();

            return estagios.Select(e => new EstagioViewModel
            {
                Id = e.Id,
                Etapa = e.Etapa,
                Tipo = e.Tipo,
                DataInicio = e.DataInicio,
                DataFim = e.DataFim,
                Horario = e.Horario,
                Situacao = e.Situacao,

                ProfessorOrientador = e.ProfessorOrientador?.Nome ?? "Não informado",
                Supervisor = e.SupervisorEstagio?.Nome ?? "Não informado",

                UnidadeConcedente = e.UnidadeConcedente?.RazaoSocial ?? "Não informada",
                InstituicaoEnsino = e.InstituicaoEnsino?.RazaoSocial ?? "Não informada",

                Alunos = e.Alunos
                    .Select(a => a.Nome)
                    .ToList(),

                ApoliceSeguro = e.ApoliceSeguro,
                Seguradora = e.Seguradora
            }).ToList();
        }

        // =====================================================
        // BUSCAR ESTÁGIO POR ID
        // =====================================================

        public async Task<EstagioCadastroViewModel?> ObterPorIdAsync(int id)
        {
            var estagio = await _context.Estagios
                .Include(e => e.Alunos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estagio == null)
                return null;

            return new EstagioCadastroViewModel
            {
                Id = estagio.Id,
                Tipo = estagio.Tipo,
                Etapa = estagio.Etapa,
                DataInicio = estagio.DataInicio,
                DataFim = estagio.DataFim,
                Horario = estagio.Horario,

                ProfessorOrientadorId = estagio.ProfessorOrientadorId,
                SupervisorEstagioId = estagio.SupervisorEstagioId,
                UnidadeConcedenteId = estagio.UnidadeConcedenteId,
                InstituicaoEnsinoId = estagio.InstituicaoEnsinoId,
                CoordenadorId = estagio.CoordenadorId,

                AlunosIds = estagio.Alunos
                    .Select(a => a.Id)
                    .ToList(),

                Situacao = estagio.Situacao,
                Observacao = estagio.Observacao,
                ApoliceSeguro = estagio.ApoliceSeguro,
                Seguradora = estagio.Seguradora,

                EspecificacoesHtml = estagio.EspecificacoesHtml,
                EspecificacoesArquivoAtual = estagio.EspecificacoesArquivo,
                NomeArquivoEspecificacoesAtual = estagio.NomeArquivoEspecificacoes,
                EditalArquivoAtual = estagio.EditalArquivo,
                NomeArquivoEditalAtual = estagio.NomeArquivoEdital
            };
        }

        // =====================================================
        // CARREGAR OPÇÕES DOS CAMPOS DE SELEÇÃO
        // =====================================================

        public async Task<OpcoesEstagioViewModel> ObterOpcoesAsync()
        {
            var opcoes = new OpcoesEstagioViewModel();

            opcoes.Professores = await _context.ProfessoresOrientadores
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .Select(p => new OpcaoEstagioViewModel
                {
                    Id = p.Id,
                    Nome = p.Nome
                })
                .ToListAsync();

            opcoes.Supervisores = await _context.SupervisoresEstagio
                .Where(s => s.Ativo)
                .OrderBy(s => s.Nome)
                .Select(s => new OpcaoEstagioViewModel
                {
                    Id = s.Id,
                    Nome = s.Nome
                })
                .ToListAsync();

            opcoes.UnidadesConcedentes = await _context.UnidadesConcedentes
                .Where(u => u.Ativo && !u.InstituicaoEnsino)
                .OrderBy(u => u.RazaoSocial)
                .Select(u => new OpcaoEstagioViewModel
                {
                    Id = u.Id,
                    Nome = u.RazaoSocial
                })
                .ToListAsync();

            opcoes.InstituicoesEnsino = await _context.UnidadesConcedentes
                .Where(u => u.Ativo && u.InstituicaoEnsino)
                .OrderBy(u => u.RazaoSocial)
                .Select(u => new OpcaoEstagioViewModel
                {
                    Id = u.Id,
                    Nome = u.RazaoSocial
                })
                .ToListAsync();

            opcoes.Coordenadores = await _context.Coordenadores
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new OpcaoEstagioViewModel
                {
                    Id = c.Id,
                    Nome = c.Nome
                })
                .ToListAsync();

            opcoes.Alunos = await _context.Alunos
                .Where(a => a.Ativo)
                .OrderBy(a => a.Nome)
                .Select(a => new OpcaoEstagioViewModel
                {
                    Id = a.Id,
                    Nome = a.Nome
                })
                .ToListAsync();

            return opcoes;
        }

        // =====================================================
        // CADASTRAR ESTÁGIO
        // =====================================================

        public async Task CadastrarAsync(
            EstagioCadastroViewModel model,
            string? especificacoesArquivo,
            string? nomeArquivoEspecificacoes,
            string? editalArquivo,
            string? nomeArquivoEdital)
        {
            var professor = await _context.ProfessoresOrientadores
                .FirstOrDefaultAsync(p =>
                    p.Id == model.ProfessorOrientadorId &&
                    p.Ativo &&
                    (
                        p.InstituicaoEnsinoId == model.InstituicaoEnsinoId ||
                        p.InstituicoesEnsino.Any(i =>
                            i.Id == model.InstituicaoEnsinoId)
                    ));

            var supervisor = await _context.SupervisoresEstagio
                .FirstOrDefaultAsync(s =>
                    s.Id == model.SupervisorEstagioId &&
                    s.Ativo &&
                    s.UnidadeConcedenteId == model.UnidadeConcedenteId);

            var unidade = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(u =>
                    u.Id == model.UnidadeConcedenteId &&
                    u.Ativo &&
                    !u.InstituicaoEnsino);

            var instituicao = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(u =>
                    u.Id == model.InstituicaoEnsinoId &&
                    u.Ativo &&
                    u.InstituicaoEnsino);

            if (professor == null || supervisor == null ||
                unidade == null || instituicao == null)
            {
                throw new InvalidOperationException(
                    "Um ou mais vínculos selecionados não foram encontrados.");
            }

            var alunos = await _context.Alunos
                .Where(a =>
                    model.AlunosIds.Contains(a.Id) &&
                        a.Ativo &&
                        a.InstituicaoEnsinoId == model.InstituicaoEnsinoId)
                .ToListAsync();

            if (alunos.Count != model.AlunosIds.Distinct().Count())
            {
                throw new InvalidOperationException(
                    "Um ou mais alunos selecionados não foram encontrados, estão inativos ou não pertencem à instituição de ensino selecionada.");
            }

            var estagio = new Estagio(
                model.Tipo.Trim(),
                model.Etapa,
                model.DataInicio!.Value,
                model.DataFim!.Value,
                model.Horario.Trim(),
                professor.Id,
                supervisor.Id,
                unidade.Id,
                instituicao.Id
            );

            var coordenador = model.CoordenadorId.HasValue
                ? await _context.Coordenadores.FirstOrDefaultAsync(
                    c =>
                    c.Id == model.CoordenadorId.Value &&
                    c.Ativo &&
                    c.InstituicaoEnsinoId == model.InstituicaoEnsinoId)
                : null;

            if (model.CoordenadorId.HasValue && coordenador == null)
            {
                throw new InvalidOperationException(
                    "O coordenador selecionado não foi encontrado.");
            }

            _context.Entry(estagio)
                .Reference(e => e.Coordenador)
                .CurrentValue = coordenador;

            estagio.AtualizarInformacoes(
                model.Tipo.Trim(),
                model.Etapa,
                model.DataInicio.Value,
                model.DataFim.Value,
                model.Horario.Trim(),
                model.Situacao,
                model.Observacao?.Trim() ?? string.Empty,
                model.ApoliceSeguro.Trim(),
                model.Seguradora.Trim(),
                model.EspecificacoesHtml,
                especificacoesArquivo,
                nomeArquivoEspecificacoes,
                editalArquivo,
                nomeArquivoEdital
            );

            foreach (var aluno in alunos)
            {
                estagio.AdicionarAluno(aluno);
            }

            _context.Estagios.Add(estagio);

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // ATUALIZAR ESTÁGIO
        // =====================================================

        public async Task AtualizarAsync(
            EstagioCadastroViewModel model,
            string? especificacoesArquivo,
            string? nomeArquivoEspecificacoes,
            string? editalArquivo,
            string? nomeArquivoEdital)
        {
            var estagio = await _context.Estagios
                .Include(e => e.Alunos)
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (estagio == null)
                throw new InvalidOperationException("Estágio não encontrado.");

            var professor = await _context.ProfessoresOrientadores
                .FirstOrDefaultAsync(p =>
                    p.Id == model.ProfessorOrientadorId &&
                    p.Ativo &&
                    (
                         p.InstituicaoEnsinoId == model.InstituicaoEnsinoId ||
                        p.InstituicoesEnsino.Any(i =>
                            i.Id == model.InstituicaoEnsinoId)
                    ));

            var supervisor = await _context.SupervisoresEstagio
                .FirstOrDefaultAsync(s =>
                    s.Id == model.SupervisorEstagioId &&
                    s.Ativo &&
                    s.UnidadeConcedenteId == model.UnidadeConcedenteId);

            var unidade = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(u =>
                    u.Id == model.UnidadeConcedenteId &&
                    u.Ativo &&
                    !u.InstituicaoEnsino);

            var instituicao = await _context.UnidadesConcedentes
                .FirstOrDefaultAsync(u =>
                    u.Id == model.InstituicaoEnsinoId &&
                    u.Ativo &&
                    u.InstituicaoEnsino);

            if (professor == null || supervisor == null ||
                unidade == null || instituicao == null)
            {
                throw new InvalidOperationException(
                    "Um ou mais vínculos selecionados não foram encontrados.");
            }

            var alunos = await _context.Alunos
                .Where(a =>
                    model.AlunosIds.Contains(a.Id) &&
                    a.Ativo &&
                    a.InstituicaoEnsinoId == model.InstituicaoEnsinoId)
                .ToListAsync();

            if (alunos.Count != model.AlunosIds.Distinct().Count())
            {
                throw new InvalidOperationException(
                    "Um ou mais alunos selecionados não foram encontrados, estão inativos ou não pertencem à instituição de ensino selecionada.");
            }

            var coordenador = model.CoordenadorId.HasValue
                ? await _context.Coordenadores.FirstOrDefaultAsync(
                    c =>
                    c.Id == model.CoordenadorId.Value &&
                    c.Ativo &&
                    c.InstituicaoEnsinoId == model.InstituicaoEnsinoId)
                : null;

            if (model.CoordenadorId.HasValue && coordenador == null)
            {
                throw new InvalidOperationException(
                    "O coordenador selecionado não foi encontrado.");
            }

            _context.Entry(estagio)
                .Reference(e => e.ProfessorOrientador)
                .CurrentValue = professor;

            _context.Entry(estagio)
                .Reference(e => e.SupervisorEstagio)
                .CurrentValue = supervisor;

            _context.Entry(estagio)
                .Reference(e => e.UnidadeConcedente)
                .CurrentValue = unidade;

            _context.Entry(estagio)
                .Reference(e => e.InstituicaoEnsino)
                .CurrentValue = instituicao;

            _context.Entry(estagio)
                .Reference(e => e.Coordenador)
                .CurrentValue = coordenador;

            estagio.Alunos.Clear();

            foreach (var aluno in alunos)
            {
                estagio.AdicionarAluno(aluno);
            }

            estagio.AtualizarInformacoes(
                model.Tipo.Trim(),
                model.Etapa,
                model.DataInicio!.Value,
                model.DataFim!.Value,
                model.Horario.Trim(),
                model.Situacao,
                model.Observacao?.Trim() ?? string.Empty,
                model.ApoliceSeguro.Trim(),
                model.Seguradora.Trim(),
                model.EspecificacoesHtml,
                especificacoesArquivo ?? model.EspecificacoesArquivoAtual,
                nomeArquivoEspecificacoes ?? model.NomeArquivoEspecificacoesAtual,
                editalArquivo ?? model.EditalArquivoAtual,
                nomeArquivoEdital ?? model.NomeArquivoEditalAtual
            );

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // EXCLUIR ESTÁGIO
        // =====================================================

        public async Task<bool> ExcluirAsync(int id)
        {
            var estagio = await _context.Estagios
                .Include(e => e.Documentos)
                .Include(e => e.Atividades)
                .Include(e => e.Avaliacoes)
                .Include(e => e.Alunos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estagio == null)
                return false;

            if (estagio.Documentos.Any() ||
                estagio.Atividades.Any() ||
                estagio.Avaliacoes.Any())
            {
                throw new InvalidOperationException(
                    "Não é possível excluir este estágio porque existem documentos, atividades ou avaliações vinculados a ele.");
            }

            _context.Estagios.Remove(estagio);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> InativarAsync(int id)
        {
            var estagio = await _context.Estagios
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estagio == null)
                return false;

            estagio.AlterarSituacao("Cancelado");

            await _context.SaveChangesAsync();

            return true;
        }
    }
}