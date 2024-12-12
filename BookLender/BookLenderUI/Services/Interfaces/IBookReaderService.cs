using BookLender.Shared.Models;

namespace BookLenderUI.Services.Interfaces
{
    public interface IBookReaderService
    {
        public Task AddReaderAsync(BookReader bookReader);

        public Task<BookReader> GetReaderAsync(int id);

        public Task<BookReader> GetReaderByNameAsync(string name);

        public Task<List<BookReader>> GetSearchedReadersAsync(string name);

        public Task<List<BookReader>> GetAllAsync();

        public Task UpdateReaderAsync(BookReader newBookReader);

        public Task DeleteReaderAsync(int id);
    }
}
