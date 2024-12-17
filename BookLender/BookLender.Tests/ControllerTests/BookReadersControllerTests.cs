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

namespace BookLender.Tests.ControllerTests
{
    public class BookReadersControllerTests
    {
        private readonly Mock<IBookReaderService> _mockService;
        private readonly Mock<ILogger<BookReadersController>> _mockLogger;
        private readonly BookReadersController _controller;

        public BookReadersControllerTests()
        {
            _mockService = new Mock<IBookReaderService>();
            _mockLogger = new Mock<ILogger<BookReadersController>>();
            _controller = new BookReadersController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Add_ReturnsOk_WhenBookReaderAddedSuccessfully()
        {
            // Arrange
            var newReader = new BookReader { Name = "Jane Doe", Address = "456 Another St", BirthDate = DateTime.Now.AddYears(-20) };

            _mockService.Setup(s => s.AddAsync(newReader)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(newReader);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockService.Verify(s => s.AddAsync(newReader), Times.Once);
        }

        [Fact]
        public async Task Add_ReturnsBadRequest_WhenAlreadyExistsExceptionThrown()
        {
            // Arrange
            var newReader = new BookReader { Name = "Jane Doe", Address = "456 Another St", BirthDate = DateTime.Now.AddYears(-20) };

            _mockService.Setup(s => s.AddAsync(newReader))
                .ThrowsAsync(new AlreadyExistsException("Reader already exists"));

            // Act
            var result = await _controller.Add(newReader);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Reader already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsBookReader_WhenReaderExists()
        {
            // Arrange
            var reader = new BookReader { ReaderId = 1, Name = "John Doe", Address = "123 Main St", BirthDate = DateTime.Now.AddYears(-30) };

            _mockService.Setup(s => s.GetAsync(1)).ReturnsAsync(reader);

            // Act
            var result = await _controller.GetReader(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReader = Assert.IsType<BookReader>(okResult.Value);
            Assert.Equal(reader.Name, returnedReader.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenReaderDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync(99))
                .ThrowsAsync(new NotFoundException("Reader not found"));

            // Act
            var result = await _controller.GetReader(99);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Reader not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetByName_ReturnsBookReader_WhenReaderExists()
        {
            // Arrange
            var reader = new BookReader { ReaderId = 1, Name = "John Doe", Address = "123 Main St", BirthDate = DateTime.Now.AddYears(-30) };

            _mockService.Setup(s => s.GetByNameAsync("John Doe")).ReturnsAsync(reader);

            // Act
            var result = await _controller.GetReaderByName("John Doe");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReader = Assert.IsType<BookReader>(okResult.Value);
            Assert.Equal(reader.Name, returnedReader.Name);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfReaders()
        {
            // Arrange
            var readers = new List<BookReader>
            {
                new() { ReaderId = 1, Name = "John Doe", Address = "123 Main St" },
                new() { ReaderId = 2, Name = "Jane Doe", Address = "456 Another St" }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(readers);

            // Act
            var result = await _controller.GetAllReader();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReaders = Assert.IsType<List<BookReader>>(okResult.Value);
            Assert.Equal(2, returnedReaders.Count);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenReaderUpdatedSuccessfully()
        {
            // Arrange
            var updatedReader = new BookReader { ReaderId = 1, Name = "Updated Name", Address = "Updated Address" };

            _mockService.Setup(s => s.UpdateAsync(updatedReader, 1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, updatedReader);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockService.Verify(s => s.UpdateAsync(updatedReader, 1), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatchExceptionThrown()
        {
            // Arrange
            var updatedReader = new BookReader { ReaderId = 2, Name = "Updated Name", Address = "Updated Address" };

            _mockService.Setup(s => s.UpdateAsync(updatedReader, 1))
                .ThrowsAsync(new IdMismatchException("ID mismatch"));

            // Act
            var result = await _controller.Update(1, updatedReader);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenReaderDeletedSuccessfully()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync(1)).ReturnsAsync(new BookReader());
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenReaderDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync(99))
                .ThrowsAsync(new NotFoundException("Reader not found"));

            // Act
            var result = await _controller.Delete(99);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Reader not found", notFoundResult.Value);
        }
    }
}
