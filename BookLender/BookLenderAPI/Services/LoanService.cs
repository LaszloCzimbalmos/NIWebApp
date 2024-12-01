using BookLender.Shared.Models;
using BookLenderAPI.Contexts;

using BookLenderAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLenderAPI.Services
{
    public class LoanService : ILoanService
    {
        private readonly BookLenderContext _dataBase;

        public LoanService(BookLenderContext dataBase)
        {
            _dataBase = dataBase;
        }

        public async Task AddAsync(Loan loan)
        {
            if (await IsExistingLoan(loan))
            {
                throw new Exception($"Error! The loan with the book (ID:'{loan.BookId}') and reader (ID:'{loan.ReaderId}') already exist.");
            }

            await _dataBase.AddAsync(loan);
            await _dataBase.SaveChangesAsync();
        }

        private async Task<bool> IsExistingLoan(Loan loan)
        {
            var checkedLoan = await _dataBase.Loans.FirstOrDefaultAsync(
                l => int.Equals(l.BookId, loan.BookId) &&
                     int.Equals(l.ReaderId, loan.ReaderId)
                );

            return checkedLoan is not null;
        }
        public async Task<Loan> GetAsync(int id)
        {
            var loan = await _dataBase.FindAsync<Loan>(id);

            if (loan is null)
            {
                throw new Exception($"Loan does not exist with ID '{id}'");
            }

            return loan;
        }

        public async Task<List<Loan>> GetAllAsync()
        {
            return await _dataBase.Loans.ToListAsync();
        }

        public async Task CreateBookLoan(string readerName, string bookTitle)
        {

        }

        public Task UpdateAsync(int id, Loan loan)
        {
            throw new System.NotImplementedException();
        }
        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
