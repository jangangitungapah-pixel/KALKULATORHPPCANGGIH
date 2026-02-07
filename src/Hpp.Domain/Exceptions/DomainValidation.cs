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

    public static void AgainstNegative(decimal value, string name)
    {
        if (value < 0)
        {
            throw new DomainException($"{name} cannot be negative.");
        }
    }
}
