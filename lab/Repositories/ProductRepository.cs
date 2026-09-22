using Microsoft.EntityFrameworkCore;
using ProductLab.Data;
using ProductLab.Models;

namespace ProductLab.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
