using BookLender.Shared.Models;
using BookLender.Shared.Dto;
using BookLenderAPI.Contexts;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services;
using BookLenderAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookLender.Tests
{
    public class LoanServiceTests
    {
        private readonly LoanService _loanService;
        private readonly Mock<IBookReaderService> _mockBookReaderService;
        private readonly Mock<IBookService> _mockBookService;
        private readonly BookLenderContext _context;

        public LoanServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookLenderContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BookLenderContext(options);
            _mockBookReaderService = new Mock<IBookReaderService>();
            _mockBookService = new Mock<IBookService>();

            _loanService = new LoanService(_context, _mockBookReaderService.Object, _mockBookService.Object);
        }

        [Fact]
        public async Task AddAsync_ThrowsException_WhenLoanAlreadyExists()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() => _loanService.AddAsync(loan));
        }

        [Fact]
        public async Task AddAsync_AddsLoan_WhenLoanDoesNotExist()
        {
            // Arrange
            var loan = new Loan() { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };

            // Act
            await _loanService.AddAsync(loan);

            // Assert
            var addedLoan = await _context.Loans.FindAsync(loan.LoanId);
            Assert.NotNull(addedLoan);
        }

        [Fact]
        public async Task GetAsync_ThrowsException_WhenLoanNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _loanService.GetAsync(99));
        }

        [Fact]
        public async Task GetAsync_ReturnsLoan_WhenLoanExists()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            var result = await _loanService.GetAsync(loan.LoanId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(loan.LoanId, result.LoanId);
        }

        [Fact]
        public async Task GetRentedBooksForReader_ReturnsBooks_WhenLoansExist()
        {
            // Arrange
            var readerName = "John Doe";
            var reader = new BookReader { ReaderId = 1, Name = readerName };
            var book = new Book { InventoryNumber = 1, Title = "Book Title" };
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };

            _mockBookReaderService.Setup(s => s.GetByNameAsync(readerName)).ReturnsAsync(reader);
            _mockBookService.Setup(s => s.GetAsync(book.InventoryNumber)).ReturnsAsync(book);

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            var result = await _loanService.GetRentedBooksForReader(readerName);

            // Assert
            Assert.Single(result);
            Assert.Equal(book.Title, result.First().Title);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenIdsDoNotMatch()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };

            // Act & Assert
            await Assert.ThrowsAsync<IdMismatchException>(() => _loanService.UpdateAsync(2, loan));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesLoan_WhenIdsMatch()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            var updatedLoan = new Loan { LoanId = 1, BookId = 2, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(60) };

            // Act
            await _loanService.UpdateAsync(loan.LoanId, updatedLoan);

            // Assert
            var result = await _loanService.GetAsync(loan.LoanId);
            Assert.Equal(2, result.BookId);
            Assert.Equal(updatedLoan.ReturnDueDate, result.ReturnDueDate);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenLoanNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _loanService.DeleteAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_DeletesLoan_WhenLoanExists()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(30) };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            await _loanService.DeleteAsync(loan.LoanId);

            // Assert
            var deletedLoan = await _context.Loans.FindAsync(loan.LoanId);
            Assert.Null(deletedLoan);
        }

        [Fact]
        public async Task GetDueSoonLoans_ReturnsLoans_WithDueDateWithinThreeDays()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(2) };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            var result = await _loanService.GetDueSoonLoans();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetLateLoans_ReturnsLoans_WithPastDueDate()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1, LoanDate = DateTime.Now.AddDays(-10), ReturnDueDate = DateTime.Now.AddDays(-1) };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            var result = await _loanService.GetLateLoans();

            // Assert
            Assert.Single(result);
        }
    }
}
