using BookLender.Shared.Models;
using BookLenderAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
            await _bookReaderService.AddAsync(bookReader);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<BookReader>> Get(int id)
        {
            var bookReader = await _bookReaderService.GetAsync(id);

            if (bookReader is null)
            {
                return NotFound();
            }

            return Ok(bookReader);
        }
    }
}
