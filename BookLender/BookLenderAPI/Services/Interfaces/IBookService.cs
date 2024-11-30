using System.Collections.Generic;
using System.Threading.Tasks;
using BookLender.Shared.Models;

namespace BookLenderAPI.Services.Interfaces
{
    public interface IBookService
    {
        Task AddAsync(Book book);

        Task<Book> GetAsync(int id);

        Task<List<Book>> GetAllAsync();

        public Task UpdateAsync(int id, Book newBook);

        public Task DeleteAsync(int id);
    }
}
