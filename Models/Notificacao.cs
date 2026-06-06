namespace ViziLogin.Models
{
    public class Notificacao
    {
        public int Id { get; set; }

        public string UsuarioDestino { get; set; } = string.Empty;

        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataNotificacao { get; set; } = DateTime.Now;

        public bool Lida { get; set; } = false;

        public int? ServicoId { get; set; }
    }
}