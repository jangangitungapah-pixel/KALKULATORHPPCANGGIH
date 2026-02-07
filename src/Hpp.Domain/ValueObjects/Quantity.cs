namespace Hpp.Domain.ValueObjects;

/// <summary>
/// Represents an inventory quantity.
/// </summary>
public readonly record struct Quantity(decimal Value)
{
    public static Quantity Zero => new(0m);

    public Quantity Add(Quantity other) => new(Value + other.Value);
    public Quantity Subtract(Quantity other) => new(Value - other.Value);
}
