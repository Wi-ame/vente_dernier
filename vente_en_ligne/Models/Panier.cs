using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vente_en_ligne.Models
{
    public class Panier
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Produit")]
        [Required]
        public int IDPro { get; set; }
        [Required]
        public int Quantité { get; set; }
        [Required]
        public double Total { get; set; }

    }
}