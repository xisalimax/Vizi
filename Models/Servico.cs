using System.ComponentModel.DataAnnotations.Schema;

namespace ViziLogin.Models
{
    public class Servico
    {
        public int Id { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public string Profissional { get; set; } = string.Empty;
        public string TipoServico { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public string Preco { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public int UsuarioId { get; set; }
        public Usuario ? Usuario { get; set; }

    }
}
