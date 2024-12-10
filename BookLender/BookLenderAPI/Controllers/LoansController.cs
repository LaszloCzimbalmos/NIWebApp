using BookLender.Shared.Models;
using BookLender.Shared.Dto;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using BookLenderAPI.Exceptions;
using Microsoft.Extensions.Logging;

namespace BookLenderAPI.Controllers
{
    [ApiController]
    [Route("api/loans")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoansController> _logger;
        public LoansController(ILoanService loanService, ILogger<LoansController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Loan loan )
        {
            try
            {
                await _loanService.AddAsync(loan);
                _logger.LogInformation("Loan added successfully!");

                return Ok();
            }
            catch (AlreadyExistsException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Loan>> Get(int id)
        {
            try
            {
                var loan = await _loanService.GetAsync(id);
                _logger.LogInformation($"Loan retrieved with ID {id}");

                return Ok(loan);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpGet("list-loans/{name}")]
        public async Task<ActionResult<List<Book>>> Get(string name)
        {
            try
            {
                var rentedBooks = await _loanService.GetRentedBooksForReader(name);
                _logger.LogInformation($"Rented books retrieved for '{name}'");

                return Ok(rentedBooks);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpGet("due-soon")]
        public async Task<ActionResult<List<Loan>>> GetDueSoon()
        {
            _logger.LogInformation("Due soon rents are retrieved!");
            return Ok(await _loanService.GetDueSoonLoans());
        }

        [HttpGet("late")]
        public async Task<ActionResult<List<Loan>>> GetLate()
        {
            _logger.LogInformation("Late rents are retrieved!");
            return Ok(await _loanService.GetLateLoans());
        }

        [HttpGet("get-loan/{bookId:int}/{readerId:int}")]
        public async Task<ActionResult<Loan>> Get(int bookId, int readerId)
        {
            try
            {
                _logger.LogInformation($"Retrieved loan with BookID: {bookId}, ReaderID: {readerId}");
                return Ok(await _loanService.GetLoanByBookAndReader(bookId, readerId));
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpPost("add-loan")]
        public async Task<IActionResult> AddLoan([FromBody] LoanDto loanDto)
        {
            try
            {
                await _loanService.CreateBookLoan(loanDto);
                _logger.LogInformation("Loan is created successfully!");

                return Ok();
            }
            catch (AlreadyExistsException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Loan newLoan)
        {
            try
            {
                await _loanService.UpdateAsync(id, newLoan);
                _logger.LogInformation("Loan is updated successfully!");

                return Ok();
            }
            catch (IdMismatchException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _loanService.DeleteAsync(id);
                _logger.LogInformation($"Loan with ID '{id}' is deleted succesfully!");

                return Ok();
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

    }
}
