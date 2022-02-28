using System.ComponentModel.DataAnnotations;


namespace WebApiLvl2.Models;

public class Book
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public int Year { get; set; }


    public Author Author { get; set; } = null!;
}