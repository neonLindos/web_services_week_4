using Microsoft.EntityFrameworkCore;
using hw.Data;

namespace hw.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(string? author = null)
    {
        var query = _context.Books.Include(book => book.Author).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(book => book.Author.Name.Contains(author.Trim()));
        }
        return await query.OrderBy(book => book.Id).ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.Include(book => book.Author)
            .FirstOrDefaultAsync(book => book.Id == id);
    }

    public async Task<Author> GetOrCreateAuthorAsync(string name)
    {
        name = name.Trim();
        return await _context.Autors.FirstOrDefaultAsync(author => author.Name == name)
            ?? new Author { Name = name };
    }

    public async Task AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}
