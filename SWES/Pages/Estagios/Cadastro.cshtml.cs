using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SWES.Services;
using SWES.ViewModels;
using System.Security.Cryptography;

namespace SWES.Pages.Estagios
{
    [AllowAnonymous]
    public class CadastroModel : PageModel
    {
        private readonly IDataProtector _protetorProtocolo;
        private readonly EstagioService _estagioService;
        private readonly IWebHostEnvironment _environment;

        private const long TamanhoMaximoArquivo = 10 * 1024 * 1024;

        private readonly string[] extensoesPermitidas =
        {
            ".pdf", ".doc", ".docx", ".txt"
        };

        public CadastroModel(
            EstagioService estagioService,
            IWebHostEnvironment environment,
            IDataProtectionProvider dataProtectionProvider)
        {
            _estagioService = estagioService;
            _environment = environment;

            _protetorProtocolo = dataProtectionProvider.CreateProtector(
                "SWES.Estagios.Protocolo.v1");
        }

        [BindProperty]
        public EstagioCadastroViewModel Estagio { get; set; } = new();

        [BindProperty]
        public string ProtocoloProtegido { get; set; } = string.Empty;

        public OpcoesEstagioViewModel Opcoes { get; set; } = new();

        [TempData]
        public string? Mensagem { get; set; }

        // =====================================================
        // CARREGAR PÁGINA
        // =====================================================

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var estagio = await _estagioService.ObterPorIdAsync(id.Value);

                if (estagio == null)
                    return NotFound();

                Estagio = estagio;
            }
            else
            {
                Estagio.Protocolo = _estagioService.GerarProtocolo();

                ProtocoloProtegido = _protetorProtocolo.Protect(
                    Estagio.Protocolo);
            }

            await CarregarOpcoesAsync();

            return Page();
        }

        // =====================================================
        // CADASTRAR OU ATUALIZAR ESTÁGIO
        // =====================================================

        public async Task<IActionResult> OnPostAsync()
        {
            // O protocolo protegido é validado somente em novos cadastros.

            if (!Estagio.Id.HasValue)
            {
                try
                {
                    var protocoloOriginal =
                        _protetorProtocolo.Unprotect(ProtocoloProtegido);

                    if (protocoloOriginal != Estagio.Protocolo)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            "O protocolo foi alterado. Abra novamente o cadastro.");
                    }
                }
                catch (CryptographicException)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "O protocolo é inválido. Abra novamente o cadastro.");
                }
            }

            ValidarDatas();
            ValidarArquivos();

            if (!ModelState.IsValid)
            {
                await RestaurarDocumentosExistentesAsync();
                await CarregarOpcoesAsync();

                return Page();
            }

            string? caminhoEspecificacoes = null;
            string? nomeEspecificacoes = null;

            string? caminhoEdital = null;
            string? nomeEdital = null;

            var arquivosSalvos = new List<string>();

            var documentosParaSalvar =
                new List<EstagioService.DocumentoParaSalvar>();

            // Indica se o banco já confirmou a operação.
            // Depois disso, não devemos apagar os novos uploads.
            bool bancoSalvo = false;

            try
            {
                // =============================================
                // ARQUIVO DAS ESPECIFICAÇÕES
                // =============================================

                if (Estagio.ArquivoEspecificacoes != null)
                {
                    var arquivo = await SalvarArquivoAsync(
                        Estagio.ArquivoEspecificacoes);

                    caminhoEspecificacoes = arquivo.Caminho;
                    nomeEspecificacoes = arquivo.NomeOriginal;

                    arquivosSalvos.Add(arquivo.CaminhoFisico);
                }

                // =============================================
                // ARQUIVO DO EDITAL
                // =============================================

                if (Estagio.ArquivoEdital != null)
                {
                    var arquivo = await SalvarArquivoAsync(
                        Estagio.ArquivoEdital);

                    caminhoEdital = arquivo.Caminho;
                    nomeEdital = arquivo.NomeOriginal;

                    arquivosSalvos.Add(arquivo.CaminhoFisico);
                }

                // =============================================
                // NOVOS DOCUMENTOS ANEXADOS
                // =============================================

                foreach (var documento in Estagio.Documentos)
                {
                    if (documento.Arquivo == null)
                        continue;

                    var arquivo = await SalvarArquivoAsync(
                        documento.Arquivo);

                    arquivosSalvos.Add(arquivo.CaminhoFisico);

                    documentosParaSalvar.Add(
                        new EstagioService.DocumentoParaSalvar(
                            documento.Nome.Trim(),
                            arquivo.NomeOriginal,
                            arquivo.Caminho
                        )
                    );
                }

                // =============================================
                // ATUALIZAR ESTÁGIO EXISTENTE
                // =============================================

                if (Estagio.Id.HasValue)
                {
                    var arquivosAntigosRemovidos =
                        await _estagioService.AtualizarAsync(
                            Estagio,
                            caminhoEspecificacoes,
                            nomeEspecificacoes,
                            caminhoEdital,
                            nomeEdital,
                            documentosParaSalvar
                        );

                    // A partir daqui, o banco já confirmou
                    // a atualização dos dados.
                    bancoSalvo = true;

                    // Exclui os arquivos físicos antigos somente
                    // depois da confirmação do banco.
                    foreach (var caminhoAntigo in arquivosAntigosRemovidos)
                    {
                        ExcluirArquivoFisico(caminhoAntigo);
                    }

                    Mensagem = "Estágio atualizado com sucesso!";
                }
                else
                {
                    // =========================================
                    // CADASTRAR NOVO ESTÁGIO
                    // =========================================

                    await _estagioService.CadastrarAsync(
                        Estagio,
                        caminhoEspecificacoes,
                        nomeEspecificacoes,
                        caminhoEdital,
                        nomeEdital,
                        documentosParaSalvar
                    );

                    bancoSalvo = true;

                    Mensagem = "Estágio cadastrado com sucesso!";
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Se o banco ainda não confirmou a operação,
                // os novos arquivos podem ser removidos.
                if (!bancoSalvo)
                {
                    foreach (var caminho in arquivosSalvos)
                    {
                        try
                        {
                            if (System.IO.File.Exists(caminho))
                            {
                                System.IO.File.Delete(caminho);
                            }
                        }
                        catch
                        {
                            // Evita que uma falha na limpeza
                            // esconda o erro original.
                        }
                    }
                }

                ModelState.AddModelError(
                    string.Empty,
                    $"Não foi possível salvar o estágio: {ex.Message}");

                await RestaurarDocumentosExistentesAsync();
                await CarregarOpcoesAsync();

                return Page();
            }
        }

        // =====================================================
        // RESTAURAR DOCUMENTOS EXISTENTES NA PÁGINA
        // =====================================================

        private async Task RestaurarDocumentosExistentesAsync()
        {
            if (!Estagio.Id.HasValue)
                return;

            var estagioOriginal = await _estagioService.ObterPorIdAsync(
                Estagio.Id.Value);

            if (estagioOriginal == null)
                return;

            var idsRemovidos = Estagio.DocumentosRemoverIds
                .Distinct()
                .ToHashSet();

            Estagio.DocumentosExistentes = estagioOriginal.DocumentosExistentes
                .Where(d => !idsRemovidos.Contains(d.Id))
                .ToList();
        }

        // =====================================================
        // EXCLUIR ARQUIVO FÍSICO ANTIGO
        // =====================================================

        private void ExcluirArquivoFisico(string? caminhoWeb)
        {
            if (string.IsNullOrWhiteSpace(caminhoWeb))
                return;

            try
            {
                // O banco armazena caminhos como:
                // /uploads/estagios/arquivo.pdf

                var caminhoRelativo = caminhoWeb
                    .TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar);

                var pastaUploads = Path.GetFullPath(
                    Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "estagios"
                    )
                );

                var caminhoFisico = Path.GetFullPath(
                    Path.Combine(
                        _environment.WebRootPath,
                        caminhoRelativo
                    )
                );

                // Impede que um caminho armazenado no banco
                // tente excluir arquivos fora da pasta de uploads.
                var prefixoPermitido =
                    pastaUploads + Path.DirectorySeparatorChar;

                if (!caminhoFisico.StartsWith(
                    prefixoPermitido,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (System.IO.File.Exists(caminhoFisico))
                {
                    System.IO.File.Delete(caminhoFisico);
                }
            }
            catch
            {
                // A atualização do banco já foi concluída.
                // Uma falha na exclusão física não deve
                // desfazer nem comprometer os dados salvos.
            }
        }

        // =====================================================
        // BUSCAR PROFESSORES POR INSTITUIÇÃO
        // =====================================================

        public async Task<IActionResult> OnGetProfessoresAsync(
            Guid instituicaoId)
        {
            if (instituicaoId == Guid.Empty)
                return BadRequest();

            var professores = await _estagioService
                .ObterProfessoresPorInstituicaoAsync(instituicaoId);

            return new JsonResult(professores);
        }

        // =====================================================
        // BUSCAR ALUNOS POR INSTITUIÇÃO
        // =====================================================

        public async Task<IActionResult> OnGetAlunosAsync(
            Guid instituicaoId)
        {
            if (instituicaoId == Guid.Empty)
                return BadRequest();

            var alunos = await _estagioService
                .ObterAlunosPorInstituicaoAsync(instituicaoId);

            return new JsonResult(alunos);
        }

        // =====================================================
        // BUSCAR COORDENADORES POR INSTITUIÇÃO
        // =====================================================

        public async Task<IActionResult> OnGetCoordenadoresAsync(
            Guid instituicaoId)
        {
            if (instituicaoId == Guid.Empty)
                return BadRequest();

            var coordenadores = await _estagioService
                .ObterCoordenadoresPorInstituicaoAsync(instituicaoId);

            return new JsonResult(coordenadores);
        }

        // =====================================================
        // BUSCAR SUPERVISORES POR UNIDADE
        // =====================================================

        public async Task<IActionResult> OnGetSupervisoresAsync(
            Guid unidadeConcedenteId)
        {
            if (unidadeConcedenteId == Guid.Empty)
                return BadRequest();

            var supervisores = await _estagioService
                .ObterSupervisoresPorUnidadeAsync(unidadeConcedenteId);

            return new JsonResult(supervisores);
        }

        // =====================================================
        // CARREGAR OPÇÕES DOS CAMPOS
        // =====================================================

        private async Task CarregarOpcoesAsync()
        {
            Opcoes = await _estagioService.ObterOpcoesAsync();
        }

        // =====================================================
        // VALIDAR DATAS
        // =====================================================

        private void ValidarDatas()
        {
            if (Estagio.DataInicio.HasValue &&
                Estagio.DataFim.HasValue &&
                Estagio.DataFim.Value.Date < Estagio.DataInicio.Value.Date)
            {
                ModelState.AddModelError(
                    "Estagio.DataFim",
                    "A data de término não pode ser anterior à data de início.");
            }
        }

        // =====================================================
        // VALIDAR ARQUIVOS
        // =====================================================

        private void ValidarArquivos()
        {
            ValidarArquivo(
                Estagio.ArquivoEspecificacoes,
                "Estagio.ArquivoEspecificacoes",
                "especificações");

            ValidarArquivo(
                Estagio.ArquivoEdital,
                "Estagio.ArquivoEdital",
                "edital");

            for (int i = 0; i < Estagio.Documentos.Count; i++)
            {
                var documento = Estagio.Documentos[i];

                if (string.IsNullOrWhiteSpace(documento.Nome))
                {
                    ModelState.AddModelError(
                        $"Estagio.Documentos[{i}].Nome",
                        "Informe o nome do documento.");
                }

                if (documento.Arquivo == null ||
                    documento.Arquivo.Length == 0)
                {
                    ModelState.AddModelError(
                        $"Estagio.Documentos[{i}].Arquivo",
                        "Selecione um arquivo válido.");

                    continue;
                }

                ValidarArquivo(
                    documento.Arquivo,
                    $"Estagio.Documentos[{i}].Arquivo",
                    $"documento '{documento.Nome}'");
            }
        }

        // =====================================================
        // VALIDAR UM ARQUIVO
        // =====================================================

        private void ValidarArquivo(
            IFormFile? arquivo,
            string campo,
            string descricao)
        {
            if (arquivo == null || arquivo.Length == 0)
                return;

            var extensao = Path.GetExtension(arquivo.FileName)
                .ToLowerInvariant();

            if (!extensoesPermitidas.Contains(extensao))
            {
                ModelState.AddModelError(
                    campo,
                    $"O arquivo de {descricao} deve estar em PDF, DOC, DOCX ou TXT.");

                return;
            }

            if (arquivo.Length > TamanhoMaximoArquivo)
            {
                ModelState.AddModelError(
                    campo,
                    $"O arquivo de {descricao} não pode ultrapassar 10 MB.");
            }
        }

        // =====================================================
        // SALVAR ARQUIVO NO SERVIDOR
        // =====================================================

        private async Task<(
            string Caminho,
            string NomeOriginal,
            string CaminhoFisico)> SalvarArquivoAsync(IFormFile arquivo)
        {
            var pasta = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "estagios");

            Directory.CreateDirectory(pasta);

            var nomeOriginal = Path.GetFileName(arquivo.FileName);

            var extensao = Path.GetExtension(nomeOriginal)
                .ToLowerInvariant();

            var nomeArmazenado = $"{Guid.NewGuid()}{extensao}";

            var caminhoFisico = Path.Combine(
                pasta,
                nomeArmazenado);

            await using (var stream = new FileStream(
                caminhoFisico,
                FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var caminhoWeb = $"/uploads/estagios/{nomeArmazenado}";

            return (
                caminhoWeb,
                nomeOriginal,
                caminhoFisico
            );
        }
    }
}