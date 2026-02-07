using System.Text;
using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Services;

/// <summary>
/// Exports reports to CSV/Excel.
/// </summary>
public sealed class ReportService : IReportService
{
    private readonly IInventoryRepository _repository;

    public ReportService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> ExportInventoryToCsvAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("Sku,Name,Category,StandardCost,Currency");

        foreach (var item in items)
        {
            builder.AppendLine($"{item.Sku},{item.Name},{item.Category},{item.StandardCost.Amount},{item.StandardCost.Currency}");
        }

        return builder.ToString();
    }

    public Task<string> ExportInventoryToExcelAsync(CancellationToken cancellationToken)
    {
        // TODO: Consider EPPlus or ClosedXML for real Excel export.
        return Task.FromResult("Excel export not configured. TODO: implement with EPPlus/ClosedXML.");
    }
}
