using BookLender.Shared.Models;
using BookLenderUI.Services.Interfaces;
using System.Net.Http.Json;

namespace BookLenderUI.Services
{
    public class LoanService : ILoanService
    {
        private const string BaseEndpointUrl = "api/loans";
        private readonly HttpClient _httpClient;

        public LoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Book>> GetRentedBooksForreader(string name)
        {
            return await _httpClient.GetFromJsonAsync<List<Book>>($"{BaseEndpointUrl}/list-loans/{name}");
        }
    }
}
