namespace Hpp.Application.Interfaces;

/// <summary>
/// Seeds sample data.
/// </summary>
public interface ISeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}
