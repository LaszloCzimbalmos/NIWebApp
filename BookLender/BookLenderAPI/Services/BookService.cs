using BookLender.Shared.Models;
using BookLenderAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using BookLenderAPI.Contexts;
using System.Collections.Generic;

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
                throw new Exception("Error! Book already exist. Try other title.");
            }

            await _dataBase.AddAsync(book);
            await _dataBase.SaveChangesAsync();
        }

        private async Task<Book> GetByTitleAsync(string title)
        {
            return await _dataBase.Books.FirstOrDefaultAsync(b => string.Equals(b.Title, title)); 
        }
        public async Task<Book> GetAsync(int id)
        {
            var book = await _dataBase.FindAsync<Book>(id);

            if (book is null)
            {
                throw new Exception($"Book does not exist with ID '{id}'");
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
                throw new NotSupportedException($"Error! Required IDs does not match to update: " +
                    $"Updating '{id}' with '{newBook.InventoryNumber}'");
            }

            var updateBook = await GetAsync(id);

            updateBook.Title = newBook.Title;
            updateBook.Author = newBook.Author;
            updateBook.Publisher = newBook.Publisher;
            updateBook.PublicationYear = newBook.PublicationYear;

            await _dataBase.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
