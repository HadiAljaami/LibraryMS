using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetByISBNAsync(string isbn);
    Task<Book?> GetWithDetailsAsync(int id);
    Task<PagedResult<Book>> SearchAsync(
        string? title,
        string? author,
        string? isbn,
        int? categoryId,
        bool? isAvailable,
        int pageNumber,
        int pageSize);
    Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId);
    Task<bool> IsISBNUniqueAsync(string isbn, int? excludeId = null);
}