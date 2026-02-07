using Hpp.Application.Interfaces;
using Hpp.Domain.Entities;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.UseCases;

/// <summary>
/// Imports invoices into the system.
/// </summary>
public sealed class ImportInvoiceUseCase
{
    private readonly IInvoiceIngestor _ingestor;
    private readonly IInventoryRepository _repository;

    public ImportInvoiceUseCase(IInvoiceIngestor ingestor, IInventoryRepository repository)
    {
        _ingestor = ingestor;
        _repository = repository;
    }

    public async Task ImportAsync(Stream invoiceStream, CancellationToken cancellationToken)
    {
        var purchases = await _ingestor.ParseAsync(invoiceStream, cancellationToken);

        foreach (var purchase in purchases)
        {
            var entity = new Purchase
            {
                ItemId = purchase.ItemId,
                Quantity = new Quantity(purchase.Quantity),
                UnitCost = new Money(purchase.UnitCost, purchase.Currency),
                PurchasedAt = purchase.PurchasedAt
            };

            await _repository.AddPurchaseAsync(entity, cancellationToken);
        }

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
