using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BookLenderAPI.Contexts;
using BookLenderAPI.Services.Interfaces;
using BookLenderAPI.Services;

namespace BookLenderAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IBookReaderService, BookReaderService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ILoanService, LoanService>();

            builder.Services.AddDbContext<BookLenderContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
                options.UseLazyLoadingProxies();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
