using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(m => m.Email).IsUnique();

        builder.Property(m => m.MembershipNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.MembershipNumber).IsUnique();

        builder.Property(m => m.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(m => m.Address)
            .HasMaxLength(500);

        builder.Property(m => m.NationalId)
            .HasMaxLength(20);

        builder.Property(m => m.TotalFines)
            .HasPrecision(10, 2);

        builder.Property(m => m.PaidFines)
            .HasPrecision(10, 2);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}