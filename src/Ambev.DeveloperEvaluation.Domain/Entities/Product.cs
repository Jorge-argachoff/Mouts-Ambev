using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product that can be referenced by a SaleItem using the
/// External Identities pattern (ProductId + denormalized ProductName).
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
}
