using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.People;

namespace NexusERP.Infrastructure.Persistence.Configurations.People;

public class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        builder.ToTable("job_positions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(e => e.DepartmentId).HasColumnName("department_id").IsRequired();
        builder.Property(e => e.MinSalary).HasColumnName("min_salary").HasColumnType("decimal(12,2)");
        builder.Property(e => e.MaxSalary).HasColumnName("max_salary").HasColumnType("decimal(12,2)");
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

        builder.HasOne(e => e.Department).WithMany(e => e.JobPositions).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Employees).WithOne(e => e.JobPosition).HasForeignKey(e => e.JobPositionId).OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.UpdatedAt);
    }
}
