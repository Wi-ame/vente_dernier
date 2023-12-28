using System.ComponentModel.DataAnnotations;

namespace vente_en_ligne.Models
{
    public class Favori
    {
        [Key]
        public int Id_fav { get; set; }
        [Required]
        public string IDP { get; set; }
    }
}
