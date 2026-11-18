
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SWES.ViewModels
{
    public class EstagioCadastroViewModel
    {
        public int? Id { get; set; }

        public string Protocolo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione a etapa do estágio.")]
        public string Etapa { get; set; } = "Estágio I";

        [Required(ErrorMessage = "Selecione o tipo de estágio.")]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data de início.")]
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }

        [Required(ErrorMessage = "Informe a data de término.")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }

        [Required(ErrorMessage = "Informe o horário.")]
        [StringLength(100)]
        public string Horario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o professor orientador.")]
        public Guid? ProfessorOrientadorId { get; set; }

        [Required(ErrorMessage = "Selecione o supervisor.")]
        public Guid? SupervisorEstagioId { get; set; }

        [Required(ErrorMessage = "Selecione a unidade concedente.")]
        public Guid? UnidadeConcedenteId { get; set; }

        [Required(ErrorMessage = "Selecione a instituição de ensino.")]
        public Guid? InstituicaoEnsinoId { get; set; }

        public Guid? CoordenadorId { get; set; }

        [Required(ErrorMessage = "Selecione pelo menos um aluno.")]
        [MinLength(1, ErrorMessage = "Selecione pelo menos um aluno.")]
        public List<Guid> AlunosIds { get; set; } = new();

        [Required(ErrorMessage = "Informe o número da apólice.")]
        [StringLength(100)]
        public string ApoliceSeguro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a seguradora.")]
        [StringLength(150)]
        public string Seguradora { get; set; } = string.Empty;

        [StringLength(500)]
        public string Observacao { get; set; } = string.Empty;

        // Conteúdo único das especificações do estágio.
        public string? EspecificacoesHtml { get; set; }

        // Arquivo principal das especificações.
        public IFormFile? ArquivoEspecificacoes { get; set; }

        // Arquivo do edital, independente das especificações.
        public IFormFile? ArquivoEdital { get; set; }

        // Novos documentos enviados durante o cadastro ou edição.
        public List<DocumentoAnexoViewModel> Documentos { get; set; } = new();

        // Documentos que já estão persistidos no banco.
        public List<DocumentoExistenteViewModel> DocumentosExistentes { get; set; } = new();

        // IDs dos documentos persistidos que o usuário solicitou excluir.
        public List<int> DocumentosRemoverIds { get; set; } = new();

        // Arquivo atual das especificações.
        public string? EspecificacoesArquivoAtual { get; set; }

        public string? NomeArquivoEspecificacoesAtual { get; set; }

        // Arquivo atual do edital.
        public string? EditalArquivoAtual { get; set; }

        public string? NomeArquivoEditalAtual { get; set; }

        public string Situacao { get; set; } = "Em andamento";

        public static readonly string[] EtapasDisponiveis =
        {
            "Estágio I",
            "Estágio II",
            "Estágio III",
            "Estágio IV"
        };

        public static readonly string[] SituacoesDisponiveis =
        {
            "Em andamento",
            "Pendente",
            "Validado",
            "Concluído",
            "Cancelado"
        };
    }
}