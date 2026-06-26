namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an audit record of a domain event raised by a Sale, persisted in a
/// NoSQL store (MongoDB) as a simple example of non-relational data usage.
/// </summary>
public class SaleEventLog
{
    public string Id { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public Guid SaleId { get; set; }

    public string SaleNumber { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
