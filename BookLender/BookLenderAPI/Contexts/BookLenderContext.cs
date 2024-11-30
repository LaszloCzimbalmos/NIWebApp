using BookLender.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLenderAPI.Contexts
{
    public class BookLenderContext : DbContext
    {
        public BookLenderContext(DbContextOptions<BookLenderContext> options) : base(options)
        {
        }

        public virtual DbSet<BookReader> BookReaders { get; set; }

        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<Loan> Loans { get; set; }
    }
}
