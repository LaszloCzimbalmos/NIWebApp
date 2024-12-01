using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BookLender.Shared.Models;
using BookLenderAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace BookLenderAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Book book)
        {
            try
            {
                await _bookService.AddAsync(book);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            try
            {
                var book = await _bookService.GetAsync(id);
                return Ok(book);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Book>>> Get()
        {
            return Ok(await _bookService.GetAllAsync());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            try
            {
                await _bookService.UpdateAsync(id, book);
                return Ok();
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
                await _bookService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
