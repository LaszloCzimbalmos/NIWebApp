using BookLender.Shared.Models;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BookLenderAPI.Controllers
{
    [ApiController]
    [Route("BookReader")]
    public class BookReadersController : ControllerBase
    {
        private readonly IBookReaderService _bookReaderService;

        public BookReadersController(IBookReaderService bookReaderService)
        {
            _bookReaderService = bookReaderService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BookReader bookReader)
        {
            try
            {
                await _bookReaderService.AddAsync(bookReader);
                return Ok();
            }
            catch (Exception e) // TODO: custom exception
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookReader>> Get(int id)
        {
            try
            {
                var bookReader = await _bookReaderService.GetAsync(id);
                return Ok(bookReader);
            }
            catch (Exception e) // TODO: custom exception
            {
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

                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
