using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.People;

namespace NexusERP.Infrastructure.Persistence.Configurations.People;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(e => e.EmployeeCode).HasColumnName("employee_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.JobPositionId).HasColumnName("job_position_id").IsRequired();
        builder.Property(e => e.HireDate).HasColumnName("hire_date").IsRequired();
        builder.Property(e => e.TerminationDate).HasColumnName("termination_date");
        builder.Property(e => e.EmploymentType).HasColumnName("employment_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Salary).HasColumnName("salary").HasColumnType("decimal(12,2)");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.EmployeeCode).IsUnique().HasDatabaseName("ix_employees_code");
        builder.HasIndex(e => e.PersonId).IsUnique().HasDatabaseName("ix_employees_person_id");

        builder.HasOne(e => e.Person).WithOne(e => e.Employee).HasForeignKey<Employee>(e => e.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.JobPosition).WithMany(e => e.Employees).HasForeignKey(e => e.JobPositionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Contacts).WithOne(e => e.Employee).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
