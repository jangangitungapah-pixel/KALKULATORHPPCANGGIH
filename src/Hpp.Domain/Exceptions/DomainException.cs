namespace Hpp.Domain.Exceptions;

/// <summary>
/// Represents a domain rule violation.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
