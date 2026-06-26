using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a customer that can be referenced by a Sale using the
/// External Identities pattern (CustomerId + denormalized CustomerName).
/// </summary>
public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
