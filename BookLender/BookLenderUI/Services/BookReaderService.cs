using System.Net.Http.Json;

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

        public async Task AddReaderAsync(BookReader bookReader)
        {
           var response = await _httpClient.PostAsJsonAsync(BaseEndpointUrl, bookReader);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }

        public async Task<BookReader> GetReaderAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookReader>($"{BaseEndpointUrl}/{id}");
        }

        public async Task<BookReader> GetReaderByNameAsync(string name)
        {
            return await _httpClient.GetFromJsonAsync<BookReader>($"{BaseEndpointUrl}/{name}");
        }

        public async Task<List<BookReader>> GetSearchedReadersAsync(string name)
        {
            return await _httpClient.GetFromJsonAsync<List<BookReader>>($"{BaseEndpointUrl}/search/{name}");
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
            var response = await _httpClient.DeleteAsync($"{BaseEndpointUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
        }
    }
}
