using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLender.Shared.Models;
using BookLenderAPI.Contexts;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace BookLender.Tests
{
    public class BookServiceTests : IDisposable
    {
        private readonly BookLenderContext _context;
        private readonly BookService _service;

        public BookServiceTests()
        {
            var options = new DbContextOptionsBuilder<BookLenderContext>()
                .UseInMemoryDatabase(databaseName: "BookLenderTestDb2")
                .Options;

            _context = new BookLenderContext(options);

            if (!_context.Books.Any())
            {
                _context.Books.AddRange(new List<Book>
                {
                    new Book { InventoryNumber = 1, Title = "Book One", Author = "Author One", Publisher = "Publisher One", PublicationYear = 2001 },
                    new Book { InventoryNumber = 2, Title = "Book Two", Author = "Author Two", Publisher = "Publisher Two", PublicationYear = 2002 },
                });
                _context.SaveChanges();
            }

            _service = new BookService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_ThrowsException_WhenBookAlreadyExists()
        {
            // Arrange
            var book = new Book { Title = "Book One", Author = "New Author", Publisher = "New Publisher", PublicationYear = 2023 };

            // Act & Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() => _service.AddAsync(book));
        }

        [Fact]
        public async Task AddAsync_AddsBook_WhenBookDoesNotExist()
        {
            // Arrange
            var book = new Book { Title = "New Book", Author = "New Author", Publisher = "New Publisher", PublicationYear = 2023 };

            // Act
            await _service.AddAsync(book);

            // Assert
            var addedBook = await _service.GetByTitleAsync("New Book");
            Assert.NotNull(addedBook);
            Assert.Equal("New Author", addedBook.Author);
            Assert.Equal("New Publisher", addedBook.Publisher);
            Assert.Equal(2023, addedBook.PublicationYear);
        }

        [Fact]
        public async Task GetAsync_ThrowsException_WhenBookNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAsync(99));
        }

        [Fact]
        public async Task GetAsync_ReturnsBook_WhenBookExists()
        {
            // Act
            var book = await _service.GetAsync(1);

            // Assert
            Assert.NotNull(book);
            Assert.Equal("Book One", book.Title);
            Assert.Equal("Author One", book.Author);
            Assert.Equal("Publisher One", book.Publisher);
            Assert.Equal(2001, book.PublicationYear);
        }

        [Fact]
        public async Task GetByTitleAsync_ReturnsNull_WhenBookNotFound()
        {
            // Act
            var book = await _service.GetByTitleAsync("Nonexistent Book");

            // Assert
            Assert.Null(book);
        }

        [Fact]
        public async Task GetByTitleAsync_ReturnsBook_WhenBookExists()
        {
            // Act
            var book = await _service.GetByTitleAsync("Book One");

            // Assert
            Assert.NotNull(book);
            Assert.Equal(1, book.InventoryNumber);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllBooks()
        {
            // Act
            var books = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenIdsDoNotMatch()
        {
            // Arrange
            var newBook = new Book { InventoryNumber = 3, Title = "Updated Book", Author = "Updated Author", Publisher = "Updated Publisher", PublicationYear = 2023 };

            // Act & Assert
            await Assert.ThrowsAsync<IdMismatchException>(() => _service.UpdateAsync(1, newBook));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesBook_WhenIdsMatch()
        {
            // Arrange
            var newBook = new Book { InventoryNumber = 1, Title = "Updated Book", Author = "Updated Author", Publisher = "Updated Publisher", PublicationYear = 2023 };

            // Act
            await _service.UpdateAsync(1, newBook);

            var updatedBook = await _service.GetAsync(1);

            // Assert
            Assert.Equal("Updated Book", updatedBook.Title);
            Assert.Equal("Updated Author", updatedBook.Author);
            Assert.Equal("Updated Publisher", updatedBook.Publisher);
            Assert.Equal(2023, updatedBook.PublicationYear);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenBookNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_DeletesBook_WhenBookExists()
        {
            // Act
            await _service.DeleteAsync(1);

            // Assert
            var deletedBook = await _service.GetByTitleAsync("Book One");
            Assert.Null(deletedBook);
        }
    }
}
