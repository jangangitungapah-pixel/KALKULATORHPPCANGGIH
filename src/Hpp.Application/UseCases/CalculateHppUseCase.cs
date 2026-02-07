using Hpp.Application.Interfaces;
using Hpp.Application.Services;
using Hpp.Domain.ValueObjects;

namespace Hpp.Application.UseCases;

/// <summary>
/// Calculates HPP for a single item.
/// </summary>
public sealed class CalculateHppUseCase
{
    private readonly IInventoryRepository _repository;
    private readonly CostingService _costingService;

    public CalculateHppUseCase(IInventoryRepository repository, CostingService costingService)
    {
        _repository = repository;
        _costingService = costingService;
    }

    public async Task<Money> ExecuteAsync(Guid itemId, decimal quantity, string strategy, CancellationToken cancellationToken)
    {
        var item = await _repository.GetItemAsync(itemId, cancellationToken);
        if (item is null)
        {
            throw new InvalidOperationException("Item not found.");
        }

        var lots = item.Lots.ToList();
        return _costingService.Calculate(item, lots, quantity, strategy);
    }
}
