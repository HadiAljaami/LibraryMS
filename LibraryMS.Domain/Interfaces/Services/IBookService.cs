using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.Interfaces.Services;

public interface IBookService
{
    Task<Result<BookResponseDto>> GetByIdAsync(int id);
    Task<Result<PagedResult<BookResponseDto>>> GetPagedAsync(BookSearchDto search);
    Task<Result<BookResponseDto>> CreateAsync(BookCreateDto dto);
    Task<Result> UpdateAsync(int id, BookUpdateDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<IEnumerable<BookResponseDto>>> GetByCategoryAsync(int categoryId);
}