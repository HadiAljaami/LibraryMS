namespace LibraryMS.Application.DTOs.Reports;

public record DashboardStatsDto(
    int TotalBooks,
    int TotalMembers,
    int ActiveLoans,
    int OverdueLoans,
    int TotalReservations,
    int TotalCategories,
    decimal TotalUnpaidFines,
    int NewMembersThisMonth,
    int LoansThisMonth
);

public record MonthlyLoanDto(
    int Year,
    int Month,
    string MonthName,
    int Count
);

public record TopBookDto(
    int BookId,
    string Title,
    string Author,
    int LoanCount
);

public record TopMemberDto(
    int MemberId,
    string FullName,
    string MembershipNumber,
    int LoanCount
);

public record CategoryStatsDto(
    int CategoryId,
    string CategoryName,
    int BookCount,
    int LoanCount
);