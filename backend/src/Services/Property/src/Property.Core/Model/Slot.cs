namespace Property.Core.Model;

public sealed class Slot
{
    public Guid Id { get; set; }
    public Property Property { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public decimal? CustomPrice { get; set; }

    public Slot() { }

    public Slot(Property property, DateOnly start, DateOnly end, bool isAvailable, decimal? customPrice = null)
    {
        Property = property;
        Start = start;
        End = end;
        IsAvailable = isAvailable;
        CustomPrice = customPrice;
    }
}
