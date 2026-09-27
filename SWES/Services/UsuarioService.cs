using Microsoft.AspNetCore.Identity;
using SWES.Data;
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

        public List<UsuarioViewModel> ListarUsuarios()
        {
            var usuarios = new List<UsuarioViewModel>();
            var alunos = _context.Alunos.ToList();
            foreach (var aluno in alunos)
            {
                usuarios.Add(new UsuarioViewModel
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    Email = aluno.Email,
                    Inscricao = aluno.Matricula,
                    Tipo = "Aluno",
                    Ativo = aluno.Ativo
                });
            }

            var professores = _context.ProfessoresOrientadores.ToList();
            foreach (var professor in professores)
            {
                usuarios.Add(new UsuarioViewModel
                {
                    Id = professor.Id,
                    Nome = professor.Nome,
                    Email = professor.Email,
                    Inscricao = professor.Registro,
                    Tipo = "Professor",
                    Ativo = professor.Ativo
                });
            }

            var supervisores = _context.SupervisoresEstagio.ToList();
            foreach (var supervisor in supervisores)
            {
                usuarios.Add(new UsuarioViewModel
                {
                    Id = supervisor.Id,
                    Nome = supervisor.Nome,
                    Email = supervisor.Email,
                    Inscricao = 0,
                    Tipo = "Supervisor",
                    Ativo = supervisor.Ativo
                });
            }

            var coordenadores = _context.Coordenadores.ToList();
            foreach (var coordenador in coordenadores)
            {
                usuarios.Add(new UsuarioViewModel
                {
                    Id = coordenador.Id,
                    Nome = coordenador.Nome,
                    Email = coordenador.Email,
                    Inscricao = 0,
                    Tipo = "Coordenador",
                    Ativo = coordenador.Ativo
                });
            }

            return usuarios;
        }

        public UsuarioViewModel? ObterUsuario(int id, string tipo)
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
                    Inscricao = 0,
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
                    Inscricao = 0,
                    Tipo = "Coordenador",
                    Ativo = coordenador.Ativo
                };
            }

            return null;
        }

        public UsuarioCadastroViewModel? ObterUsuarioParaEdicao(int id, string tipo)
        {
            if (tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == id);

                if (aluno == null)
                {
                    return null;
                }

                return new UsuarioCadastroViewModel
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    Email = aluno.Email,
                    Telefone = aluno.Telefone,
                    Tipo = "Aluno",
                    Matricula = aluno.Matricula,
                    Curso = aluno.Curso,
                    Turno = aluno.Turno,
                    Semestre = aluno.Semestre,
                    DataNasc = aluno.DataNasc
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

                return new UsuarioCadastroViewModel
                {
                    Id = professor.Id,
                    Nome = professor.Nome,
                    Email = professor.Email,
                    Telefone = professor.Telefone,
                    Tipo = "Professor",
                    Registro = professor.Registro
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

                return new UsuarioCadastroViewModel
                {
                    Id = supervisor.Id,
                    Nome = supervisor.Nome,
                    Email = supervisor.Email,
                    Telefone = supervisor.Telefone,
                    Tipo = "Supervisor",
                    Cargo = supervisor.Cargo,
                    Empresa = supervisor.Empresa
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

                return new UsuarioCadastroViewModel
                {
                    Id = coordenador.Id,
                    Nome = coordenador.Nome,
                    Email = coordenador.Email,
                    Telefone = coordenador.Telefone,
                    Tipo = "Coordenador",
                    Setor = coordenador.Setor
                };
            }

            return null;
        }

        public async Task<bool> AtualizarUsuario(UsuarioCadastroViewModel model)
        {
            if (!model.Id.HasValue) return false;

            if (model.Tipo == "Aluno")
            {
                var aluno = _context.Alunos
                    .FirstOrDefault(a => a.Id == model.Id.Value);

                if (aluno == null) return false;

                aluno.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone);

                if (model.DataNasc.HasValue)
                {
                    aluno.AtualizarDadosAcademicos(
                        (int)model.Matricula,
                        model.Curso,
                        model.Turno,
                        model.Semestre,
                        model.DataNasc.Value);
                }
            }
            else if (model.Tipo == "Professor")
            {
                var professor = _context.ProfessoresOrientadores
                    .FirstOrDefault(p => p.Id == model.Id.Value);

                if (professor == null)
                    return false;

                professor.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone);

                professor.AtualizarRegistro((short)model.Registro);
            }
            else if (model.Tipo == "Supervisor")
            {
                var supervisor = _context.SupervisoresEstagio
                    .FirstOrDefault(s => s.Id == model.Id.Value);

                if (supervisor == null)
                    return false;

                supervisor.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone);

                supervisor.AtualizarDadosProfissionais(
                    model.Cargo,
                    model.Empresa);
            }
            else if (model.Tipo == "Coordenador")
            {
                var coordenador = _context.Coordenadores
                    .FirstOrDefault(c => c.Id == model.Id.Value);

                if (coordenador == null)
                    return false;

                coordenador.AtualizarDados(
                    model.Nome,
                    model.Email,
                    model.Telefone);

                coordenador.AtualizarSetor(model.Setor);
            }
            else
            {
                return false;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> InativarUsuario(int id, string tipo)
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

        public async Task<IdentityResult> CadastrarUsuario(UsuarioCadastroViewModel model)
        {
            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var resultado = await _userManager.CreateAsync(
                identityUser,
                model.Senha);

            if (!resultado.Succeeded)
            {
                return resultado;
            }

            if (model.Tipo == "Aluno")
            {
                var aluno = new Aluno(
                    identityUser.Id,
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF,
                    (int)model.Matricula,
                    model.Curso,
                    model.Turno,
                    model.Semestre,
                    model.DataNasc ?? DateTime.Now
                );

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
                    model.Curso,
                    (int)model.Registro
                    
                );

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
                    (int)model.CNPJ,
                    model.Cargo,
                    model.Empresa
                );

                _context.SupervisoresEstagio.Add(supervisor);
            }
            else if (model.Tipo == "Coordenador")
            {
                var coordenador = new Coordenador(
                    identityUser.Id,
                    model.Nome,
                    model.Email,
                    model.Telefone,
                    model.CPF,
                    model.Setor
                );

                _context.Coordenadores.Add(coordenador);
            }

            await _context.SaveChangesAsync();

            return resultado;
        }

    }

}