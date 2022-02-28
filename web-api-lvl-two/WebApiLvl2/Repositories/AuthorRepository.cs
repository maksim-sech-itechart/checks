using WebApiLvl2.Helpers;
using WebApiLvl2.Models;


namespace WebApiLvl2.Repositories;

public class AuthorRepository
{
    private readonly LuckHelper _luckHelper;


    public AuthorRepository(LuckHelper luckHelper) => _luckHelper = luckHelper;


    public IReadOnlyCollection<Author> GetAll() => new[]
    {
        new Author
        {
            Id = Guid.NewGuid(),
            FirstName = "Fyodor",
            LastName = "Dostoevsky",
        },
        new Author
        {
            Id = Guid.NewGuid(),
            FirstName = "Dante",
            LastName = "Alighieri",
        },
        new Author
        {
            Id = Guid.NewGuid(),
            FirstName = "Lev",
            LastName = "Tolstoy",
        },
    };

    public Author? Get(Guid id)
    {
        if (_luckHelper.Lucky)
        {
            return null;
        }

        return new Author
        {
            Id = id,
            FirstName = "Lev",
            LastName = "Tolstoy",
        };
    }
}