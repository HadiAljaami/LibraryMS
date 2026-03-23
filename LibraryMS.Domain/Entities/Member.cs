using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.Entities;

public class Member : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string MembershipNumber { get; set; } = string.Empty;
    public DateTime MembershipExpiry { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public decimal TotalFines { get; set; } = 0;
    public decimal PaidFines { get; set; } = 0;
    public decimal UnpaidFines => TotalFines - PaidFines;
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<FinePayment> FinePayments { get; set; } = new List<FinePayment>();
}