using BookLender.Shared.Models;
using BookLenderAPI.Contexts;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BookLenderAPI.Services
{
    public class BookReaderService : IBookReaderService
    {
        private readonly BookLenderContext _dataBase;

        public BookReaderService(BookLenderContext dataBase)
        {
            _dataBase = dataBase;
        }

        public async Task AddAsync(BookReader bookReader)
        {
            var checkedBookReader = await GetByNameAsync(bookReader.Name);

            if (checkedBookReader is not null)
            {
                throw new Exception("Reader already exist. Try other name.");
            }

            await _dataBase.AddAsync(bookReader);
            await _dataBase.SaveChangesAsync();
        }

        public async Task<BookReader> GetAsync(int id)
        {
            var bookReader = await _dataBase.FindAsync<BookReader>(id);

            if (bookReader is null)
            {
                throw new Exception($"No reader with ID '{id}'");
            }

            return bookReader;
        }

        public async Task<BookReader> GetByNameAsync(string name)
        {
            return await _dataBase.BookReaders
                                 .FirstOrDefaultAsync(reader => reader.Name == name);
        }

        public async Task DeleteAsync(int id)
        {
            var checkedBookreader = await _dataBase.FindAsync<BookReader>(id);

            if (checkedBookreader is null)
            {
                throw new Exception("Error! Reader does not exist.");
            }

            _dataBase.Remove(checkedBookreader);
            await _dataBase.SaveChangesAsync();
        }
    }
}
