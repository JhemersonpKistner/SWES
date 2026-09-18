using System.Collections.Generic;
using System.Linq;

namespace SWES.Models
{
    public class Coordenador : Usuario
    {
        public string Setor { get; private set; }

        private Coordenador() { }

        public Coordenador(
            string userId,
            string nome,
            string email,
            string telefone,
            string setor)
            : base(userId, nome, email, telefone)
        {
            Setor = setor;
        }

        public bool ValidarEstagio(Estagio estagio)
        {
            estagio.AlterarSituacao("Validado");
            return true;
        }

        public bool AnalisarPlano(Estagio estagio)
        {
            return !string.IsNullOrWhiteSpace(estagio.Observacao);
        }

        public void AcompanharEstagio(Estagio estagio)
        {
            // Registrar acompanhamento pela coordenação
        }

        public List<Documento> ConsultarRelatorio(Estagio estagio)
        {
            return estagio.Documentos
                .Where(d => d.Tipo == "Relatorio")
                .ToList();
        }
    }
}