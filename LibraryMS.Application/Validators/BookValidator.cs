using FluentValidation;
using LibraryMS.Application.DTOs.Books;

namespace LibraryMS.Application.Validators;

public class BookCreateValidator : AbstractValidator<BookCreateDto>
{
    public BookCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان الكتاب مطلوب")
            .MaximumLength(300).WithMessage("العنوان لا يتجاوز 300 حرف");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("اسم المؤلف مطلوب")
            .MaximumLength(200).WithMessage("اسم المؤلف لا يتجاوز 200 حرف");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("رقم ISBN مطلوب")
            .MaximumLength(20).WithMessage("ISBN لا يتجاوز 20 حرف");

        RuleFor(x => x.Publisher)
            .NotEmpty().WithMessage("دار النشر مطلوبة")
            .MaximumLength(200).WithMessage("دار النشر لا تتجاوز 200 حرف");

        RuleFor(x => x.PublishYear)
            .InclusiveBetween(1000, DateTime.Now.Year)
            .WithMessage($"سنة النشر يجب أن تكون بين 1000 و {DateTime.Now.Year}");

        RuleFor(x => x.TotalCopies)
            .GreaterThan(0).WithMessage("عدد النسخ يجب أن يكون أكبر من صفر");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("التصنيف مطلوب");
    }
}

public class BookUpdateValidator : AbstractValidator<BookUpdateDto>
{
    public BookUpdateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان الكتاب مطلوب")
            .MaximumLength(300).WithMessage("العنوان لا يتجاوز 300 حرف");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("اسم المؤلف مطلوب")
            .MaximumLength(200).WithMessage("اسم المؤلف لا يتجاوز 200 حرف");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("رقم ISBN مطلوب")
            .MaximumLength(20).WithMessage("ISBN لا يتجاوز 20 حرف");

        RuleFor(x => x.TotalCopies)
            .GreaterThan(0).WithMessage("عدد النسخ يجب أن يكون أكبر من صفر");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("التصنيف مطلوب");
    }
}