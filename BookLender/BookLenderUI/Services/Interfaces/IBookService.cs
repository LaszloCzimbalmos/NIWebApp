using BookLender.Shared.Models;

namespace BookLenderUI.Services.Interfaces
{
    public interface IBookService
    {
        public Task AddBookAsync(Book book);

        public Task<List<Book>> GetAllBooksAsync();

        public Task<List<Book>> GetSearchedBooksAsync(string title);

        public Task<Book> GetBookAsync(int id);

        public Task UpdateBookAsync(Book newBook);

        public Task DeleteBookAsync(int id);
    }
}
