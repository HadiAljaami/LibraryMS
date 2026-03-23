using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface IBookCopyRepository : IGenericRepository<BookCopy>
{
    Task<IEnumerable<BookCopy>> GetByBookAsync(int bookId);
    Task<BookCopy?> GetAvailableCopyAsync(int bookId);
    Task<bool> IsCopyNumberUniqueAsync(string copyNumber, int? excludeId = null);
    Task<IEnumerable<BookCopy>> GetByCopyStatusAsync(CopyStatus status);
}