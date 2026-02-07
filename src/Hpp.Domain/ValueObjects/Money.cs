using System.Globalization;
using Hpp.Domain.Exceptions;

namespace Hpp.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount in a currency.
/// </summary>
public readonly record struct Money
{
    public Money(decimal amount, string currency)
    {
        DomainValidation.AgainstNullOrWhiteSpace(currency, nameof(currency));
        Currency = currency.Trim().ToUpperInvariant();
        Amount = decimal.Round(amount, 4, MidpointRounding.AwayFromZero);
    }

    public decimal Amount { get; }

    public string Currency { get; }

    public static Money Zero(string currency) => new(0m, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
        => new(Amount * factor, Currency);

    public Money Round(int decimals)
        => new(decimal.Round(Amount, decimals, MidpointRounding.AwayFromZero), Currency);

    public override string ToString()
        => $"{Amount.ToString("0.####", CultureInfo.InvariantCulture)} {Currency}";

    public static bool TryParse(string? raw, out Money value)
    {
        value = default;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var parts = raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0 || !decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            return false;
        }

        var currency = parts.Length > 1 ? parts[1] : "IDR";
        try
        {
            value = new Money(amount, currency);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    private void EnsureSameCurrency(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Currency mismatch: {Currency} vs {other.Currency}.");
        }
    }
}
