using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Integrations;

/// <summary>
/// Local AI adapter stub that returns a structured summary.
/// </summary>
public sealed class OpenAiAdapter : IOpenAiClient
{
    public Task<string> SummarizeAsync(string prompt, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return Task.FromResult("No prompt provided.");
        }

        var cleaned = prompt.Trim();
        var snippet = cleaned.Length > 220 ? $"{cleaned[..220]}..." : cleaned;
        return Task.FromResult($"Summary (local fallback): {snippet}");
    }
}

/// <summary>
/// Null AI client for offline usage.
/// </summary>
public sealed class OpenAiNullClient : IOpenAiClient
{
    public Task<string> SummarizeAsync(string prompt, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult("AI is disabled. Configure OPENAI_API_KEY to enable live summarization.");
    }
}
