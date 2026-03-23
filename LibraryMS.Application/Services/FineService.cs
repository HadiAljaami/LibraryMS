using AutoMapper;
using LibraryMS.Application.DTOs.Fines;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;

namespace LibraryMS.Application.Services;

public class FineService(
    IFinePaymentRepository fineRepo,
    IMemberRepository memberRepo,
    ILoanRepository loanRepo,
    IMapper mapper)
{
    public async Task<Result<FinePaymentResponseDto>> PayFineAsync(
        FinePaymentCreateDto dto, string receivedBy)
    {
        var member = await memberRepo.GetByIdAsync(dto.MemberId);
        if (member is null)
            return Result.Failure<FinePaymentResponseDto>("العضو غير موجود");

        if (member.UnpaidFines <= 0)
            return Result.Failure<FinePaymentResponseDto>(
                "لا توجد غرامات مستحقة على هذا العضو");

        if (dto.Amount <= 0)
            return Result.Failure<FinePaymentResponseDto>(
                "المبلغ يجب أن يكون أكبر من صفر");

        if (dto.Amount > member.UnpaidFines)
            return Result.Failure<FinePaymentResponseDto>(
                $"المبلغ يتجاوز الغرامات المستحقة ({member.UnpaidFines})");

        var payment = mapper.Map<FinePayment>(dto);
        payment.ReceivedBy  = receivedBy;
        payment.PaymentDate = DateTime.UtcNow;
        payment.CreatedAt   = DateTime.UtcNow;
        payment.CreatedBy   = receivedBy;

        member.PaidFines  += dto.Amount;
        member.UpdatedAt   = DateTime.UtcNow;

        // Mark loan fine as paid if fully paid
        if (dto.LoanId.HasValue)
        {
            var loan = await loanRepo.GetByIdAsync(dto.LoanId.Value);
            if (loan is not null && dto.Amount >= loan.FineAmount)
                loan.FinePaid = true;
        }

        await fineRepo.AddAsync(payment);
        await memberRepo.UpdateAsync(member);

        var created = await fineRepo.GetByIdAsync(payment.Id);
        return Result.Success(mapper.Map<FinePaymentResponseDto>(created!));
    }

    public async Task<Result<IEnumerable<FinePaymentResponseDto>>>
        GetByMemberAsync(int memberId)
    {
        var payments = await fineRepo.GetByMemberAsync(memberId);
        return Result.Success(
            payments.Select(mapper.Map<FinePaymentResponseDto>));
    }

    public async Task<Result<IEnumerable<FinePaymentResponseDto>>>
        GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var payments = await fineRepo.GetByDateRangeAsync(from, to);
        return Result.Success(
            payments.Select(mapper.Map<FinePaymentResponseDto>));
    }
}