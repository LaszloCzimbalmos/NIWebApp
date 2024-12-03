using BookLender.Shared.Models;

namespace BookLenderUI.Services.Interfaces
{
    public interface IBookReaderService
    {
        public Task AddReaderAsync(BookReader bookReader);

        public Task<BookReader> GetReaderAsync(int id);

        public Task<List<BookReader>> GetAllAsync();

        public Task UpdateReaderAsync(BookReader newBookReader);

        public Task DeleteReaderAsync(int id);
    }
}
