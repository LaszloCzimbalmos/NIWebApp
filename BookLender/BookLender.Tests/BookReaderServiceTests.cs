using Moq;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using BookLenderAPI.Contexts;
using BookLenderAPI.Services;
using BookLenderAPI.Exceptions;
using BookLender.Shared.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLender.Tests
{
    public class BookReaderServiceTests : IDisposable
    {
        private readonly Mock<ILogger<BookReaderService>> _mockLogger;
        private readonly BookLenderContext _context;
        private readonly BookReaderService _service;
        private static DbContextOptions<BookLenderContext> _dbContextOptions;

        public BookReaderServiceTests()
        {
            _dbContextOptions ??= new DbContextOptionsBuilder<BookLenderContext>()
                    .UseInMemoryDatabase(databaseName: "BookLenderTestDb")
                    .Options;

            _context = new BookLenderContext(_dbContextOptions);
            _mockLogger = new Mock<ILogger<BookReaderService>>();

            if (!_context.BookReaders.Any())
            {
                _context.BookReaders.AddRange(new List<BookReader>
                {
                    new() { ReaderId = 1, Name = "John Doe", Address = "123 Main St", BirthDate = new DateTime(2001, 01, 01) },
                    new() { ReaderId = 2, Name = "Test Laci", Address = "NewYork Main St", BirthDate = new DateTime(2003, 01, 01) },
                });
                _context.SaveChanges();
            }

            _service = new BookReaderService(_context, _mockLogger.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_ThrowsException_WhenReaderAlreadyExists()
        {
            // Arrange
            var reader = new BookReader { Name = "John Doe", Address = "test street", BirthDate = new DateTime(2001, 01, 01) };
            await _context.BookReaders.AddAsync(reader);
            await _context.SaveChangesAsync();

            var service = new BookReaderService(_context, _mockLogger.Object);

            var newReader = reader;

            // Act & Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() => service.AddAsync(newReader));
        }

        [Fact]
        public async Task AddAsync_AddsReader_WhenReaderDoesNotExist()
        {
            // Arrange
            var newReader = new BookReader { ReaderId = 3, Name = "Jane Doe", Address = "456 Another St", BirthDate = DateTime.Now.AddYears(-20) };

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
            await _service.DeleteAsync(2);
            var deletedReader = await _service.GetByNameAsync("Test Laci");

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
            Assert.NotNull(result.Where(br => string.Equals("John Doe", br.Name)));
        }
    }
}
