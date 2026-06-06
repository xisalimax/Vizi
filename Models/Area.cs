using System.ComponentModel.DataAnnotations;

namespace ViziLogin.Models
{
    public class Area
    {
        [Key]
        public int Id_Area { get; set; }

        [Required]
        public string Nome { get; set; }
    }
}


