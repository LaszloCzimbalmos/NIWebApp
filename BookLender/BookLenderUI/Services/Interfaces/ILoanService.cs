using BookLender.Shared.Dto;
using BookLender.Shared.Models;

namespace BookLenderUI.Services.Interfaces
{
    public interface ILoanService
    {
        public Task<List<Book>> GetRentedBooksForReader(string name);

        public Task<Loan> GetLoanByBookAndReader(int bookId, int readerId);

        public Task<List<Loan>> GetLateLoans();

        public Task<List<Loan>> GetDueSoonLoans();

        public Task CreateLoanAsync(LoanDto loanDto);

        public Task DeleteLoanAsync(int id);
    }
}
