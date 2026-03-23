using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.Infrastructure.Data.Configurations;

public class FinePaymentConfiguration : IEntityTypeConfiguration<FinePayment>
{
    public void Configure(EntityTypeBuilder<FinePayment> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Amount)
            .HasPrecision(10, 2);

        builder.Property(f => f.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(f => f.Notes)
            .HasMaxLength(500);

        builder.Property(f => f.ReceivedBy)
            .HasMaxLength(256);

        builder.HasOne(f => f.Member)
            .WithMany(m => m.FinePayments)
            .HasForeignKey(f => f.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}