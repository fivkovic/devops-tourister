using Identity.Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Core.Database;

public class IdentityContext(DbContextOptions<IdentityContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        model.Entity<User>(ConfigureUser);
    }

    private static void ConfigureUser(EntityTypeBuilder<User> user)
    {
        user.Property(u => u.FirstName).HasMaxLength(100);
        user.Property(u => u.LastName).HasMaxLength(100);
        user.Property(u => u.Role).HasMaxLength(32);
    }
}