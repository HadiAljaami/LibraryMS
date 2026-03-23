namespace LibraryMS.Application.DTOs.Members;

public record MemberCreateDto(
    string FullName,
    string Email,
    string PhoneNumber,
    string? Address,
    string? NationalId,
    DateTime MembershipExpiry
);

public record MemberUpdateDto(
    string FullName,
    string Email,
    string PhoneNumber,
    string? Address,
    string? NationalId,
    DateTime MembershipExpiry,
    bool IsActive
);

public record MemberResponseDto(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string MembershipNumber,
    DateTime MembershipExpiry,
    DateTime JoinDate,
    bool IsActive,
    string? Address,
    decimal TotalFines,
    decimal PaidFines,
    decimal UnpaidFines
);

public record MemberSearchDto(
    string? Name,
    string? Email,
    string? MembershipNumber,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 12
);