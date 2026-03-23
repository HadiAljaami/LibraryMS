using LibraryMS.Domain.Common;
using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Entities;

public class Reservation : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }
}