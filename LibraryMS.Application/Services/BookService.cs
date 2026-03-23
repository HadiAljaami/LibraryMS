using AutoMapper;
using LibraryMS.Application.DTOs.Books;
using LibraryMS.Application.Validators;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;

namespace LibraryMS.Application.Services;

public class BookService(
    IBookRepository bookRepo,
    ICategoryRepository categoryRepo,
    IMapper mapper)
{
    public async Task<Result<PagedResult<BookResponseDto>>> SearchAsync(
        BookSearchDto search)
    {
        var result = await bookRepo.SearchAsync(
            search.Title, search.Author, search.ISBN,
            search.CategoryId, search.IsAvailable,
            search.PageNumber, search.PageSize);

        var mapped = new PagedResult<BookResponseDto>(
            result.Items.Select(mapper.Map<BookResponseDto>),
            result.TotalCount, result.PageNumber, result.PageSize);

        return Result.Success(mapped);
    }

    public async Task<Result<BookResponseDto>> GetByIdAsync(int id)
    {
        var book = await bookRepo.GetWithDetailsAsync(id);
        if (book is null)
            return Result.Failure<BookResponseDto>("الكتاب غير موجود");

        return Result.Success(mapper.Map<BookResponseDto>(book));
    }

    public async Task<Result<BookResponseDto>> CreateAsync(BookCreateDto dto)
    {
        // Validate
        var validator = new BookCreateValidator();
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Failure<BookResponseDto>(
                string.Join("، ", validation.Errors.Select(e => e.ErrorMessage)));

        // Check ISBN unique
        if (!await bookRepo.IsISBNUniqueAsync(dto.ISBN))
            return Result.Failure<BookResponseDto>("رقم ISBN مستخدم مسبقاً");

        // Check category exists
        if (!await categoryRepo.ExistsAsync(dto.CategoryId))
            return Result.Failure<BookResponseDto>("التصنيف غير موجود");

        var book = mapper.Map<Book>(dto);
        book.AvailableCopies = dto.TotalCopies;
        book.CreatedAt = DateTime.UtcNow;

        await bookRepo.AddAsync(book);
        var created = await bookRepo.GetWithDetailsAsync(book.Id);
        return Result.Success(mapper.Map<BookResponseDto>(created!));
    }

    public async Task<Result> UpdateAsync(int id, BookUpdateDto dto)
    {
        var validator = new BookUpdateValidator();
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Failure(
                string.Join("، ", validation.Errors.Select(e => e.ErrorMessage)));

        var book = await bookRepo.GetByIdAsync(id);
        if (book is null)
            return Result.Failure("الكتاب غير موجود");

        if (!await bookRepo.IsISBNUniqueAsync(dto.ISBN, id))
            return Result.Failure("رقم ISBN مستخدم مسبقاً");

        var diff = dto.TotalCopies - book.TotalCopies;
        mapper.Map(dto, book);
        book.AvailableCopies = Math.Max(0, book.AvailableCopies + diff);
        book.UpdatedAt = DateTime.UtcNow;

        await bookRepo.UpdateAsync(book);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var book = await bookRepo.GetWithDetailsAsync(id);
        if (book is null)
            return Result.Failure("الكتاب غير موجود");

        if (book.AvailableCopies < book.TotalCopies)
            return Result.Failure("لا يمكن حذف كتاب لديه نسخ مستعارة");

        await bookRepo.DeleteAsync(book);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<BookResponseDto>>> GetByCategoryAsync(
        int categoryId)
    {
        var books = await bookRepo.GetByCategoryAsync(categoryId);
        return Result.Success(books.Select(mapper.Map<BookResponseDto>));
    }
}