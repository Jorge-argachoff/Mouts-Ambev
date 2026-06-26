using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for persisting Sale domain event audit records in the NoSQL store.
/// </summary>
public interface ISaleEventLogRepository
{
    Task LogAsync(SaleEventLog eventLog, CancellationToken cancellationToken = default);
}
