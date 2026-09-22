using Microsoft.EntityFrameworkCore;
using TeamPractice.Models;

namespace TeamPractice.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; }
}
