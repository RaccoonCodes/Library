using Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepo _bookRepo;
        public BooksController(IBookRepo bookRepo, ICategoryRepo categoryRepo) => _bookRepo = bookRepo;

        //Get All Books
        [HttpGet]
        public IAsyncEnumerable<Book> GetBooks()
        {
            return _bookRepo.GetAllBooksAsync();
        }

        //Get book by ID
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBook(long id)
        {
            var book = await _bookRepo.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }
            
            return Ok(book);

        }

        //retrives all books based on categoryID
        [HttpGet("GetBooks/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetBooksByCategory(long categoryId)
        {
            
            var books =  _bookRepo.GetBooksByCategoryIdAsync(categoryId);

            if (books == null)
            {
                return NotFound();
            }

            return Ok(books);
        }

        //Update Book
        [HttpPost]
        public async Task<IActionResult> SaveBook(BookBinding target)
        {
            Book book = target.ToBook();
            await _bookRepo.AddBookAsync(book);
            return Ok(book);
        }

        //Create New Book
        [HttpPut]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            await _bookRepo.UpdateBookAsync(book);
            return Ok();
        }


        //Delete Book
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteBook(long id)
        {

            await _bookRepo.DeleteBookAsync(id);
            return NoContent();
        }

        //testing redirect
        [HttpGet("redirect")]
        public IActionResult Redirect()
        {
            return RedirectToAction(nameof(GetBook), new { id = 1 });
        }
    }
}


