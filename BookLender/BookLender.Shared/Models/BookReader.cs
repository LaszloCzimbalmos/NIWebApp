using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace BookLender.Shared.Models
{
    public class BookReader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReaderId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(BookReader), nameof(ValidateBirthDate))]
        public DateTime BirthDate { get; set; }

        public static ValidationResult ValidateBirthDate(DateTime birthDate, ValidationContext context)
        {
            if (birthDate.Year < 1900)
            {
                return new ValidationResult("The birth date can't be before 1900");
            }

            return ValidationResult.Success;
        }
    }
}
