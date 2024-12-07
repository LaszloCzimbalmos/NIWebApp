using BookLender.Shared.Dto;
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

        public async Task<List<Book>> GetRentedBooksForReader(string name)
        {
            return await _httpClient.GetFromJsonAsync<List<Book>>($"{BaseEndpointUrl}/list-loans/{name}");
        }

        public async Task<Loan> GetLoanByBookAndReader(int bookId, int readerId)
        {
            return await _httpClient.GetFromJsonAsync<Loan>($"{BaseEndpointUrl}/get-loan/{bookId}/{readerId}");
        }

        public async Task<List<Loan>> GetDueSoonLoans()
        {
            return await _httpClient.GetFromJsonAsync<List<Loan>>($"{BaseEndpointUrl}/due-soon");
        }

        public async Task<List<Loan>> GetLateLoans()
        {
            return await _httpClient.GetFromJsonAsync<List<Loan>>($"{BaseEndpointUrl}/late");
        }

        public async Task CreateLoanAsync(LoanDto loanDto)
        {
            await _httpClient.PostAsJsonAsync<LoanDto>($"{BaseEndpointUrl}/add-loan", loanDto);
        }

        public async Task DeleteLoanAsync(int id)
        {
            await _httpClient.DeleteAsync($"{BaseEndpointUrl}/{id}");
        }
    }
}
