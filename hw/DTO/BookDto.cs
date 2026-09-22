
namespace hw.DTO;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public decimal Price { get; set; }
    public int Year { get; set; }
    public string Description { get; set; } = string.Empty;
}
