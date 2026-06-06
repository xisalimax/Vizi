using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViziLogin.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }

        public string? Tipo_Perfil { get; set; }

        [Column("Id_Area")]
        public int? Id_Area { get; set; }

        [ForeignKey("Id_Area")]
        public Area? Area { get; set; }

        public string? TelefonePessoal { get; set; }
        public string? TelefoneServico { get; set; }
        public string? Sobre { get; set; }
        public DateTime? Inclusao { get; set; } = DateTime.Now;
    }
}