using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class BookRepository(AppDbContext db)
    : GenericRepository<Book>(db), IBookRepository
{
    public async Task<Book?> GetByISBNAsync(string isbn) =>
        await _dbSet.FirstOrDefaultAsync(b => b.ISBN == isbn);

    public async Task<Book?> GetWithDetailsAsync(int id) =>
        await _dbSet
            .Include(b => b.Category)
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<PagedResult<Book>> SearchAsync(
        string? title,
        string? author,
        string? isbn,
        int? categoryId,
        bool? isAvailable,
        int pageNumber,
        int pageSize)
    {
        var query = _dbSet.Include(b => b.Category).AsQueryable();

        if (!string.IsNullOrEmpty(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrEmpty(author))
            query = query.Where(b => b.Author.Contains(author));

        if (!string.IsNullOrEmpty(isbn))
            query = query.Where(b => b.ISBN.Contains(isbn));

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId);

        if (isAvailable.HasValue)
            query = isAvailable.Value
                ? query.Where(b => b.AvailableCopies > 0)
                : query.Where(b => b.AvailableCopies == 0);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Book>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId) =>
        await _dbSet
            .Where(b => b.CategoryId == categoryId)
            .OrderBy(b => b.Title)
            .ToListAsync();

    public async Task<bool> IsISBNUniqueAsync(string isbn, int? excludeId = null)
    {
        var query = _dbSet.Where(b => b.ISBN == isbn);
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId);
        return !await query.AnyAsync();
    }
}