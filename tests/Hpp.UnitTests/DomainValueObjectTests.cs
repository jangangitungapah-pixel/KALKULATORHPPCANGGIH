using Hpp.Domain.Exceptions;
using Hpp.Domain.ValueObjects;
using Xunit;

namespace Hpp.UnitTests;

public class DomainValueObjectTests
{
    [Fact]
    public void Money_TryParse_ParsesInvariantFormat()
    {
        var parsed = Money.TryParse("1234.5|idr", out var value);

        Assert.True(parsed);
        Assert.Equal(1234.5m, value.Amount);
        Assert.Equal("IDR", value.Currency);
    }

    [Fact]
    public void Quantity_Subtract_BelowZero_Throws()
    {
        var qty = new Quantity(1m);

        Assert.Throws<DomainException>(() => qty.Subtract(new Quantity(2m)));
    }

    [Fact]
    public void Money_Add_DifferentCurrencies_Throws()
    {
        var left = new Money(10m, "IDR");
        var right = new Money(1m, "USD");

        Assert.Throws<InvalidOperationException>(() => left.Add(right));
    }
}
