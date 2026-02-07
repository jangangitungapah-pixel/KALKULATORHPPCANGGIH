using System.Text;
using Hpp.Application.Interfaces;

namespace Hpp.Infrastructure.Services;

/// <summary>
/// Exports reports to CSV/Excel-compatible XML.
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

        foreach (var item in items.OrderBy(x => x.Sku, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append(EscapeCsv(item.Sku)).Append(',')
                .Append(EscapeCsv(item.Name)).Append(',')
                .Append(EscapeCsv(item.Category)).Append(',')
                .Append(item.StandardCost.Amount.ToString("0.####")).Append(',')
                .Append(EscapeCsv(item.StandardCost.Currency))
                .AppendLine();
        }

        return builder.ToString();
    }

    public async Task<string> ExportInventoryToExcelAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetItemsAsync(cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("""<?xml version="1.0"?>""");
        builder.AppendLine("""<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet">""");
        builder.AppendLine("""  <Worksheet ss:Name="Inventory" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">""");
        builder.AppendLine("    <Table>");
        builder.AppendLine("""      <Row><Cell><Data ss:Type="String">Sku</Data></Cell><Cell><Data ss:Type="String">Name</Data></Cell><Cell><Data ss:Type="String">Category</Data></Cell><Cell><Data ss:Type="String">StandardCost</Data></Cell><Cell><Data ss:Type="String">Currency</Data></Cell></Row>""");

        foreach (var item in items.OrderBy(x => x.Sku, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine(
                $"      <Row><Cell><Data ss:Type=\"String\">{EscapeXml(item.Sku)}</Data></Cell><Cell><Data ss:Type=\"String\">{EscapeXml(item.Name)}</Data></Cell><Cell><Data ss:Type=\"String\">{EscapeXml(item.Category)}</Data></Cell><Cell><Data ss:Type=\"Number\">{item.StandardCost.Amount:0.####}</Data></Cell><Cell><Data ss:Type=\"String\">{EscapeXml(item.StandardCost.Currency)}</Data></Cell></Row>");
        }

        builder.AppendLine("    </Table>");
        builder.AppendLine("  </Worksheet>");
        builder.AppendLine("</Workbook>");
        return builder.ToString();
    }

    private static string EscapeCsv(string? value)
    {
        var safe = value ?? string.Empty;
        if (!safe.Contains(',') && !safe.Contains('"') && !safe.Contains('\n') && !safe.Contains('\r'))
        {
            return safe;
        }

        return $"\"{safe.Replace("\"", "\"\"")}\"";
    }

    private static string EscapeXml(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("'", "&apos;", StringComparison.Ordinal);
    }
}
