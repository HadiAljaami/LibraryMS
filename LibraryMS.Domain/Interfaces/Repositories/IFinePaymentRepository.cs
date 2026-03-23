using LibraryMS.Domain.Entities;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface IFinePaymentRepository : IGenericRepository<FinePayment>
{
    Task<IEnumerable<FinePayment>> GetByMemberAsync(int memberId);
    Task<decimal> GetTotalPaidByMemberAsync(int memberId);
    Task<IEnumerable<FinePayment>> GetByDateRangeAsync(
        DateTime fromDate, DateTime toDate);
}