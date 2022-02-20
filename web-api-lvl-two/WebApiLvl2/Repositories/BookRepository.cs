using WebApiLvl2.Helpers;
using WebApiLvl2.Models;


// ReSharper disable MemberCanBeMadeStatic.Global

namespace WebApiLvl2.Repositories;

public class BookRepository
{
    private readonly LuckHelper _luckHelper;


    public BookRepository(LuckHelper luckHelper) => _luckHelper = luckHelper;


    public IReadOnlyCollection<Book> GetAll() => new[]
    {
        new Book
        {
            Id = Guid.NewGuid(),
            Name = "The Idiot",
            Year = 1868,
            Author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "Fyodor",
                LastName = "Dostoevsky",
            },
        },
        new Book
        {
            Id = Guid.NewGuid(),
            Name = "Divine Comedy",
            Year = 1320,
            Author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "Dante",
                LastName = "Alighieri",
            },
        },
    };

    public Book Get(Guid id)
    {
        if (_luckHelper.Lucky)
        {
            return null;
        }

        return new Book
        {
            Id = Guid.NewGuid(),
            Name = "Divine Comedy",
            Year = 1320,
            Author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = "Dante",
                LastName = "Alighieri",
            },
        };
    }
}