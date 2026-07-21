using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Infrastructure.Persistence.SeedData;

public static class ApplicationDbInitializer
{
    public static async Task SeedAsync(NexusERPDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.EnsureCreatedAsync();

        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Name = "Admin", Description = "Administrator with full access", IsActive = true },
                new() { Name = "Manager", Description = "Manager with partial access", IsActive = true },
                new() { Name = "Employee", Description = "Employee with basic access", IsActive = true }
            };
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@nexuserp.com",
                PasswordHash = passwordHasher.HashPassword("Admin123!"),
                IsActive = true,
                EmailVerified = true
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                AssignedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        if (!await context.SystemConfigurations.AnyAsync())
        {
            var configs = new List<SystemConfiguration>
            {
                new() { Module = "General", Key = "CompanyName", Value = "NexusERP Demo", DataType = "string" },
                new() { Module = "General", Key = "CompanyRfc", Value = "XAXX010101000", DataType = "string" },
                new() { Module = "Inventory", Key = "DefaultWarehouseCode", Value = "ALM-001", DataType = "string" },
                new() { Module = "Sales", Key = "InvoicePrefix", Value = "INV", DataType = "string" },
                new() { Module = "Purchases", Key = "PurchaseOrderPrefix", Value = "OC", DataType = "string" }
            };
            context.SystemConfigurations.AddRange(configs);
            await context.SaveChangesAsync();
        }
    }
}
