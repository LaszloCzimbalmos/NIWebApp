using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BookLender.Shared.Models;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookLenderAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Book book)
        {
            try
            {
                await _bookService.AddAsync(book);
                _logger.LogInformation("Book added successfully!");

                return Ok();
            }
            catch (AlreadyExistsException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            try
            {
                var book = await _bookService.GetAsync(id);
                _logger.LogInformation($"Book retrieved with ID '{id}'");

                return Ok(book);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpGet("{title}")]
        public async Task <ActionResult<List<Book>>> GetSearchedBooks(string title)
        {
            var books = await _bookService.SearchByTitleAsync(title);
            _logger.LogInformation($"All books retrieved that contains '{title}'");

            return Ok(books);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Book>>> GetAllBook()
        {
            var books = await _bookService.GetAllAsync();
            _logger.LogInformation("All books retrieved!");

            return Ok(books);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            try
            {
                await _bookService.UpdateAsync(id, book);
                _logger.LogInformation("Book updated succesfully!");

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
                await _bookService.DeleteAsync(id);
                _logger.LogInformation($"Book deleted with ID '{id}'");

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
