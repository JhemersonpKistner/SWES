using Microsoft.AspNetCore.Identity;
using SWES.Data;
using SWES.Models;

namespace SWES.Services
{
    /// <summary>
    /// Gera dados fictícios somente para desenvolvimento/testes de paginação.
    /// Os registros utilizam o domínio @swes.local para facilitar a identificação.
    /// </summary>
    public class DadosTesteService
    {
        private const string PrefixoEmail = "usuario.teste";
        private const string DominioEmail = "@swes.local";
        private const string SenhaPadrao = "Teste@12345";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private static readonly string[] Nomes =
        {
            "Ana Beatriz Souza", "Bruno Henrique Lima", "Camila Ferreira Alves",
            "Daniel Martins Costa", "Eduarda Oliveira Santos", "Felipe Rodrigues Silva",
            "Gabriela Mendes Rocha", "Henrique Almeida Gomes", "Isabela Carvalho Dias",
            "João Pedro Nunes", "Karina Barbosa Freitas", "Lucas Ribeiro Martins",
            "Mariana Castro Lopes", "Nathan Pereira Alves", "Olivia Moreira Santos",
            "Paulo Henrique Duarte", "Rafaela Fernandes Costa", "Samuel Correia Lima",
            "Tatiane Moura Souza", "Victor Hugo Cardoso"
        };

        private static readonly string[] Cursos =
        {
            "Sistemas de Informação", "Administração", "Direito", "Engenharia Civil"
        };

        private static readonly string[] Turnos =
        {
            "Matutino", "Vespertino", "Noturno", "Integral"
        };

        private static readonly string[] Semestres =
        {
            "1º semestre", "2º semestre", "3º semestre", "4º semestre",
            "5º semestre", "6º semestre", "7º semestre", "8º semestre"
        };

        private static readonly string[] Cargos =
        {
            "Analista de Sistemas", "Desenvolvedor", "Assistente Administrativo",
            "Analista de RH", "Coordenador de Projetos", "Técnico de Suporte"
        };

        private static readonly string[] Empresas =
        {
            "Empresa Modelo Tecnologia", "Soluções Alpha", "Grupo Horizonte",
            "Núcleo Digital", "Inova Serviços", "Conecta Sistemas"
        };

        private static readonly string[] Setores =
        {
            "Coordenação de Estágios", "Tecnologia da Informação", "Recursos Humanos",
            "Secretaria Acadêmica", "Administração"
        };

        public DadosTesteService(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int QuantidadeAtual
        {
            get
            {
                return _context.Alunos.Count(a => a.Email.EndsWith(DominioEmail))
                     + _context.ProfessoresOrientadores.Count(p => p.Email.EndsWith(DominioEmail))
                     + _context.SupervisoresEstagio.Count(s => s.Email.EndsWith(DominioEmail))
                     + _context.Coordenadores.Count(c => c.Email.EndsWith(DominioEmail));
            }
        }

        public async Task<int> GerarAsync(int quantidadeDesejada)
        {
            if (quantidadeDesejada < 1)
                throw new ArgumentOutOfRangeException(nameof(quantidadeDesejada));

            // A página permite informar uma quantidade total desejada.
            // Se já existirem 250 registros fictícios e for solicitado 250 novamente,
            // nenhum registro duplicado será criado.
            var existentes = QuantidadeAtual;
            var faltantes = quantidadeDesejada - existentes;

            if (faltantes <= 0)
                return 0;

            // Identity e as entidades de perfil usam o mesmo DbContext.
            // A transação evita deixar usuários do Identity órfãos caso a gravação
            // de algum perfil falhe durante a geração dos dados de teste.
            await using var transacao = await _context.Database.BeginTransactionAsync();

            var proximoNumero = ObterProximoNumero();
            var proximaMatricula = ObterProximaMatricula();
            var proximoRegistro = ObterProximoRegistro();

            var alunos = new List<Aluno>();
            var professores = new List<ProfessorOrientador>();
            var supervisores = new List<SupervisorEstagio>();
            var coordenadores = new List<Coordenador>();

            for (var i = 0; i < faltantes; i++)
            {
                var numero = proximoNumero + i;
                var nome = Nomes[i % Nomes.Length];
                var nomeCompleto = $"{nome} {numero:0000}";
                var email = $"{PrefixoEmail}{numero:0000}{DominioEmail}";
                var telefone = $"6199{(numero % 10000000):0000000}";
                var cpf = GerarCpf(numero);
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var resultado = await _userManager.CreateAsync(user, SenhaPadrao);
                if (!resultado.Succeeded)
                {
                    var erros = string.Join(" ", resultado.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Não foi possível criar o usuário de teste {email}: {erros}");
                }

                var tipo = i % 4;

                switch (tipo)
                {
                    case 0:
                        alunos.Add(new Aluno(
                            user.Id,
                            nomeCompleto,
                            email,
                            telefone,
                            cpf,
                            proximaMatricula++,
                            Cursos[i % Cursos.Length],
                            Turnos[i % Turnos.Length],
                            Semestres[i % Semestres.Length],
                            new DateTime(1995 + (i % 10), 1 + (i % 12), 1 + (i % 27))));
                        break;

                    case 1:
                        professores.Add(new ProfessorOrientador(
                            user.Id,
                            nomeCompleto,
                            email,
                            telefone,
                            cpf,
                            Cursos[i % Cursos.Length],
                            proximoRegistro++));
                        break;

                    case 2:
                        supervisores.Add(new SupervisorEstagio(
                            user.Id,
                            nomeCompleto,
                            email,
                            telefone,
                            cpf,
                            GerarCnpj(numero),
                            Cargos[i % Cargos.Length],
                            Empresas[i % Empresas.Length]));
                        break;

                    default:
                        coordenadores.Add(new Coordenador(
                            user.Id,
                            nomeCompleto,
                            email,
                            telefone,
                            cpf,
                            Setores[i % Setores.Length]));
                        break;
                }
            }

            _context.Alunos.AddRange(alunos);
            _context.ProfessoresOrientadores.AddRange(professores);
            _context.SupervisoresEstagio.AddRange(supervisores);
            _context.Coordenadores.AddRange(coordenadores);

            await _context.SaveChangesAsync();
            await transacao.CommitAsync();

            return faltantes;
        }

        private int ObterProximoNumero()
        {
            var maior = _userManager.Users
                .Where(u => u.Email != null && u.Email.StartsWith(PrefixoEmail))
                .Select(u => u.Email!)
                .AsEnumerable()
                .Select(ExtrairNumero)
                .DefaultIfEmpty(0)
                .Max();

            return maior + 1;
        }

        private int ObterProximaMatricula()
        {
            var maior = _context.Alunos.Any()
                ? _context.Alunos.Max(a => a.Matricula)
                : 0;

            if (maior >= int.MaxValue)
                throw new InvalidOperationException("Não há matrículas disponíveis para gerar os dados de teste.");

            return maior + 1;
        }

        private int ObterProximoRegistro()
        {
            var maior = _context.ProfessoresOrientadores.Any()
                ? _context.ProfessoresOrientadores.Max(p => p.Registro)
                : 0;

            if (maior >= int.MaxValue)
                throw new InvalidOperationException("Não há registros disponíveis para gerar os dados de teste.");

            return maior + 1;
        }

        private static int ExtrairNumero(string email)
        {
            var inicio = email.IndexOf(PrefixoEmail, StringComparison.Ordinal);
            if (inicio < 0)
                return 0;

            var trecho = email[(inicio + PrefixoEmail.Length)..];
            var numero = new string(trecho.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(numero, out var valor) ? valor : 0;
        }

        private static string GerarCpf(int numero)
        {
            // Base determinística de 9 dígitos, seguida dos dois dígitos verificadores.
            var baseCpf = (100_000_000 + numero).ToString("D9");
            var primeiro = CalcularDigito(baseCpf);
            var segundo = CalcularDigito(baseCpf + primeiro);
            return baseCpf + primeiro + segundo;
        }

        private static int CalcularDigito(string valor)
        {
            var tamanho = valor.Length;
            var soma = 0;

            for (var i = 0; i < tamanho; i++)
                soma += (valor[i] - '0') * (tamanho + 1 - i);

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static string GerarCnpj(int numero)
        {
            var baseCnpj = (10_000_000 + numero).ToString("D8") + "0001";
            var primeiro = CalcularDigitoCnpj(baseCnpj);
            var segundo = CalcularDigitoCnpj(baseCnpj + primeiro);
            return baseCnpj + primeiro + segundo;
        }

        private static int CalcularDigitoCnpj(string valor)
        {
            var pesos = valor.Length == 12
                ? new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }
                : new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var soma = 0;
            for (var i = 0; i < valor.Length; i++)
                soma += (valor[i] - '0') * pesos[i];

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
