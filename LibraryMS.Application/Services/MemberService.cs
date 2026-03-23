using AutoMapper;
using LibraryMS.Application.DTOs.Members;
using LibraryMS.Application.Validators;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;

namespace LibraryMS.Application.Services;

public class MemberService(
    IMemberRepository memberRepo,
    IMapper mapper)
{
    public async Task<Result<PagedResult<MemberResponseDto>>> SearchAsync(
        MemberSearchDto search)
    {
        var result = await memberRepo.SearchAsync(
            search.Name, search.Email, search.MembershipNumber,
            search.IsActive, search.PageNumber, search.PageSize);

        var mapped = new PagedResult<MemberResponseDto>(
            result.Items.Select(mapper.Map<MemberResponseDto>),
            result.TotalCount, result.PageNumber, result.PageSize);

        return Result.Success(mapped);
    }

    public async Task<Result<MemberResponseDto>> GetByIdAsync(int id)
    {
        var member = await memberRepo.GetWithLoansAsync(id);
        if (member is null)
            return Result.Failure<MemberResponseDto>("العضو غير موجود");

        return Result.Success(mapper.Map<MemberResponseDto>(member));
    }

    public async Task<Result<MemberResponseDto>> CreateAsync(MemberCreateDto dto)
    {
        var validator = new MemberCreateValidator();
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Failure<MemberResponseDto>(
                string.Join("، ", validation.Errors.Select(e => e.ErrorMessage)));

        if (!await memberRepo.IsEmailUniqueAsync(dto.Email))
            return Result.Failure<MemberResponseDto>(
                "البريد الإلكتروني مستخدم مسبقاً");

        var member = mapper.Map<Member>(dto);
        member.MembershipNumber = await GenerateMembershipNumberAsync();
        member.JoinDate = DateTime.UtcNow;
        member.CreatedAt = DateTime.UtcNow;

        await memberRepo.AddAsync(member);
        return Result.Success(mapper.Map<MemberResponseDto>(member));
    }

    public async Task<Result> UpdateAsync(int id, MemberUpdateDto dto)
    {
        var validator = new MemberUpdateValidator();
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Failure(
                string.Join("، ", validation.Errors.Select(e => e.ErrorMessage)));

        var member = await memberRepo.GetByIdAsync(id);
        if (member is null)
            return Result.Failure("العضو غير موجود");

        if (!await memberRepo.IsEmailUniqueAsync(dto.Email, id))
            return Result.Failure("البريد الإلكتروني مستخدم مسبقاً");

        mapper.Map(dto, member);
        member.UpdatedAt = DateTime.UtcNow;

        await memberRepo.UpdateAsync(member);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var member = await memberRepo.GetWithLoansAsync(id);
        if (member is null)
            return Result.Failure("العضو غير موجود");

        if (member.Loans.Any(l =>
            l.Status == Domain.Enums.LoanStatus.Active))
            return Result.Failure("لا يمكن حذف عضو لديه استعارات نشطة");

        if (member.UnpaidFines > 0)
            return Result.Failure("لا يمكن حذف عضو لديه غرامات غير مدفوعة");

        await memberRepo.DeleteAsync(member);
        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int id)
    {
        var member = await memberRepo.GetByIdAsync(id);
        if (member is null)
            return Result.Failure("العضو غير موجود");

        member.IsActive = !member.IsActive;
        member.UpdatedAt = DateTime.UtcNow;

        await memberRepo.UpdateAsync(member);
        return Result.Success();
    }

    private async Task<string> GenerateMembershipNumberAsync()
    {
        var count = (await memberRepo.GetAllAsync()).Count();
        return $"LMS-{DateTime.Now.Year}-{(count + 1):D5}";
    }
}