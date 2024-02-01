using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// NOTE: Namespace specified before using Property.Core.Model due to the fact Property is also a namespace...
namespace Property.Core.Database;

using Property.Core.Model;

public class PropertyContext(DbContextOptions<PropertyContext> options) : DbContext(options)
{
    public required DbSet<Slot> Slots { get; set; }
    public required DbSet<Property> Properties { get; set; }
    public required DbSet<Reservation> Reservations { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("pg_trgm");
        ConfigureEntities(modelBuilder);
    }

    private static void ConfigureEntities(ModelBuilder model)
    {
        model.Entity<Slot>(ConfigureSlot);
        model.Entity<Property>(ConfigureProperty);
    }

    private static void ConfigureSlot(EntityTypeBuilder<Slot> slot)
    {
        slot.HasIndex(s => s.IsAvailable);
        slot.HasIndex(s => s.Start);
        slot.HasIndex(s => s.End);
    }

    private static void ConfigureProperty(EntityTypeBuilder<Property> property)
    {
        property.Property(p => p.Name).HasMaxLength(255);
        property.HasIndex(p => p.Location).HasMethod("gin").HasOperators("gin_trgm_ops");
        property.HasIndex(p => p.MaxPeople);
    }
}