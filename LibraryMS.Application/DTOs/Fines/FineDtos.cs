namespace LibraryMS.Application.DTOs.Fines;

public record FinePaymentCreateDto(
    int MemberId,
    int? LoanId,
    decimal Amount,
    string PaymentMethod,
    string? Notes
);

public record FinePaymentResponseDto(
    int Id,
    int MemberId,
    string MemberName,
    int? LoanId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string? Notes,
    string ReceivedBy
);