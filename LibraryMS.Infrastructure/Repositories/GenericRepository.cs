using LibraryMS.Domain.Common;
using LibraryMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using LibraryMS.Infrastructure.Data;

namespace LibraryMS.Infrastructure.Repositories;

public class GenericRepository<T>(AppDbContext db)
    : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _db = db;
    protected readonly DbSet<T> _dbSet = db.Set<T>();

    public async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber, int pageSize)
    {
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _dbSet.AnyAsync(e => e.Id == id);
}