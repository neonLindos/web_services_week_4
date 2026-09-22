using System.ComponentModel.DataAnnotations;

namespace hw.DTO;

public class SaveBookDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string AuthorName { get; set; } = string.Empty;

    [Range(1, 9999)]
    public int Year { get; set; }

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal Price { get; set; }

    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;
}
