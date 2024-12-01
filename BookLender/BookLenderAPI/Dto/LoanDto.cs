using System.ComponentModel.DataAnnotations;

namespace BookLenderAPI.Dto
{
    public class LoanDto
    {
        [Required]
        public string ReaderName { get; set; }

        [Required]
        public string BookTitle { get; set; }

        [Range(1, 6, ErrorMessage = "Maximum rent time is 6 months.")]
        public int Months { get; set; }
    }
}
