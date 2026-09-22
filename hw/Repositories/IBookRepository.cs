
namespace hw.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(string? author = null);
    Task<Author> GetOrCreateAuthorAsync(string name);
    Task<Book?> GetByIdAsync(int id);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
}
