using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SWES.Data;
using SWES.Authorization;
using SWES.Models;
using SWES.ViewModels;

namespace SWES.Services
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuarioService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // OPÇÕES DOS SELECTS DO CADASTRO (cursos e unidades/instituições)

        public OpcoesCadastroViewModel ObterOpcoesCadastro()
        {
            var cursos = _context.Cursos
                .AsNoTracking()
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new OpcaoViewModel
                {
                    Id = c.Id,
                    Nome = c.Nome + " - " + c.Instituicao.RazaoSocial,
                    InstituicaoId = c.InstituicaoId,
                    Periodo = c.Periodo
                })
                .ToList();

            var unidades = _context.UnidadesConcedentes
                .AsNoTracking()
                .Where(u => u.Ativo)
                .OrderBy(u => u.RazaoSocial)
                .Select(u => new OpcaoViewModel
                {
                    Id = u.Id,
                    Nome = u.InstituicaoEnsino
                        ? u.RazaoSocial + " (Instituição de ensino)"
                        : u.RazaoSocial
                })
                .ToList();

            return new OpcoesCadastroViewModel
            {
                Cursos = cursos,
                UnidadesConcedentes = unidades
            };
        }

        private async Task<List<Curso>?> ResolverCursosAsync(List<Guid>? ids)
        {
            var distintos = (ids ?? new List<Guid>()).Distinct().ToList();

            if (distintos.Count == 0)
                return null;

            var cursos = await _context.Cursos
                .Where(c => distintos.Contains(c.Id) && c.Ativo)
                .ToListAsync();

            return cursos.Count == distintos.Count ? cursos : null;
        }

        private static string NormalizarDocumento(string? valor)
        {
            return new string((valor ?? string.Empty)
                .Where(char.IsLetterOrDigit)
                .Select(char.ToUpperInvariant)
                .ToArray());
        }

        public async Task<string?> ValidarDocumentosDuplicadosAsync(
            string? cpf, string? rg, Guid? ignorarId = null)
        {
            var cpfNormalizado = NormalizarDocumento(cpf);
            var rgNormalizado = NormalizarDocumento(rg);

            var registros = await _context.Alunos.AsNoTracking()
                .Select(u => new { u.Id, u.CPF, u.RG }).ToListAsync();
            registros.AddRange(await _context.Coordenadores.AsNoTracking()
                .Select(u => new { u.Id, u.CPF, u.RG }).ToListAsync());
            registros.AddRange(await _context.ProfessoresOrientadores.AsNoTracking()
                .Select(u => new { u.Id, u.CPF, u.RG }).ToListAsync());
            registros.AddRange(await _context.SupervisoresEstagio.AsNoTracking()
                .Select(u => new { u.Id, u.CPF, u.RG }).ToListAsync());

            var existentes = registros.Where(u => !ignorarId.HasValue || u.Id != ignorarId.Value);

            if (!string.IsNullOrWhiteSpace(cpfNormalizado) &&
                existentes.Any(u => NormalizarDocumento(u.CPF) == cpfNormalizado))
                return "Já existe um usuário cadastrado com este CPF.";

            if (!string.IsNullOrWhiteSpace(rgNormalizado) &&
                existentes.Any(u => NormalizarDocumento(u.RG) == rgNormalizado))
                return "Já existe um usuário cadastrado com este RG.";

            return null;
        }

        private static IdentityResult Falha(string codigo, string mensagem)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = codigo,
                Description = mensagem
            });
        }

        // DADOS PESSOAIS (RG, endereço etc.)

        private static UsuarioViewModel ComDadosPessoais(UsuarioViewModel vm, Usuario u)
        {
            vm.RG = u.RG;
            vm.OrgaoExpedidor = u.OrgaoExpedidor;
            vm.EstadoCivil = u.EstadoCivil;
            vm.Nacionalidade = u.Nacionalidade;
            vm.Endereco = u.Endereco;
            vm.Bairro = u.Bairro;
            vm.Cidade = u.Cidade;
            vm.Estado = u.Estado;
            vm.CEP = u.CEP;
            vm.TelefoneSecundario = u.TelefoneSecundario;
            return vm;
        }

        private static UsuarioCadastroViewModel ComDadosPessoais(UsuarioCadastroViewModel vm, Usuario u)
        {
            vm.RG = u.RG ?? string.Empty;
            vm.OrgaoExpedidor = u.OrgaoExpedidor ?? string.Empty;
            vm.EstadoCivil = u.EstadoCivil ?? string.Empty;
            vm.Nacionalidade = u.Nacionalidade ?? string.Empty;
            vm.Endereco = u.Endereco ?? string.Empty;
            vm.Bairro = u.Bairro ?? string.Empty;
            vm.Cidade = u.Cidade ?? string.Empty;
            vm.Estado = u.Estado ?? string.Empty;
            vm.CEP = u.CEP ?? string.Empty;
            vm.TelefoneSecundario = u.TelefoneSecundario;
            return vm;
        }

        private static void AplicarDadosPessoais(Usuario usuario, UsuarioCadastroViewModel m)
        {
            usuario.AtualizarDadosPessoais(
                m.RG, m.OrgaoExpedidor, m.EstadoCivil, m.Nacionalidade,
                m.Endereco, m.Bairro, m.Cidade, m.Estado, m.CEP,
                m.TelefoneSecundario);
        }

        // LISTAGEM

        public List<UsuarioViewModel> ListarUsuarios()
        {
            var usuarios = new List<UsuarioViewModel>();

            var alunos = _context.Alunos
                .Include(a => a.Curso)
                .ToList();

            foreach (var aluno in alunos)
            {
                usuarios.Add(ComDadosPessoais(new UsuarioViewModel
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    Email = aluno.Email,
                    Telefone = aluno.Telefone,
                    CPF = aluno.CPF,
                    Inscricao = aluno.Matricula,
                    Tipo = "Aluno",
                    Ativo = aluno.Ativo,

                    Matricula = aluno.Matricula,
                    Curso = aluno.Curso?.Nome,
                    Turno = aluno.Turno,
                    Semestre = aluno.Semestre,
                    DataNasc = aluno.DataNasc
                }, aluno));
            }

            var professores = _context.ProfessoresOrientadores
                .Include(p => p.Cursos)
                .ToList();

            foreach (var professor in professores)
            {
                usuarios.Add(ComDadosPessoais(new UsuarioViewModel
                {
                    Id = professor.Id,
                    Nome = professor.Nome,
                    Email = professor.Email,
                    Telefone = professor.Telefone,
                    CPF = professor.CPF,
                    Inscricao = professor.Registro,
                    Tipo = "Professor",
                    Ativo = professor.Ativo,

                    Registro = professor.Registro,
                    CursosProfessor = professor.Cursos
                        .Select(c => c.Nome)
                        .OrderBy(n => n)
                        .ToList()
                }, professor));
            }

            var supervisores = _context.SupervisoresEstagio
                .Include(s => s.UnidadeConcedente)
                .ToList();

            foreach (var supervisor in supervisores)
            {
                usuarios.Add(ComDadosPessoais(new UsuarioViewModel
                {
                    Id = supervisor.Id,
                    Nome = supervisor.Nome,
                    Email = supervisor.Email,
                    Telefone = supervisor.Telefone,
                    CPF = supervisor.CPF,
                    Inscricao = null,
                    Tipo = "Supervisor",
                    Ativo = supervisor.Ativo,

                    Cargo = supervisor.Cargo,
                    Empresa = supervisor.UnidadeConcedente?.RazaoSocial,
                    CNPJ = supervisor.UnidadeConcedente?.Cnpj
                }, supervisor));
            }

            var coordenadores = _context.Coordenadores.ToList();

            foreach (var coordenador in coordenadores)
            {
                usuarios.Add(ComDadosPessoais(new UsuarioViewModel
                {
                    Id = coordenador.Id,
                    Nome = coordenador.Nome,
                    Email = coordenador.Email,
                    Telefone = coordenador.Telefone,
                    CPF = coordenador.CPF,
                    Inscricao = null,
                    Tipo = "Coordenador",
                    Ativo = coordenador.Ativo,

                    Setor = coordenador.Setor
                }, coordenador));
            }

            return usuarios;
        }

        // CONSULTA SIMPLES

        public UsuarioViewModel? ObterUsuario(Guid id, string tipo)
        {
            if (tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == id);

                if (aluno == null)
                {
                    return null;
                }

                return new UsuarioViewModel
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    Email = aluno.Email,
                    Inscricao = aluno.Matricula,
                    Tipo = "Aluno",
                    Ativo = aluno.Ativo
                };
            }

            if (tipo == "Professor")
            {
                var professor = _context.ProfessoresOrientadores
                    .FirstOrDefault(p => p.Id == id);

                if (professor == null)
                {
                    return null;
                }

                return new UsuarioViewModel
                {
                    Id = professor.Id,
                    Nome = professor.Nome,
                    Email = professor.Email,
                    Inscricao = professor.Registro,
                    Tipo = "Professor",
                    Ativo = professor.Ativo
                };
            }

            if (tipo == "Supervisor")
            {
                var supervisor = _context.SupervisoresEstagio
                    .FirstOrDefault(s => s.Id == id);

                if (supervisor == null)
                {
                    return null;
                }

                return new UsuarioViewModel
                {
                    Id = supervisor.Id,
                    Nome = supervisor.Nome,
                    Email = supervisor.Email,
                    Inscricao = null,
                    Tipo = "Supervisor",
                    Ativo = supervisor.Ativo
                };
            }

            if (tipo == "Coordenador")
            {
                var coordenador = _context.Coordenadores
                    .FirstOrDefault(c => c.Id == id);

                if (coordenador == null)
                {
                    return null;
                }

                return new UsuarioViewModel
                {
                    Id = coordenador.Id,
                    Nome = coordenador.Nome,
                    Email = coordenador.Email,
                    Inscricao = null,
                    Tipo = "Coordenador",
                    Ativo = coordenador.Ativo
                };
            }

            return null;
        }

        // CARREGAR PARA EDIÇÃO

        public UsuarioCadastroViewModel? ObterUsuarioParaEdicao(Guid id, string tipo)
        {
            if (tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == id);

                if (aluno == null)
                {
                    return null;
                }

                return ComDadosPessoais(new UsuarioCadastroViewModel
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    Email = aluno.Email,
                    Telefone = aluno.Telefone,
                    CPF = aluno.CPF,
                    Tipo = "Aluno",
                    InstituicaoEnsinoId = aluno.InstituicaoEnsinoId,
                    Matricula = aluno.Matricula,
                    CursoId = aluno.CursoId,
                    Turno = aluno.Turno,
                    Semestre = aluno.Semestre,
                    DataNasc = aluno.DataNasc
                }, aluno);
            }

            if (tipo == "Professor")
            {
                var professor = _context.ProfessoresOrientadores
                    .Include(p => p.Cursos)
                    .Include(p => p.InstituicoesEnsino)
                    .FirstOrDefault(p => p.Id == id);

                if (professor == null)
                {
                    return null;
                }

                return ComDadosPessoais(new UsuarioCadastroViewModel
                {
                    Id = professor.Id,
                    Nome = professor.Nome,
                    Email = professor.Email,
                    Telefone = professor.Telefone,
                    CPF = professor.CPF,
                    Tipo = "Professor",
                    InstituicaoEnsinoId = professor.InstituicaoEnsinoId,
                    Registro = professor.Registro,
                    CursosProfessorIds = professor.Cursos
                        .Select(c => c.Id)
                        .ToList(),
                    InstituicoesProfessorIds = professor.InstituicoesEnsino
                        .Select(i => i.Id)
                        .ToList()
                }, professor);
            }

            if (tipo == "Supervisor")
            {
                var supervisor = _context.SupervisoresEstagio
                    .FirstOrDefault(s => s.Id == id);

                if (supervisor == null)
                {
                    return null;
                }

                return ComDadosPessoais(new UsuarioCadastroViewModel
                {
                    Id = supervisor.Id,
                    Nome = supervisor.Nome,
                    Email = supervisor.Email,
                    Telefone = supervisor.Telefone,
                    CPF = supervisor.CPF,
                    Tipo = "Supervisor",
                    Cargo = supervisor.Cargo,
                    UnidadeConcedenteId = supervisor.UnidadeConcedenteId
                }, supervisor);
            }

            if (tipo == "Coordenador")
            {
                var coordenador = _context.Coordenadores
                    .FirstOrDefault(c => c.Id == id);

                if (coordenador == null)
                {
                    return null;
                }

                return ComDadosPessoais(new UsuarioCadastroViewModel
                {
                    Id = coordenador.Id,
                    Nome = coordenador.Nome,
                    Email = coordenador.Email,
                    Telefone = coordenador.Telefone,
                    CPF = coordenador.CPF,
                    Tipo = "Coordenador",
                    InstituicaoEnsinoId = coordenador.InstituicaoEnsinoId,
                    Setor = coordenador.Setor
                }, coordenador);
            }

            return null;
        }

        // ATUALIZAR

        public async Task<bool> AtualizarUsuario(UsuarioCadastroViewModel model)
        {
            if (!model.Id.HasValue) return false;

            if (await ValidarDocumentosDuplicadosAsync(model.CPF, model.RG, model.Id) != null)
                return false;

            if (model.Tipo != "Supervisor" && model.Tipo != "Professor" &&
                (model.InstituicaoEnsinoId == null ||
                    model.InstituicaoEnsinoId == Guid.Empty ||
                    !await _context.UnidadesConcedentes.AnyAsync(u =>
                        u.Id == model.InstituicaoEnsinoId.Value &&
                        u.Ativo &&
                        u.InstituicaoEnsino)))
            {
                return false;
            }

            // Localiza a conta Identity vinculada ao perfil que será atualizado.
            string? identityUserId = model.Tipo switch
            {
                "Aluno" => await _context.Alunos.Where(x => x.Id == model.Id.Value)
                    .Select(x => x.UserId).FirstOrDefaultAsync(),
                "Professor" => await _context.ProfessoresOrientadores.Where(x => x.Id == model.Id.Value)
                    .Select(x => x.UserId).FirstOrDefaultAsync(),
                "Supervisor" => await _context.SupervisoresEstagio.Where(x => x.Id == model.Id.Value)
                    .Select(x => x.UserId).FirstOrDefaultAsync(),
                "Coordenador" => await _context.Coordenadores.Where(x => x.Id == model.Id.Value)
                    .Select(x => x.UserId).FirstOrDefaultAsync(),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(identityUserId))
                return false;

            if (model.Tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == model.Id.Value);

                if (aluno == null) return false;

                if (model.CursoId == null ||
                    !await _context.Cursos.AnyAsync(c =>
                        c.Id == model.CursoId.Value &&
                        c.Ativo &&
                        c.InstituicaoId == model.InstituicaoEnsinoId!.Value))
                {
                    return false;
                }

                aluno.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF);

                aluno.DefinirInstituicaoEnsino(model.InstituicaoEnsinoId!.Value);

                AplicarDadosPessoais(aluno, model);

                model.Turno = await _context.Cursos
                    .Where(c => c.Id == model.CursoId.Value && c.Ativo)
                    .Select(c => c.Periodo)
                    .FirstAsync();

                aluno.AtualizarDadosAcademicos(
                    (int)model.Matricula!,
                    model.CursoId.Value,
                    model.Turno!,
                    model.Semestre!,
                    model.DataNasc!.Value);
            }
            else if (model.Tipo == "Professor")
            {
                var professor = _context.ProfessoresOrientadores
                    .Include(p => p.Cursos)
                    .Include(p => p.InstituicoesEnsino)
                    .FirstOrDefault(p => p.Id == model.Id.Value);

                if (professor == null) return false;

                var cursos = await ResolverCursosAsync(model.CursosProfessorIds);

                if (cursos == null) return false;

                var idsInstituicoes = (model.InstituicoesProfessorIds ?? new List<Guid>())
                    .Distinct()
                    .ToList();

                if (idsInstituicoes.Count == 0)
                    return false;

                var instituicoesProfessor = await _context.UnidadesConcedentes
                    .Where(u =>
                        idsInstituicoes.Contains(u.Id) &&
                        u.Ativo &&
                        u.InstituicaoEnsino)
                    .ToListAsync();

                if (instituicoesProfessor.Count != idsInstituicoes.Count)
                {
                    return false;
                }

                if (cursos.Any(c =>
                    !idsInstituicoes.Contains(c.InstituicaoId)))
                {
                    return false;
                }

                professor.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF);

                professor.DefinirInstituicaoEnsino(instituicoesProfessor
                    .OrderBy(i => i.RazaoSocial).First().Id);

                AplicarDadosPessoais(professor, model);

                professor.AtualizarRegistro((int)model.Registro!);

                professor.DefinirCursos(cursos);

                professor.InstituicoesEnsino.Clear();

                foreach (var instituicao in instituicoesProfessor)
                {
                    professor.InstituicoesEnsino.Add(instituicao);
                }
            }
            else if (model.Tipo == "Supervisor")
            {
                var supervisor = _context.SupervisoresEstagio
                    .FirstOrDefault(s => s.Id == model.Id.Value);

                if (supervisor == null) return false;

                if (model.UnidadeConcedenteId == null ||
                    !await _context.UnidadesConcedentes.AnyAsync(u =>
                        u.Id == model.UnidadeConcedenteId.Value &&
                        u.Ativo &&
                        !u.InstituicaoEnsino))
                {
                    return false;
                }

                supervisor.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF);

                AplicarDadosPessoais(supervisor, model);

                supervisor.AtualizarDadosProfissionais(
                    model.Cargo!,
                    model.UnidadeConcedenteId.Value);
            }
            else if (model.Tipo == "Coordenador")
            {
                var coordenador = _context.Coordenadores
                    .FirstOrDefault(c => c.Id == model.Id.Value);

                if (coordenador == null) return false;

                coordenador.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF);

                coordenador.DefinirInstituicaoEnsino(model.InstituicaoEnsinoId!.Value);

                AplicarDadosPessoais(coordenador, model);

                coordenador.AtualizarSetor(model.Setor!);
            }
            else
            {
                return false;
            }

            // Mantém o e-mail de login sincronizado com o e-mail do perfil.
            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser == null)
                return false;

            identityUser.Email = model.Email.Trim();
            identityUser.UserName = model.Email.Trim();
            identityUser.EmailConfirmed = true;

            var resultadoIdentity = await _userManager.UpdateAsync(identityUser);
            if (!resultadoIdentity.Succeeded)
                return false;

            // Garante que o papel de autorização corresponda ao tipo do cadastro.
            var perfilCorreto = PerfisSistema.ParaTipoUsuario(model.Tipo);
            var perfisConhecidos = new[]
            {
                PerfisSistema.Aluno,
                PerfisSistema.Professor,
                PerfisSistema.Supervisor,
                PerfisSistema.Coordenador
            };
            var perfisAtuais = await _userManager.GetRolesAsync(identityUser);
            var perfisRemover = perfisAtuais
                .Where(r => perfisConhecidos.Contains(r) && r != perfilCorreto)
                .ToList();

            if (perfisRemover.Count > 0)
            {
                var removidos = await _userManager.RemoveFromRolesAsync(identityUser, perfisRemover);
                if (!removidos.Succeeded)
                    return false;
            }

            if (!await _userManager.IsInRoleAsync(identityUser, perfilCorreto))
            {
                var adicionado = await _userManager.AddToRoleAsync(identityUser, perfilCorreto);
                if (!adicionado.Succeeded)
                    return false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // INATIVAR / REATIVAR

        public async Task<bool> InativarUsuario(Guid id, string tipo)
        {
            if (tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == id);

                if (aluno == null)
                    return false;

                aluno.Inativar();
            }
            else if (tipo == "Professor")
            {
                var professor = _context.ProfessoresOrientadores
                    .FirstOrDefault(p => p.Id == id);

                if (professor == null)
                    return false;

                professor.Inativar();
            }
            else if (tipo == "Supervisor")
            {
                var supervisor = _context.SupervisoresEstagio
                    .FirstOrDefault(s => s.Id == id);

                if (supervisor == null)
                    return false;

                supervisor.Inativar();
            }
            else if (tipo == "Coordenador")
            {
                var coordenador = _context.Coordenadores
                    .FirstOrDefault(c => c.Id == id);

                if (coordenador == null)
                    return false;

                coordenador.Inativar();
            }
            else
            {
                return false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReativarUsuario(Guid id, string tipo)
        {
            if (tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == id);

                if (aluno == null || aluno.Ativo)
                    return false;

                aluno.Ativar();
            }
            else if (tipo == "Professor")
            {
                var professor = _context.ProfessoresOrientadores
                    .FirstOrDefault(p => p.Id == id);

                if (professor == null || professor.Ativo)
                    return false;

                professor.Ativar();
            }
            else if (tipo == "Supervisor")
            {
                var supervisor = _context.SupervisoresEstagio
                    .FirstOrDefault(s => s.Id == id);

                if (supervisor == null || supervisor.Ativo)
                    return false;

                supervisor.Ativar();
            }
            else if (tipo == "Coordenador")
            {
                var coordenador = _context.Coordenadores
                    .FirstOrDefault(c => c.Id == id);

                if (coordenador == null || coordenador.Ativo)
                    return false;

                coordenador.Ativar();
            }
            else
            {
                return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // CADASTRAR

        public async Task<IdentityResult> CadastrarUsuario(UsuarioCadastroViewModel model)
        {
            var erroDocumento = await ValidarDocumentosDuplicadosAsync(model.CPF, model.RG);
            if (erroDocumento != null)
                return Falha("DocumentoDuplicado", erroDocumento);

            // Valida os vínculos ANTES de criar o usuário no Identity,
            // para não deixar usuário "órfão" se algo estiver errado.
            List<Curso>? cursosProfessor = null;

            List<UnidadeConcedente>? instituicoesProfessor = null;

            if (model.Tipo != "Supervisor" && model.Tipo != "Professor" &&
                (model.InstituicaoEnsinoId == null ||
                    model.InstituicaoEnsinoId == Guid.Empty ||
                    !await _context.UnidadesConcedentes.AnyAsync(u =>
                        u.Id == model.InstituicaoEnsinoId.Value &&
                        u.Ativo &&
                        u.InstituicaoEnsino)))
            {
                return Falha(
                    "InstituicaoInvalida",
                    "Selecione uma instituição de ensino válida.");
            }

            if (model.Tipo == "Aluno")
            {
                if (model.CursoId == null ||
                    !await _context.Cursos.AnyAsync(c =>
                        c.Id == model.CursoId.Value &&
                        c.Ativo &&
                        c.InstituicaoId == model.InstituicaoEnsinoId!.Value))
                {
                    return Falha(
                        "VinculoInvalido",
                        "Selecione um curso pertencente à instituição de ensino informada.");
                }
            }
            else if (model.Tipo == "Professor")
            {
                cursosProfessor = await ResolverCursosAsync(model.CursosProfessorIds);

                if (cursosProfessor == null)
                {
                    return Falha("VinculoInvalido", "Selecione pelo menos um curso válido.");

                }

                var idsInstituicoes = model.InstituicoesProfessorIds
                    .Distinct()
                    .ToList();

                if (idsInstituicoes.Count == 0)
                {
                    return Falha(
                        "InstituicaoInvalida",
                        "Selecione pelo menos uma instituição de ensino para o professor.");
                }

                instituicoesProfessor = await _context.UnidadesConcedentes
                    .Where(u =>
                        idsInstituicoes.Contains(u.Id) &&
                        u.Ativo &&
                        u.InstituicaoEnsino)
                    .ToListAsync();

                if (instituicoesProfessor.Count != idsInstituicoes.Count)
                {
                    return Falha(
                        "InstituicaoInvalida",
                        "Uma ou mais instituições selecionadas são inválidas.");
                }

                if (cursosProfessor.Any(c =>
                    !idsInstituicoes.Contains(c.InstituicaoId)))
                {
                    return Falha(
                        "VinculoInvalido",
                        "Todos os cursos do professor devem pertencer às instituições de ensino selecionadas.");
                }

            }
            else if (model.Tipo == "Supervisor")
            {
                if (model.UnidadeConcedenteId == null ||
                    !await _context.UnidadesConcedentes.AnyAsync(u =>
                        u.Id == model.UnidadeConcedenteId.Value &&
                        u.Ativo &&
                        !u.InstituicaoEnsino))
                {
                    return Falha(
                        "VinculoInvalido",
                        "Selecione uma unidade concedente ativa e válida.");
                }
            }
            else if (model.Tipo != "Coordenador")
            {
                return Falha("TipoInvalido", "Tipo de usuário inválido.");
            }

            var identityUser = new IdentityUser
            {
                UserName = model.Email.Trim(),
                Email = model.Email.Trim(),
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(
                identityUser,
                model.Senha!);

            if (!resultado.Succeeded)
            {
                return resultado;
            }

            var perfil = PerfisSistema.ParaTipoUsuario(model.Tipo);
            var resultadoPerfil = await _userManager.AddToRoleAsync(identityUser, perfil);
            if (!resultadoPerfil.Succeeded)
            {
                await _userManager.DeleteAsync(identityUser);
                return resultadoPerfil;
            }

            if (model.Tipo == "Aluno")
            {
                model.Turno = await _context.Cursos
                    .Where(c => c.Id == model.CursoId!.Value && c.Ativo)
                    .Select(c => c.Periodo)
                    .FirstAsync();

                var aluno = new Aluno(
                    identityUser.Id,
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF,
                    (int)model.Matricula!,
                    model.CursoId!.Value,
                    model.Turno!,
                    model.Semestre!,
                    model.DataNasc!.Value
                );

                aluno.DefinirInstituicaoEnsino(model.InstituicaoEnsinoId!.Value);

                AplicarDadosPessoais(aluno, model);
                _context.Alunos.Add(aluno);
            }
            else if (model.Tipo == "Professor")
            {
                var professor = new ProfessorOrientador(
                    identityUser.Id,
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF,
                    (int)model.Registro!
                );

                professor.DefinirInstituicaoEnsino(instituicoesProfessor!
                    .OrderBy(i => i.RazaoSocial).First().Id);

                foreach (var instituicao in instituicoesProfessor!)
                {
                    professor.InstituicoesEnsino.Add(instituicao);
                }

                professor.DefinirCursos(cursosProfessor!);
                AplicarDadosPessoais(professor, model);
                _context.ProfessoresOrientadores.Add(professor);
            }
            else if (model.Tipo == "Supervisor")
            {
                var supervisor = new SupervisorEstagio(
                    identityUser.Id,
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF,
                    model.Cargo!,
                    model.UnidadeConcedenteId!.Value
                );

                AplicarDadosPessoais(supervisor, model);
                _context.SupervisoresEstagio.Add(supervisor);
            }
            else
            {
                var coordenador = new Coordenador(
                    identityUser.Id,
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF,
                    model.Setor!
                );

                coordenador.DefinirInstituicaoEnsino(model.InstituicaoEnsinoId!.Value);

                AplicarDadosPessoais(coordenador, model);
                _context.Coordenadores.Add(coordenador);
            }

            await _context.SaveChangesAsync();

            return resultado;
        }
    }
}