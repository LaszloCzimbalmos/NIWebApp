using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BookLender.Shared.Dto;
using BookLender.Shared.Models;
using BookLenderAPI.Controllers;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace BookLender.Tests.ControllerTests
{

    public class LoansControllerTests
    {
        private readonly Mock<ILoanService> _loanServiceMock;
        private readonly Mock<ILogger<LoansController>> _loggerMock;
        private readonly LoansController _controller;

        public LoansControllerTests()
        {
            _loanServiceMock = new Mock<ILoanService>();
            _loggerMock = new Mock<ILogger<LoansController>>();
            _controller = new LoansController(_loanServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Add_ValidLoan_ReturnsOk()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, ReaderId = 1, BookId = 1, LoanDate = DateTime.Now, ReturnDueDate = DateTime.Now.AddDays(7) };

            // Act
            var result = await _controller.Add(loan);

            // Assert
            Assert.IsType<OkResult>(result);
            _loanServiceMock.Verify(service => service.AddAsync(loan), Times.Once);
        }

        [Fact]
        public async Task Add_DuplicateLoan_ReturnsBadRequest()
        {
            // Arrange
            var loan = new Loan();
            _loanServiceMock.Setup(service => service.AddAsync(loan)).ThrowsAsync(new AlreadyExistsException("Loan already exists."));

            // Act
            var result = await _controller.Add(loan);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Loan already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task Get_ValidLoanId_ReturnsLoan()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, ReaderId = 1, BookId = 1 };
            _loanServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(loan);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(loan, okResult.Value);
        }

        [Fact]
        public async Task Get_InvalidLoanId_ReturnsNotFound()
        {
            // Arrange
            _loanServiceMock.Setup(service => service.GetAsync(1)).ThrowsAsync(new NotFoundException("Loan not found."));

            // Act
            var result = await _controller.Get(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Loan not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetRentedBooksForReader_ValidReaderName_ReturnsBooks()
        {
            // Arrange
            var books = new List<Book> { new Book { InventoryNumber = 1, Title = "Test Book" } };
            _loanServiceMock.Setup(service => service.GetRentedBooksForReader("John")).ReturnsAsync(books);

            // Act
            var result = await _controller.GetLoanForReaderByName("John");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(books, okResult.Value);
        }

        [Fact]
        public async Task GetDueSoon_ReturnsLoans()
        {
            // Arrange
            var loans = new List<Loan> { new Loan { LoanId = 1 } };
            _loanServiceMock.Setup(service => service.GetDueSoonLoans()).ReturnsAsync(loans);

            // Act
            var result = await _controller.GetDueSoonLoans();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(loans, okResult.Value);
        }

        [Fact]
        public async Task GetLoanByBookAndReader_ValidIds_ReturnsLoan()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, BookId = 1, ReaderId = 1 };
            _loanServiceMock.Setup(service => service.GetLoanByBookAndReader(1, 1)).ReturnsAsync(loan);

            // Act
            var result = await _controller.Get(1, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(loan, okResult.Value);
        }

        [Fact]
        public async Task AddLoan_ValidLoanDto_ReturnsOk()
        {
            // Arrange
            var loanDto = new LoanDto
            {
                ReaderName = "John Doe",
                BookTitle = "Some Book Title",
                Months = 3
            };

            // Act
            var result = await _controller.AddLoan(loanDto);

            // Assert
            Assert.IsType<OkResult>(result);
            _loanServiceMock.Verify(service => service.CreateBookLoan(loanDto), Times.Once);
        }

        [Fact]
        public async Task Update_ValidLoan_ReturnsOk()
        {
            // Arrange
            var loan = new Loan { LoanId = 1, ReaderId = 1, BookId = 1 };

            // Act
            var result = await _controller.Update(1, loan);

            // Assert
            Assert.IsType<OkResult>(result);
            _loanServiceMock.Verify(service => service.UpdateAsync(1, loan), Times.Once);
        }

        [Fact]
        public async Task Delete_ValidLoanId_ReturnsOk()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
            _loanServiceMock.Verify(service => service.DeleteAsync(1), Times.Once);
        }
    }
}
