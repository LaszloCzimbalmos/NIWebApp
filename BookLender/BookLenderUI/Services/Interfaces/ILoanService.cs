using BookLender.Shared.Models;

namespace BookLenderUI.Services.Interfaces
{
    public interface ILoanService
    {
        public Task<List<Book>> GetRentedBooksForreader(string name);
    }
}
