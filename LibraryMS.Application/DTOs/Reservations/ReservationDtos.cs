using LibraryMS.Domain.Enums;

namespace LibraryMS.Application.DTOs.Reservations;

public record ReservationCreateDto(
    int BookId,
    int MemberId
);

public record ReservationResponseDto(
    int Id,
    int BookId,
    string BookTitle,
    int MemberId,
    string MemberName,
    string MembershipNumber,
    DateTime ReservationDate,
    DateTime ExpiryDate,
    ReservationStatus Status,
    int QueuePosition
);