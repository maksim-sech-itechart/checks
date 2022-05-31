namespace WebApiLvl2.Controllers;

[ApiController]
[Route("api/[controller]")]
[DisableCors]
public class AuthorsController : ControllerBase
{
    private readonly AuthorRepository _authorRepository;
    private readonly LuckHelper _luckHelper;


    public AuthorsController(AuthorRepository authorRepository, LuckHelper luckHelper)
        => (_authorRepository, _luckHelper) = (authorRepository, luckHelper);


    [HttpGet]
    [ApiVersion(ApiVersionConstant.V1)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<Author>))]
    public ActionResult<IReadOnlyCollection<Author>> GetAllV1() => Ok(_authorRepository.GetAll());

    [HttpGet]
    [ApiVersion(ApiVersionConstant.V2)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<Author>))]
    public ActionResult<IReadOnlyCollection<Author>> GetAllV2()
    {
        var authors = _authorRepository.GetAll();

        foreach (var author in authors)
        {
            author.FirstName += "V2";
        }

        return Ok();
    }

    [HttpGet]
    [ApiVersionNeutral]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Author> Get(Guid id)
    {
        var author = _authorRepository.Get(id);

        if (author is null)
        {
            return NotFound();
        }

        return author;
    }

    [HttpPost]
    [ApiVersionNeutral]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Author))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<Author> Create(Author author)
    {
        if (_luckHelper.Lucky)
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
    }
}