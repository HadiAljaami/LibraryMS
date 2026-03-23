using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.Infrastructure.Data.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.CopyNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(bc => bc.CopyNumber).IsUnique();

        builder.Property(bc => bc.Notes)
            .HasMaxLength(500);

        builder.HasOne(bc => bc.Book)
            .WithMany(b => b.Copies)
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(bc => !bc.IsDeleted);
    }
}