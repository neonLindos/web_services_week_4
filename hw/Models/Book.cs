


public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    public decimal Price { get; set; }

    public int Year { get; set; }

    public string Description { get; set; } = "";
}
