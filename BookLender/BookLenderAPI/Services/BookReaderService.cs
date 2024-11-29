using BookLender.Shared.Models;
using BookLenderAPI.Contexts;
using BookLenderAPI.Services.Interfaces;
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
            await _dataBase.AddAsync(bookReader);
            await _dataBase.SaveChangesAsync();
        }

        public async Task<BookReader> GetAsync(int id)
        {
            return await _dataBase.FindAsync<BookReader>(id);
        }
    }
}
