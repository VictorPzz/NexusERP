using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.People;

namespace NexusERP.Infrastructure.Persistence.Configurations.People;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.DocumentType).HasColumnName("document_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.DocumentNumber).HasColumnName("document_number").HasMaxLength(20).IsRequired();
        builder.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.MiddleName).HasColumnName("middle_name").HasMaxLength(100);
        builder.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.SecondLastName).HasColumnName("second_last_name").HasMaxLength(100);
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
        builder.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
        builder.Property(e => e.Mobile).HasColumnName("mobile").HasMaxLength(20);
        builder.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
        builder.Property(e => e.Gender).HasColumnName("gender").HasMaxLength(10);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.DocumentNumber).IsUnique().HasDatabaseName("ix_persons_document_number");
        builder.HasIndex(e => e.Email).HasDatabaseName("ix_persons_email");

        builder.Ignore(e => e.UpdatedAt);
    }
}
