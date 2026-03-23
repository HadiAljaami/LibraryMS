using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<Loan?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Loan>> GetByMemberAsync(int memberId);
    Task<IEnumerable<Loan>> GetByBookAsync(int bookId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(int memberId);
    Task<bool> HasActiveLoanAsync(int memberId, int bookId);
    Task<PagedResult<Loan>> GetPagedWithDetailsAsync(
        int? memberId,
        int? bookId,
        LoanStatus? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize);
    Task<int> GetActiveLoanCountByMemberAsync(int memberId);
}