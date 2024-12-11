using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLender.Shared.Models;
using BookLenderAPI.Contexts;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLender.Tests
{
    public class BookReaderServiceTests
    {
        private readonly Mock<ILogger<BookReaderService>> _mockLogger;
        private readonly BookReaderService _service;

        public BookReaderServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookLenderContext>()
                .UseInMemoryDatabase(databaseName: "BookLenderTestDb")
                .Options;

            var context = new BookLenderContext(options);
            _mockLogger = new Mock<ILogger<BookReaderService>>();

            context.BookReaders.AddRange(new List<BookReader>
            {
                new BookReader { ReaderId = 1, Name = "John Doe", Address = "123 Main St", BirthDate = new DateTime(2001, 01, 01) },
                new BookReader { ReaderId = 2, Name = "Test Laci", Address = "NewYork Main St", BirthDate = new DateTime(2003, 01, 01) }
            });
            context.SaveChanges();

            _service = new BookReaderService(context, _mockLogger.Object);
        }


        [Fact]
        public async Task AddAsync_ThrowsException_WhenReaderAlreadyExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookLenderContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new BookLenderContext(options);
            var reader = new BookReader { Name = "John Doe", Address = "test street", BirthDate = new DateTime(2001, 01, 01) };
            context.BookReaders.Add(reader);
            await context.SaveChangesAsync();

            var logger = Mock.Of<ILogger<BookReaderService>>();
            var service = new BookReaderService(context, logger);

            var newReader = reader;

            // Act & Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() => service.AddAsync(newReader));
        }


        [Fact]
        public async Task AddAsync_AddsReader_WhenReaderDoesNotExist()
        {
            // Arrange
            var newReader = new BookReader { Name = "Jane Doe", Address = "456 Another St", BirthDate = DateTime.Now.AddYears(-20) };

            // Act
            await _service.AddAsync(newReader);

            // Assert
            var addedReader = await _service.GetByNameAsync("Jane Doe");
            Assert.NotNull(addedReader);
            Assert.Equal("456 Another St", addedReader.Address);
        }

        [Fact]
        public async Task GetAsync_ThrowsException_WhenReaderNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAsync(99));
        }

        [Fact]
        public async Task GetAsync_ReturnsReader_WhenReaderExists()
        {
            // Arrange
            var existingReader = new BookReader { ReaderId = 1, Name = "John Doe", Address = "123 Main St", BirthDate = new DateTime(2001, 01, 01) };

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Equal(existingReader.Name, result.Name);
            Assert.Equal(existingReader.Address, result.Address);
            Assert.Equal(existingReader.BirthDate, result.BirthDate);
            Assert.Equal(existingReader.ReaderId, result.ReaderId);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenIdsDoNotMatch()
        {
            // Arrange
            var newReader = new BookReader { Name = "John Doe", Address = "test street", BirthDate = new DateTime(2001, 01, 01) };

            // Act & Assert
            await Assert.ThrowsAsync<IdMismatchException>(() => _service.UpdateAsync(newReader, 1));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesReader_WhenIdsMatch()
        {
            // Arrange
            var newReader = new BookReader { ReaderId = 1, Name = "James Doe", Address = "New Address", BirthDate = new DateTime(1999, 01, 01) };

            // Act
            await _service.UpdateAsync(newReader, 1);

            var updatedReader = await _service.GetByNameAsync("James Doe");

            // Assert
            Assert.Equal("James Doe", updatedReader.Name);
            Assert.Equal("New Address", updatedReader.Address);
            Assert.Equal(new DateTime(1999, 01, 01), updatedReader.BirthDate.Date);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenReaderNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_DeletesReader_WhenReaderExists()
        {
            // Act
            await _service.DeleteAsync(1);
            var deletedReader = await _service.GetByNameAsync("John Doe");

            // Assert
            Assert.Null(deletedReader);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllReaders()
        {
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}