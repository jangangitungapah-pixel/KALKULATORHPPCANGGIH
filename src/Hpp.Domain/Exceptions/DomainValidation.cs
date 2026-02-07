namespace Hpp.Domain.Exceptions;

/// <summary>
/// Provides validation helpers for domain rules.
/// </summary>
public static class DomainValidation
{
    public static void AgainstNull(object? value, string name)
    {
        if (value is null)
        {
            throw new DomainException($"{name} cannot be null.");
        }
    }

    public static void AgainstNullOrWhiteSpace(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} cannot be empty.");
        }
    }

    public static void AgainstNegative(decimal value, string name)
    {
        if (value < 0)
        {
            throw new DomainException($"{name} cannot be negative.");
        }
    }

    public static void AgainstZeroOrNegative(decimal value, string name)
    {
        if (value <= 0)
        {
            throw new DomainException($"{name} must be greater than zero.");
        }
    }

    public static void AgainstOutOfRange(decimal value, decimal minInclusive, decimal maxInclusive, string name)
    {
        if (value < minInclusive || value > maxInclusive)
        {
            throw new DomainException($"{name} must be between {minInclusive} and {maxInclusive}.");
        }
    }
}
