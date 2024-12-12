using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLender.Shared.Models;
using BookLenderAPI.Controllers;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BookLender.Tests.ControllerTests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly Mock<ILogger<BooksController>> _mockLogger;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _mockLogger = new Mock<ILogger<BooksController>>();
            _controller = new BooksController(_mockBookService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Add_ReturnsOk_WhenBookIsAddedSuccessfully()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "Test Author",
                Publisher = "Test Publisher",
                PublicationYear = 2023
            };

            _mockBookService.Setup(service => service.AddAsync(newBook)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(newBook);

            // Assert
            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task Add_ReturnsBadRequest_WhenBookAlreadyExists()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "Test Author",
                Publisher = "Test Publisher",
                PublicationYear = 2023
            };

            _mockBookService.Setup(service => service.AddAsync(newBook)).ThrowsAsync(new AlreadyExistsException("Book already exists"));

            // Act
            var result = await _controller.Add(newBook);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("Book already exists", actionResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenBookExists()
        {
            // Arrange
            var book = new Book
            {
                InventoryNumber = 1,
                Title = "Test Book",
                Author = "Test Author",
                Publisher = "Test Publisher",
                PublicationYear = 2023
            };

            _mockBookService.Setup(service => service.GetAsync(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Equal(book, actionResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetAsync(99)).ThrowsAsync(new NotFoundException("Book not found"));

            // Act
            var result = await _controller.Get(99);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, actionResult.StatusCode);
            Assert.Equal("Book not found", actionResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithListOfBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new() { InventoryNumber = 1, Title = "Book 1", Author = "Author 1", Publisher = "Publisher 1", PublicationYear = 2020 },
                new() { InventoryNumber = 2, Title = "Book 2", Author = "Author 2", Publisher = "Publisher 2", PublicationYear = 2021 }
            };

            _mockBookService.Setup(service => service.GetAllAsync()).ReturnsAsync(books);

            // Act
            var result = await _controller.Get();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
            Assert.Equal(books, actionResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenBookIsUpdatedSuccessfully()
        {
            // Arrange
            var updatedBook = new Book
            {
                InventoryNumber = 1,
                Title = "Updated Book",
                Author = "Updated Author",
                Publisher = "Updated Publisher",
                PublicationYear = 2022
            };

            _mockBookService.Setup(service => service.UpdateAsync(1, updatedBook)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, updatedBook);

            // Assert
            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var updatedBook = new Book
            {
                InventoryNumber = 2,
                Title = "Updated Book",
                Author = "Updated Author",
                Publisher = "Updated Publisher",
                PublicationYear = 2022
            };

            _mockBookService.Setup(service => service.UpdateAsync(1, updatedBook)).ThrowsAsync(new IdMismatchException("ID mismatch"));

            // Act
            var result = await _controller.Update(1, updatedBook);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            Assert.Equal("ID mismatch", actionResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenBookIsDeletedSuccessfully()
        {
            // Arrange
            _mockBookService.Setup(service => service.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var actionResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, actionResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookService.Setup(service => service.DeleteAsync(99)).ThrowsAsync(new NotFoundException("Book not found"));

            // Act
            var result = await _controller.Delete(99);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, actionResult.StatusCode);
            Assert.Equal("Book not found", actionResult.Value);
        }
    }
}