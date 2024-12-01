using BookLender.Shared.Models;
using BookLenderAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLenderAPI.Services.Interfaces
{
    public interface ILoanService
    {
        Task AddAsync(Loan loan);

        Task<Loan> GetAsync(int id);

        Task<List<Loan>> GetAllAsync();

        public Task<List<Book>> GetRentedBooksForReader(string readerName);

        public Task CreateBookLoan(LoanDto loanDto);

        public Task UpdateAsync(int id, Loan loan);

        public Task DeleteAsync(int id);
    }
}
