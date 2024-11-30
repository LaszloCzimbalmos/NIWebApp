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
                throw new Exception($"No reader exist with ID '{id}'");
            }

            return bookReader;
        }

        private async Task<BookReader> GetByNameAsync(string name)
        {
            return await _dataBase.BookReaders
                                 .FirstOrDefaultAsync(reader => reader.Name == name);
        }

        public async Task UpdateAsync(BookReader bookReader, int id)
        {
            if (id != bookReader.ReaderId)
            {
                throw new NotSupportedException($"Error! Required IDs does not match to update: " +
                    $"Updating '{id}' with '{bookReader.ReaderId}'");
            }

            var updateReader = await GetAsync(bookReader.ReaderId);

            if (updateReader is null)
            {
                throw new Exception($"Error! Reader not found with ID '{id}'");
            }

            updateReader.Name = bookReader.Name;
            updateReader.Address = bookReader.Address;
            updateReader.BirthDate = bookReader.BirthDate;

            await _dataBase.SaveChangesAsync();
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
