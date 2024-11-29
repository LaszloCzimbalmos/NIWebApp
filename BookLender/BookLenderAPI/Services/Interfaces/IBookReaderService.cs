using System.Collections.Generic;
using System.Threading.Tasks;
using BookLender.Shared.Models;

namespace BookLenderAPI.Services.Interfaces
{
    public interface IBookReaderService
    {
        Task AddAsync(BookReader bookReader);

        Task<BookReader> GetAsync(int id);

        public Task DeleteAsync(int id);
    }
}
