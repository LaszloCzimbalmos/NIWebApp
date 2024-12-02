using BookLender.Shared.Models;

namespace BookLenderUI.Services.Interfaces
{
    public interface IBookService
    {
        public Task<List<Book>> GetAllBooksAsync();

        public Task<Book> GetBookAsync(int id);

        public Task UpdateBookAsync(Book newBook);
    }
}
