namespace Hpp.Application.Interfaces;

/// <summary>
/// Pluggable AI client abstraction.
/// </summary>
public interface IOpenAiClient
{
    Task<string> SummarizeAsync(string prompt, CancellationToken cancellationToken);
}
