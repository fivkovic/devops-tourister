namespace Reservation.Core.Model;

public sealed class Property
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string[] Images { get; set; } = [];
}
