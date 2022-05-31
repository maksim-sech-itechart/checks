namespace WebApiLvl2.Models;

public class Author
{
    public Guid Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public List<Book> Books { get; set; } = null!;
}