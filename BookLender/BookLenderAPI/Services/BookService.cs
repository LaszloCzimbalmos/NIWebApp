using BookLender.Shared.Models;
using BookLenderAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using BookLenderAPI.Contexts;
using System.Collections.Generic;
using BookLenderAPI.Exceptions;
using System.Linq;

namespace BookLenderAPI.Services
{
    public class BookService : IBookService
    {
        private readonly BookLenderContext _dataBase;

        public BookService(BookLenderContext dataBase)
        {
            _dataBase = dataBase;
        }

        public async Task AddAsync(Book book)
        {
            var checkedBook = await GetByTitleAsync(book.Title);

            if (checkedBook is not null)
            {
                throw new AlreadyExistsException($"Error! Book already exist with title '{book.Title}'");
            }

            await _dataBase.AddAsync(book);
            await _dataBase.SaveChangesAsync();
        }

        public async Task<Book> GetByTitleAsync(string title)
        {
            return await _dataBase.Books.FirstOrDefaultAsync(b => string.Equals(b.Title, title)); 
        }

        public async Task<List<Book>> SearchByTitleAsync(string title)
        {
            return await _dataBase.Books.Where(b => EF.Functions.Like(b.Title.ToLower(), $"%{title.ToLower()}%")).ToListAsync();
        }

        public async Task<Book> GetAsync(int id)
        {
            var book = await _dataBase.FindAsync<Book>(id);

            if (book is null)
            {
                throw new NotFoundException($"Book does not exist with ID '{id}'");
            }

            return book;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _dataBase.Books.ToListAsync();
        }

        public async Task UpdateAsync(int id, Book newBook)
        {
            if (newBook.InventoryNumber != id)
            {
                throw new IdMismatchException($"Error! Required IDs does not match to update: " +
                    $"Updating '{id}' with '{newBook.InventoryNumber}'");
            }

            var updateBook = await GetAsync(id);

            updateBook.Title = newBook.Title;
            updateBook.Author = newBook.Author;
            updateBook.Publisher = newBook.Publisher;
            updateBook.PublicationYear = newBook.PublicationYear;

            await _dataBase.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await GetAsync(id);
            var loansWithDeletedBook = _dataBase.Loans.Where(loan => int.Equals(loan.BookId, book.InventoryNumber)).ToList();

            foreach(var loan in loansWithDeletedBook)
            {
                _dataBase.Remove(loan);
            }

            _dataBase.Remove(book);
            await _dataBase.SaveChangesAsync();
        }
    }
}
