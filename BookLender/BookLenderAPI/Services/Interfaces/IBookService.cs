using System.Collections.Generic;
using System.Threading.Tasks;
using BookLender.Shared.Models;

namespace BookLenderAPI.Services.Interfaces
{
    public interface IBookService
    {
        Task AddAsync(Book book);

        Task<Book> GetAsync(int id);

        public Task<Book> GetByTitleAsync(string title);

        public Task<List<Book>> SearchByTitleAsync(string title);

        Task<List<Book>> GetAllAsync();

        public Task UpdateAsync(int id, Book newBook);

        public Task DeleteAsync(int id);
    }
}
