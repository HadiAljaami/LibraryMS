using LibraryMS.Domain.Common;
using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Entities;

public class Loan : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    public int RenewalCount { get; set; } = 0;
    public decimal FineAmount { get; set; } = 0;
    public bool FinePaid { get; set; } = false;
    public string? Notes { get; set; }
}