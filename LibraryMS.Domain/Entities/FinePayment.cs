using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.Entities;

public class FinePayment : BaseEntity
{
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public int? LoanId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentMethod { get; set; } = "نقداً";
    public string? Notes { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
}