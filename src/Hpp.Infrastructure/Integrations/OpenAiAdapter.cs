using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Integrations;

/// <summary>
/// Azure OpenAI adapter (placeholder).
/// </summary>
public sealed class OpenAiAdapter : IOpenAiClient
{
    public Task<string> SummarizeAsync(string prompt, CancellationToken cancellationToken)
    {
        // TODO: Wire Azure OpenAI client with configured endpoint and keys.
        return Task.FromResult("TODO: AI summary not configured.");
    }
}

/// <summary>
/// Null AI client for offline usage.
/// </summary>
public sealed class OpenAiNullClient : IOpenAiClient
{
    public Task<string> SummarizeAsync(string prompt, CancellationToken cancellationToken)
    {
        return Task.FromResult("AI is disabled. Configure OPENAI_API_KEY.");
    }
}
