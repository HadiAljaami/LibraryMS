using AutoMapper;
using LibraryMS.Application.DTOs.Categories;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;

namespace LibraryMS.Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepo,
    IMapper mapper)
{
    public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync()
    {
        var categories = await categoryRepo.GetAllWithBookCountAsync();
        return Result.Success(categories.Select(mapper.Map<CategoryResponseDto>));
    }

    public async Task<Result<CategoryResponseDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepo.GetWithBooksAsync(id);
        if (category is null)
            return Result.Failure<CategoryResponseDto>("التصنيف غير موجود");

        return Result.Success(mapper.Map<CategoryResponseDto>(category));
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result.Failure<CategoryResponseDto>("اسم التصنيف مطلوب");

        if (!await categoryRepo.IsNameUniqueAsync(dto.Name))
            return Result.Failure<CategoryResponseDto>("اسم التصنيف مستخدم مسبقاً");

        var category = mapper.Map<Category>(dto);
        category.CreatedAt = DateTime.UtcNow;

        await categoryRepo.AddAsync(category);
        return Result.Success(mapper.Map<CategoryResponseDto>(category));
    }

    public async Task<Result> UpdateAsync(int id, CategoryUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result.Failure("اسم التصنيف مطلوب");

        var category = await categoryRepo.GetByIdAsync(id);
        if (category is null)
            return Result.Failure("التصنيف غير موجود");

        if (!await categoryRepo.IsNameUniqueAsync(dto.Name, id))
            return Result.Failure("اسم التصنيف مستخدم مسبقاً");

        mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;

        await categoryRepo.UpdateAsync(category);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var category = await categoryRepo.GetWithBooksAsync(id);
        if (category is null)
            return Result.Failure("التصنيف غير موجود");

        if (category.Books.Any())
            return Result.Failure("لا يمكن حذف تصنيف يحتوي على كتب");

        await categoryRepo.DeleteAsync(category);
        return Result.Success();
    }
}