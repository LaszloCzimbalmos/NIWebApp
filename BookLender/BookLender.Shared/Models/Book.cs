using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLender.Shared.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        [MaxLength(100)]
        public string Publisher { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The year of publication cannot be negative.")]
        public int PublicationYear { get; set; }
    }
}
