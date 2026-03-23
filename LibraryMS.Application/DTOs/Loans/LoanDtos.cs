using LibraryMS.Domain.Enums;

namespace LibraryMS.Application.DTOs.Loans;

public record LoanCreateDto(
    int BookId,
    int MemberId,
    int DurationDays
);

public record LoanResponseDto(
    int Id,
    int BookId,
    string BookTitle,
    string BookISBN,
    int MemberId,
    string MemberName,
    string MembershipNumber,
    DateTime BorrowDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    LoanStatus Status,
    int RenewalCount,
    decimal FineAmount,
    bool FinePaid
);

public record LoanSearchDto(
    int? MemberId,
    int? BookId,
    LoanStatus? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 12
);

public record LoanRenewDto(
    int LoanId,
    int AdditionalDays
);