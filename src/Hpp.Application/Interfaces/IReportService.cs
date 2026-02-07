namespace Hpp.Application.Interfaces;

/// <summary>
/// Exports reports.
/// </summary>
public interface IReportService
{
    Task<string> ExportInventoryToCsvAsync(CancellationToken cancellationToken);
    Task<string> ExportInventoryToExcelAsync(CancellationToken cancellationToken);
}
