using AutoMapper;
using LibraryMS.Application.DTOs.Books;
using LibraryMS.Application.DTOs.Categories;
using LibraryMS.Application.DTOs.Fines;
using LibraryMS.Application.DTOs.Loans;
using LibraryMS.Application.DTOs.Members;
using LibraryMS.Application.DTOs.Reservations;
using LibraryMS.Domain.Entities;

namespace LibraryMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Book
        CreateMap<Book, BookResponseDto>()
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category.Name));
        CreateMap<BookCreateDto, Book>();
        CreateMap<BookUpdateDto, Book>();

        // Category
        CreateMap<Category, CategoryResponseDto>()
            .ForMember(d => d.BookCount,
                o => o.MapFrom(s => s.Books.Count));
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();

        // Member
        CreateMap<Member, MemberResponseDto>();
        CreateMap<MemberCreateDto, Member>();
        CreateMap<MemberUpdateDto, Member>();

        // Loan
        CreateMap<Loan, LoanResponseDto>()
            .ForMember(d => d.BookTitle,
                o => o.MapFrom(s => s.Book.Title))
            .ForMember(d => d.BookISBN,
                o => o.MapFrom(s => s.Book.ISBN))
            .ForMember(d => d.MemberName,
                o => o.MapFrom(s => s.Member.FullName))
            .ForMember(d => d.MembershipNumber,
                o => o.MapFrom(s => s.Member.MembershipNumber));

        // Reservation
        CreateMap<Reservation, ReservationResponseDto>()
            .ForMember(d => d.BookTitle,
                o => o.MapFrom(s => s.Book.Title))
            .ForMember(d => d.MemberName,
                o => o.MapFrom(s => s.Member.FullName))
            .ForMember(d => d.MembershipNumber,
                o => o.MapFrom(s => s.Member.MembershipNumber))
            .ForMember(d => d.QueuePosition,
                o => o.Ignore());

        // FinePayment
        CreateMap<FinePayment, FinePaymentResponseDto>()
            .ForMember(d => d.MemberName,
                o => o.MapFrom(s => s.Member.FullName));
        CreateMap<FinePaymentCreateDto, FinePayment>();
    }
}