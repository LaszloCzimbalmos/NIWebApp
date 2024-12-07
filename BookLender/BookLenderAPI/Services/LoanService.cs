using BookLender.Shared.Models;
using BookLenderAPI.Contexts;
using BookLender.Shared.Dto;
using BookLenderAPI.Services;
using BookLenderAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLenderAPI.Services
{
    public class LoanService : ILoanService
    {
        private readonly BookLenderContext _dataBase;
        private readonly IBookReaderService _bookReaderService;
        private readonly IBookService _bookService;

        public LoanService(
            BookLenderContext dataBase,
            IBookReaderService bookReaderService,
            IBookService bookService)
        {
            _dataBase = dataBase;
            _bookReaderService = bookReaderService;
            _bookService = bookService;
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

        public async Task<List<Book>> GetRentedBooksForReader(string readerName)
        {
            var booksForReader = new List<Book>();

            var loansForReader = await GetLoansForReader(readerName);
            var bookIds = loansForReader.Select(loan => loan.BookId);

            foreach (var id in bookIds)
            {
                booksForReader.Add(await _bookService.GetAsync(id));
            }

            return booksForReader;
        }

        public async Task<List<Loan>> GetLoansForReader (string readerName)
        {
            var reader = await _bookReaderService.GetByNameAsync(readerName);

            return await _dataBase.Loans.Where(loan => int.Equals(loan.ReaderId, reader.ReaderId)).ToListAsync();
        }

        public async Task CreateBookLoan(LoanDto loanDto)
        {
            var book = await _bookService.GetByTitleAsync(loanDto.BookTitle);
            var reader = await _bookReaderService.GetByNameAsync(loanDto.ReaderName);

            if (book is null)
            {
                throw new Exception($"Book does not exist with title '{loanDto.BookTitle}'");
            }
            if (reader is null)
            {
                throw new Exception($"Reader does not exist with name '{loanDto.ReaderName}'");
            }

            var todayDate = DateTime.Now;
            var loanDueDate = todayDate.AddMonths(loanDto.Months);

            var loan = new Loan(reader.ReaderId, book.InventoryNumber, todayDate, loanDueDate);

            await AddAsync(loan);
        }

        public async Task<Loan> GetLoanByBookAndReader(int bookId, int readerId)
        {
            var loan = await _dataBase.Loans.Where(loan => int.Equals(loan.BookId, bookId) &&
                                                           int.Equals(loan.ReaderId, readerId)).FirstOrDefaultAsync();
            if (loan is null)
            {
                throw new NotSupportedException("Loan does not exist");
            }

            return loan;
        }

        public async Task<List<Loan>> GetDueSoonLoans()
        {
            return (await _dataBase.Loans.ToListAsync())
                .Where(loan => CalculateRemainingRentDays(loan.ReturnDueDate) <= 3 &&
                               CalculateRemainingRentDays(loan.ReturnDueDate) >= 0)
                .ToList();
        }

        public async Task<List<Loan>> GetLateLoans()
        {
            return (await _dataBase.Loans.ToListAsync())
                .Where(loan => CalculateRemainingRentDays(loan.ReturnDueDate) < 0)
                .ToList();
        }

        private int CalculateRemainingRentDays(DateTime dueDate)
        {
            return (dueDate - DateTime.Now).Days;
        }

        public async Task UpdateAsync(int id, Loan loan)
        {
            if (id != loan.LoanId)
            {
                throw new NotSupportedException($"Error! Required IDs does not match to update: " +
                    $"Updating '{id}' with '{loan.LoanId}'");
            }

            var updateloan = await GetAsync(id);

            updateloan.BookId = loan.BookId;
            updateloan.ReaderId = loan.ReaderId;
            updateloan.LoanDate = loan.LoanDate;
            updateloan.ReturnDueDate = loan.ReturnDueDate;

            await _dataBase.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var loan = await GetAsync(id);

            _dataBase.Remove(loan);
            await _dataBase.SaveChangesAsync();
        }
    }
}
