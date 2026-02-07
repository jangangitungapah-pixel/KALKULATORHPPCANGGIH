using System.Text;
using Hpp.Application.DTOs;
using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Integrations;

/// <summary>
/// Form recognizer adapter with deterministic local parsing fallback.
/// </summary>
public sealed class FormRecognizerAdapter : IInvoiceParser
{
    public Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<PurchaseDto>>(ParsePlainTextRows(content));
    }

    private static IReadOnlyList<PurchaseDto> ParsePlainTextRows(byte[] content)
    {
        if (content.Length == 0)
        {
            return Array.Empty<PurchaseDto>();
        }

        var text = Encoding.UTF8.GetString(content);
        var rows = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var purchases = new List<PurchaseDto>();

        foreach (var row in rows)
        {
            // Expected: itemId|qty|unitCost|currency|timestamp
            var parts = row.Split('|', StringSplitOptions.TrimEntries);
            if (parts.Length < 3)
            {
                continue;
            }

            if (!Guid.TryParse(parts[0], out var itemId))
            {
                continue;
            }

            if (!decimal.TryParse(parts[1], out var qty) ||
                !decimal.TryParse(parts[2], out var unitCost) ||
                qty <= 0m ||
                unitCost < 0m)
            {
                continue;
            }

            var currency = parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]) ? parts[3] : "IDR";
            var purchasedAt = parts.Length > 4 && DateTimeOffset.TryParse(parts[4], out var parsedAt)
                ? parsedAt
                : DateTimeOffset.UtcNow;

            purchases.Add(new PurchaseDto(Guid.NewGuid(), itemId, qty, unitCost, currency, purchasedAt));
        }

        return purchases;
    }
}

/// <summary>
/// Tesseract OCR fallback adapter.
/// </summary>
public sealed class TesseractAdapter : IInvoiceParser
{
    public Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken)
    {
        // Reuse deterministic parser while OCR integration is not configured.
        return new FormRecognizerAdapter().ParseAsync(content, cancellationToken);
    }
}

/// <summary>
/// Mock invoice parser for offline usage.
/// </summary>
public sealed class InvoiceParserMock : IInvoiceParser
{
    public Task<IReadOnlyList<PurchaseDto>> ParseAsync(byte[] content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sample = new PurchaseDto(
            Id: Guid.NewGuid(),
            ItemId: Guid.NewGuid(),
            Quantity: 1,
            UnitCost: 10000m,
            Currency: "IDR",
            PurchasedAt: DateTimeOffset.UtcNow);

        return Task.FromResult<IReadOnlyList<PurchaseDto>>(new List<PurchaseDto> { sample });
    }
}
