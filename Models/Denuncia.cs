using System.ComponentModel.DataAnnotations;

namespace ViziLogin.Models
{
    public class Denuncia
    {
        public int Id { get; set; }

        public int ServicoId { get; set; }

        public Servico Servico { get; set; }

        [Required]
        public string Motivo { get; set; }

        public string NomeUsuario { get; set; }

        public DateTime DataDenuncia { get; set; } = DateTime.Now;
    }
}