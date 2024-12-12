using BookLender.Shared.Models;
using BookLenderUI.Services.Interfaces;
using System.Net.Http.Json;

namespace BookLenderUI.Services
{
    public class BookService : IBookService
    {
        private const string BaseEndpointUrl = "api/books";
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddBookAsync(Book book)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpointUrl, book);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }

        public async Task<Book> GetBookAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Book>($"{BaseEndpointUrl}/{id}");
        }

        public async Task<List<Book>> GetSearchedBooksAsync(string title)
        {
            return await _httpClient.GetFromJsonAsync<List<Book>>($"{BaseEndpointUrl}/{title}");
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Book>>($"{BaseEndpointUrl}/all");
        }

        public async Task UpdateBookAsync(Book newBook)
        {
            await _httpClient.PutAsJsonAsync<Book>($"{BaseEndpointUrl}/{newBook.InventoryNumber}", newBook);
        }

        public async Task DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpointUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }
    }
}
