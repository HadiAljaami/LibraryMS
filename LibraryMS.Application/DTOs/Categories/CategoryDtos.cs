namespace LibraryMS.Application.DTOs.Categories;

public record CategoryCreateDto(
    string Name,
    string? Description,
    string? IconClass
);

public record CategoryUpdateDto(
    string Name,
    string? Description,
    string? IconClass
);

public record CategoryResponseDto(
    int Id,
    string Name,
    string? Description,
    string? IconClass,
    int BookCount
);