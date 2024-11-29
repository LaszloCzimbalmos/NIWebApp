using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace BookLender.Shared.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanId { get; set; }

        [Required]
        public int ReaderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Loan), nameof(ValidateLoanDate))]
        public DateTime LoanDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Loan), nameof(ValidateReturnDate))]
        public DateTime ReturnDueDate { get; set; }

        public static ValidationResult ValidateLoanDate(DateTime loanDate, ValidationContext context)
        {
            if (loanDate.Date < DateTime.Now.Date)
            {
                return new ValidationResult("The loan date cannot be earlier than today.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateReturnDate(DateTime returnDueDate, ValidationContext context)
        {
            var instance = (Loan)context.ObjectInstance;
            if (returnDueDate <= instance.LoanDate)
            {
                return new ValidationResult("The return due date must be later than the loan date.");
            }
            return ValidationResult.Success;
        }
    }
}
