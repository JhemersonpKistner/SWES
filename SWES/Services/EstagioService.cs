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

        // ==========================================
        // GERAR PROTOCOLO
        // ==========================================

        public string GerarProtocolo()
        {
            return $"SWES-{DateTime.UtcNow:yyyy}-{Guid.NewGuid():N}"
                .Substring(0, 30)
                .ToUpperInvariant();
        }

        public record DocumentoParaSalvar(
            string Nome,
            string NomeArquivo,
            string CaminhoArquivo);

        public IQueryable<Estagio> ConsultarEstagiosPermitidos()
        {
            return _context.Estagios;
        }

        // =====================================================
        // LISTAR ESTÁGIOS
        // =====================================================

        public async Task<List<EstagioViewModel>> ListarEstagiosAsync(
            string? pesquisa = null)
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
                    e.Protocolo.Contains(termo) ||
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
                Protocolo = e.Protocolo,
                Etapa = e.Etapa,
                Tipo = e.Tipo,
                DataInicio = e.DataInicio,
                DataFim = e.DataFim,
                Horario = e.Horario,
                Situacao = e.Situacao,

                ProfessorOrientador =
                    e.ProfessorOrientador?.Nome ?? "Não informado",

                Supervisor =
                    e.SupervisorEstagio?.Nome ?? "Não informado",

                UnidadeConcedente =
                    e.UnidadeConcedente?.RazaoSocial ?? "Não informada",

                InstituicaoEnsino =
                    e.InstituicaoEnsino?.RazaoSocial ?? "Não informada",

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
                .Include(e => e.Documentos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estagio == null)
                return null;

            return new EstagioCadastroViewModel
            {
                Id = estagio.Id,
                Protocolo = estagio.Protocolo,
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
                NomeArquivoEspecificacoesAtual =
                    estagio.NomeArquivoEspecificacoes,

                EditalArquivoAtual = estagio.EditalArquivo,
                NomeArquivoEditalAtual = estagio.NomeArquivoEdital,

                DocumentosExistentes = estagio.Documentos
                    .Select(d => new DocumentoExistenteViewModel
                    {
                        Id = d.Id,
                        Nome = d.Nome ?? "Documento sem nome",
                        NomeArquivo = d.NomeArquivo,
                        CaminhoArquivo = d.CaminhoArquivo
                    })
                    .ToList()
            };
        }

        // =====================================================
        // BUSCAR PROFESSORES POR INSTITUIÇÃO
        // =====================================================

        public async Task<List<OpcaoEstagioViewModel>>
            ObterProfessoresPorInstituicaoAsync(Guid instituicaoId)
        {
            return await _context.ProfessoresOrientadores
                .Where(p => p.Ativo &&
                    (p.InstituicaoEnsinoId == instituicaoId ||
                     p.InstituicoesEnsino.Any(i => i.Id == instituicaoId)))
                .OrderBy(p => p.Nome)
                .Select(p => new OpcaoEstagioViewModel
                {
                    Id = p.Id,
                    Nome = p.Nome
                })
                .ToListAsync();
        }

        // =====================================================
        // BUSCAR ALUNOS POR INSTITUIÇÃO
        // =====================================================

        public async Task<List<OpcaoEstagioViewModel>>
            ObterAlunosPorInstituicaoAsync(Guid instituicaoId)
        {
            return await _context.Alunos
                .Where(a => a.Ativo &&
                            a.InstituicaoEnsinoId == instituicaoId)
                .OrderBy(a => a.Nome)
                .Select(a => new OpcaoEstagioViewModel
                {
                    Id = a.Id,
                    Nome = a.Nome
                })
                .ToListAsync();
        }

        // =====================================================
        // BUSCAR COORDENADORES POR INSTITUIÇÃO
        // =====================================================

        public async Task<List<OpcaoEstagioViewModel>>
            ObterCoordenadoresPorInstituicaoAsync(Guid instituicaoId)
        {
            return await _context.Coordenadores
                .Where(c => c.Ativo &&
                            c.InstituicaoEnsinoId == instituicaoId)
                .OrderBy(c => c.Nome)
                .Select(c => new OpcaoEstagioViewModel
                {
                    Id = c.Id,
                    Nome = c.Nome
                })
                .ToListAsync();
        }

        // =====================================================
        // BUSCAR SUPERVISORES POR UNIDADE
        // =====================================================

        public async Task<List<OpcaoEstagioViewModel>>
            ObterSupervisoresPorUnidadeAsync(Guid unidadeConcedenteId)
        {
            return await _context.SupervisoresEstagio
                .Where(s => s.Ativo &&
                            s.UnidadeConcedenteId == unidadeConcedenteId)
                .OrderBy(s => s.Nome)
                .Select(s => new OpcaoEstagioViewModel
                {
                    Id = s.Id,
                    Nome = s.Nome
                })
                .ToListAsync();
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
            string? nomeArquivoEdital,
            List<DocumentoParaSalvar> documentos)
        {
            var professor = await _context.ProfessoresOrientadores
                .FirstOrDefaultAsync(p =>
                    p.Id == model.ProfessorOrientadorId &&
                    p.Ativo &&
                    (p.InstituicaoEnsinoId == model.InstituicaoEnsinoId ||
                     p.InstituicoesEnsino.Any(i =>
                         i.Id == model.InstituicaoEnsinoId)));

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

            if (professor == null)
            {
                throw new InvalidOperationException(
                    "Professor não encontrado ou não vinculado à instituição selecionada.");
            }

            if (supervisor == null)
            {
                throw new InvalidOperationException(
                    "Supervisor não encontrado ou não vinculado à unidade concedente selecionada.");
            }

            if (unidade == null)
            {
                throw new InvalidOperationException(
                    "Unidade concedente não encontrada ou inativa.");
            }

            if (instituicao == null)
            {
                throw new InvalidOperationException(
                    "Instituição de ensino não encontrada ou inativa.");
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

            estagio.DefinirProtocolo(model.Protocolo);

            var coordenador = model.CoordenadorId.HasValue
                ? await _context.Coordenadores.FirstOrDefaultAsync(c =>
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

            foreach (var documento in documentos)
            {
                estagio.Documentos.Add(new Documento(
                    "Anexo",
                    estagio.Id,
                    documento.Nome,
                    documento.NomeArquivo,
                    documento.CaminhoArquivo));
            }

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // ATUALIZAR ESTÁGIO
        // =====================================================

        public async Task<List<string>> AtualizarAsync(
            EstagioCadastroViewModel model,
            string? especificacoesArquivo,
            string? nomeArquivoEspecificacoes,
            string? editalArquivo,
            string? nomeArquivoEdital,
            List<DocumentoParaSalvar> documentos)
        {
            var estagio = await _context.Estagios
                .Include(e => e.Alunos)
                .Include(e => e.Documentos)
                .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (estagio == null)
                throw new InvalidOperationException("Estágio não encontrado.");

            var professor = await _context.ProfessoresOrientadores
                .FirstOrDefaultAsync(p =>
                    p.Id == model.ProfessorOrientadorId &&
                    p.Ativo &&
                    (p.InstituicaoEnsinoId == model.InstituicaoEnsinoId ||
                     p.InstituicoesEnsino.Any(i =>
                         i.Id == model.InstituicaoEnsinoId)));

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

            if (professor == null)
            {
                throw new InvalidOperationException(
                    "Professor não encontrado ou não vinculado à instituição selecionada.");
            }

            if (supervisor == null)
            {
                throw new InvalidOperationException(
                    "Supervisor não encontrado ou não vinculado à unidade concedente selecionada.");
            }

            if (unidade == null)
            {
                throw new InvalidOperationException(
                    "Unidade concedente não encontrada ou inativa.");
            }

            if (instituicao == null)
            {
                throw new InvalidOperationException(
                    "Instituição de ensino não encontrada ou inativa.");
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
                ? await _context.Coordenadores.FirstOrDefaultAsync(c =>
                    c.Id == model.CoordenadorId.Value &&
                    c.Ativo &&
                    c.InstituicaoEnsinoId == model.InstituicaoEnsinoId)
                : null;

            if (model.CoordenadorId.HasValue && coordenador == null)
            {
                throw new InvalidOperationException(
                    "O coordenador selecionado não foi encontrado.");
            }

            // Atualiza os relacionamentos do estágio.

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

            // Atualiza os alunos vinculados.

            estagio.Alunos.Clear();

            foreach (var aluno in alunos)
            {
                estagio.AdicionarAluno(aluno);
            }

            // Atualiza as informações principais.
            // Se não houver novo arquivo, mantém o arquivo atual.

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

            // =================================================
            // REMOVER DOCUMENTOS SELECIONADOS
            // =================================================

            var idsParaRemover = model.DocumentosRemoverIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var caminhosArquivosRemovidos = new List<string>();

            if (idsParaRemover.Any())
            {
                // Busca somente documentos pertencentes ao estágio atual.
                var documentosParaRemover = estagio.Documentos
                    .Where(d => idsParaRemover.Contains(d.Id))
                    .ToList();

                // Impede que IDs de documentos de outro estágio
                // sejam utilizados nesta operação.
                if (documentosParaRemover.Count != idsParaRemover.Count)
                {
                    throw new InvalidOperationException(
                        "Um ou mais documentos selecionados para exclusão não pertencem a este estágio ou não foram encontrados.");
                }

                foreach (var documento in documentosParaRemover)
                {
                    if (!string.IsNullOrWhiteSpace(documento.CaminhoArquivo))
                    {
                        caminhosArquivosRemovidos.Add(
                            documento.CaminhoArquivo);
                    }

                    estagio.Documentos.Remove(documento);

                    _context.Remove(documento);
                }
            }

            // =================================================
            // ADICIONAR NOVOS DOCUMENTOS
            // =================================================

            foreach (var documento in documentos)
            {
                estagio.Documentos.Add(new Documento(
                    "Anexo",
                    estagio.Id,
                    documento.Nome,
                    documento.NomeArquivo,
                    documento.CaminhoArquivo));
            }

            // Persiste todas as alterações.
            // Os caminhos dos arquivos removidos só serão
            // devolvidos após a conclusão bem-sucedida.

            await _context.SaveChangesAsync();

            return caminhosArquivosRemovidos;
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

        // =====================================================
        // INATIVAR ESTÁGIO
        // =====================================================

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