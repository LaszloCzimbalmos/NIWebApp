using BookLender.Shared.Models;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
