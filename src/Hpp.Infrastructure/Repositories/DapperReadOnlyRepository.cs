using System.Data;
using Dapper;

namespace Hpp.Infrastructure.Repositories;

/// <summary>
/// Example Dapper repository for heavy read queries.
/// </summary>
public sealed class DapperReadOnlyRepository
{
    private readonly IDbConnection _connection;

    public DapperReadOnlyRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyList<SkuSummary>> GetSkuSummaryAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT i.Sku, i.Name, i.Category
            FROM Items i
            ORDER BY i.Sku
            """;

        var result = await _connection.QueryAsync<SkuSummary>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return result.ToList();
    }
}

/// <summary>
/// SKU summary DTO for read-only queries.
/// </summary>
public sealed record SkuSummary(string Sku, string Name, string Category);
