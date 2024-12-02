using System.Net.Http.Json;
using BookLender.Shared;
using BookLender.Shared.Models;
using BookLenderUI.Services.Interfaces;

namespace BookLenderUI.Services
{
    public class BookReaderService : IBookReaderService
    {
        private const string BaseEndpointUrl = "api/book-readers";
        private readonly HttpClient _httpClient;

        public BookReaderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookReader> GetReaderAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookReader>($"{BaseEndpointUrl}/{id}");
        }

        public async Task<List<BookReader>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BookReader>>($"{BaseEndpointUrl}/all");
        }

        public async Task UpdateReaderAsync(BookReader newBookReader)
        {
            await _httpClient.PutAsJsonAsync<BookReader>($"{BaseEndpointUrl}/{newBookReader.ReaderId}", newBookReader);
        }

        public async Task DeleteReaderAsync(int id)
        {
            await _httpClient.DeleteAsync($"{BaseEndpointUrl}/{id}");
        }
    }
}
