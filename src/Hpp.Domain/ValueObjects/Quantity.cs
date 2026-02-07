using Hpp.Domain.Exceptions;

namespace Hpp.Domain.ValueObjects;

/// <summary>
/// Represents an inventory quantity.
/// </summary>
public readonly record struct Quantity
{
    public Quantity(decimal value)
    {
        DomainValidation.AgainstNegative(value, nameof(value));
        Value = decimal.Round(value, 4, MidpointRounding.AwayFromZero);
    }

    public decimal Value { get; }

    public static Quantity Zero => new(0m);

    public bool IsZero => Value == 0m;

    public Quantity Add(Quantity other)
        => new(Value + other.Value);

    public Quantity Subtract(Quantity other)
    {
        if (other.Value > Value)
        {
            throw new DomainException("Quantity subtraction cannot result in a negative quantity.");
        }

        return new Quantity(Value - other.Value);
    }

    public Quantity Multiply(decimal factor)
        => new(Value * factor);

    public override string ToString() => Value.ToString("0.####");
}
