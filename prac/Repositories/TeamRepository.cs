using Microsoft.EntityFrameworkCore;
using TeamPractice.Data;
using TeamPractice.Models;

namespace TeamPractice.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Team?> GetAsync(int id)
    {
        return await _context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task CreateAsync(Team team)
    {
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        if (team is null)
            return;

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Team>> GetByCityAsync(string city)
    {
        return await _context.Teams
            .AsNoTracking()
            .Where(t => t.City == city)
            .ToListAsync();
    }
}
