using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext db)
    : GenericRepository<Category>(db), ICategoryRepository
{
    public async Task<Category?> GetWithBooksAsync(int id) =>
        await _dbSet
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.Where(c => c.Name == name);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId);
        return !await query.AnyAsync();
    }

    public async Task<IEnumerable<Category>> GetAllWithBookCountAsync() =>
        await _dbSet
            .Include(c => c.Books)
            .OrderBy(c => c.Name)
            .ToListAsync();
}