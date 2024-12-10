using BookLender.Shared.Models;
using BookLenderAPI.Contexts;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLenderAPI.Services
{
    public class BookReaderService : IBookReaderService
    {
        private readonly BookLenderContext _dataBase;
        private readonly ILogger<BookReaderService> _logger;

        public BookReaderService(BookLenderContext dataBase, ILogger<BookReaderService> logger)
        {
            _dataBase = dataBase;
            _logger = logger;
        }

        public async Task AddAsync(BookReader bookReader)
        {
            var checkedBookReader = await GetByNameAsync(bookReader.Name);

            if (checkedBookReader is not null)
            {
                throw new AlreadyExistsException($"Reader already exist with name '{bookReader.Name}'.");
            }

            _logger.LogInformation($"Adding new reader: {bookReader}");

            await _dataBase.AddAsync(bookReader);
            await _dataBase.SaveChangesAsync();
        }

        public async Task<BookReader> GetAsync(int id)
        {
            var bookReader = await _dataBase.FindAsync<BookReader>(id);

            if (bookReader is null)
            {
                throw new NotFoundException($"No reader exist with ID '{id}'");
            }

            return bookReader;
        }

        public async Task<BookReader> GetByNameAsync(string name)
        {
            return await _dataBase.BookReaders
                                 .FirstOrDefaultAsync(r => string.Equals(r.Name, name));
        }

        public async Task UpdateAsync(BookReader newBookReader, int id)
        {
            if (id != newBookReader.ReaderId)
            {
                throw new IdMismatchException($"Error! Required IDs does not match to update: " +
                    $"Updating '{id}' with '{newBookReader.ReaderId}'");
            }

            var updateReader = await GetAsync(newBookReader.ReaderId);

            updateReader.Name = newBookReader.Name;
            updateReader.Address = newBookReader.Address;
            updateReader.BirthDate = newBookReader.BirthDate;

            await _dataBase.SaveChangesAsync();
        }

        public async Task<List<BookReader>> GetAllAsync()
        {
            _logger.LogInformation("All readers retrieved.");
            return await _dataBase.BookReaders.ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var bookReader = await GetAsync(id);

            //check if there is no loan for this reader

            _dataBase.Remove(bookReader);
            await _dataBase.SaveChangesAsync();
        }
    }
}
