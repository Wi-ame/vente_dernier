using System.ComponentModel.DataAnnotations;

namespace vente_en_ligne.Models
{
    public class Ban
    {
        [Key]
        public int Id_Ban { get; set; }
        [Required]
        public string IDP { get; set;}
    }
}
