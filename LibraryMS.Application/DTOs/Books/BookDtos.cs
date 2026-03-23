namespace LibraryMS.Application.DTOs.Books;

public record BookCreateDto(
    string Title,
    string Author,
    string ISBN,
    string? Description,
    string? CoverImage,
    string Publisher,
    int PublishYear,
    string Language,
    int TotalCopies,
    int CategoryId
);

public record BookUpdateDto(
    string Title,
    string Author,
    string ISBN,
    string? Description,
    string? CoverImage,
    string Publisher,
    int PublishYear,
    string Language,
    int TotalCopies,
    int CategoryId
);

public record BookResponseDto(
    int Id,
    string Title,
    string Author,
    string ISBN,
    string? Description,
    string? CoverImage,
    string Publisher,
    int PublishYear,
    string Language,
    int TotalCopies,
    int AvailableCopies,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt
);

public record BookSearchDto(
    string? Title,
    string? Author,
    string? ISBN,
    int? CategoryId,
    bool? IsAvailable,
    int PageNumber = 1,
    int PageSize = 12
);