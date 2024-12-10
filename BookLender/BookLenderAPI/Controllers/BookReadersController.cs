using BookLender.Shared.Models;
using BookLenderAPI.Exceptions;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLenderAPI.Controllers
{
    [ApiController]
    [Route("api/book-readers")]
    public class BookReadersController : ControllerBase
    {
        private readonly IBookReaderService _bookReaderService;
        private readonly ILogger<BookReadersController> _logger;

        public BookReadersController(IBookReaderService bookReaderService, ILogger<BookReadersController> logger)
        {
            _bookReaderService = bookReaderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BookReader bookReader)
        {
            try
            {
                await _bookReaderService.AddAsync(bookReader);
                _logger.LogInformation("New reader added!");

                return Ok();
            }
            catch (AlreadyExistsException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookReader>> Get(int id)
        {
            try
            {
                var bookReader = await _bookReaderService.GetAsync(id);
                _logger.LogInformation($"Reader retrieved with ID '{id}'");

                return Ok(bookReader);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<BookReader>> Get(string name)
        {
            return Ok(await _bookReaderService.GetByNameAsync(name));
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<BookReader>>> Get()
        {
            return Ok(await _bookReaderService.GetAllAsync());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookReader newBookReader)
        {
            try
            {
                await _bookReaderService.UpdateAsync(newBookReader, id);
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
                var bookReader = await _bookReaderService.GetAsync(id);
                await _bookReaderService.DeleteAsync(id);

                _logger.LogInformation($"Reader deleted with ID '{id}'");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred: {Message}", e.Message);
                return NotFound(e.Message);
            }
        }
    }
}
