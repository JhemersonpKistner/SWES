using System;
using Microsoft.AspNetCore.Identity;

namespace SWES.Models
{
    public class Notificacao
    {
        public int Id { get; private set; }
        public string Mensagem { get; private set; }
        public DateTime DataEnvio { get; private set; }
        public bool Lida { get; private set; }

        public string UsuarioId { get; private set; }
        public IdentityUser Usuario { get; private set; }

        private Notificacao() { }

        public Notificacao(string mensagem, string usuarioId)
        {
            Mensagem = mensagem;
            UsuarioId = usuarioId;
            Lida = false;
        }

        public void Enviar()
        {
            DataEnvio = DateTime.Now;
        }

        public void MarcarComoLida()
        {
            Lida = true;
        }
    }
}