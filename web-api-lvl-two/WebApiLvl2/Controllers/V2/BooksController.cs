using Microsoft.AspNetCore.Mvc;
using WebApiLvl2.Constants;
using WebApiLvl2.Helpers;
using WebApiLvl2.Models;
using WebApiLvl2.Repositories;


namespace WebApiLvl2.Controllers.V2;

[ApiController]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
[ApiVersion(ApiVersionConstant.V2)]
public class BooksController : ControllerBase
{
    private readonly BookRepository _bookRepository;
    private readonly LuckHelper _luckHelper;


    public BooksController(BookRepository bookRepository, LuckHelper luckHelper)
        => (_bookRepository, _luckHelper) = (bookRepository, luckHelper);


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<Book>))]
    public ActionResult<IReadOnlyCollection<Book>> GetAll() => Ok(_bookRepository.GetAll());

    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Book> Get(Guid id)
    {
        var book = _bookRepository.Get(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Book> Create(Book book)
    {
        if (_luckHelper.Lucky)
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
    }
}