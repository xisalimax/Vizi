using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViziLogin.Models
{
    public class Avaliacao
    {
        [Key]
        public int IdAvaliacao { get; set; }

        public string NomeAvaliador { get; set; }

        [Range(0, 5)]
        [Column(TypeName = "decimal(3, 1)")]
        public decimal Nota { get; set; }

        public string Comentario { get; set; }

        [Display(Name = "Data da Avaliação")]
        public DateTime DataAvaliacao { get; set; } = DateTime.Now;
        public int ServicoId { get; set; }
        public Servico? Servico { get; set; }
    }
}