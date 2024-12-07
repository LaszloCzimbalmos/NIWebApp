using BookLender.Shared.Models;
using BookLender.Shared.Dto;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookLenderAPI.Controllers
{
    [ApiController]
    [Route("api/loans")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Loan loan )
        {
            try
            {
                await _loanService.AddAsync(loan);
                return Ok("Succesfully added loan");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Loan>> Get(int id)
        {
            try
            {
                var loan = await _loanService.GetAsync(id);
                return Ok(loan);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Loan>>> GetAll()
        {
            return Ok(await _loanService.GetAllAsync());
        }

        [HttpGet("list-loans/{name}")]
        public async Task<ActionResult<List<Book>>> Get(string name)
        {
            try
            {
                var rentedBooks = await _loanService.GetRentedBooksForReader(name);
                return Ok(rentedBooks);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("due-soon")]
        public async Task<ActionResult<List<Loan>>> GetDueSoon()
        {
            return Ok(await _loanService.GetDueSoonLoans());
        }

        [HttpGet("late")]
        public async Task<ActionResult<List<Loan>>> GetLate()
        {
            return Ok(await _loanService.GetLateLoans());
        }

        [HttpGet("get-loan/{bookId:int}/{readerId:int}")]
        public async Task<ActionResult<Loan>> Get(int bookId, int readerId)
        {
            try
            {
                return Ok(await _loanService.GetLoanByBookAndReader(bookId, readerId));
            }
            catch (NotSupportedException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("add-loan")]
        public async Task<IActionResult> AddLoan([FromBody] LoanDto loanDto)
        {
            try
            {
                await _loanService.CreateBookLoan(loanDto);
                return Ok("Succesfully created loan");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Loan newLoan)
        {
            try
            {
                await _loanService.UpdateAsync(id, newLoan);
                return Ok("Loan data updated!");
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _loanService.DeleteAsync(id);
                return Ok($"Loan with ID '{id}' is deleted succesfully!");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

    }
}
