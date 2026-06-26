using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a single product line within a Sale.
/// Denormalizes the product description following the External Identities pattern.
/// </summary>
public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }

    /// <summary>External identity of the product.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Denormalized product description at the time of sale.</summary>
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    /// <summary>Discount percentage applied to this item (0, 0.10 or 0.20).</summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>Total amount for this item after discount.</summary>
    public decimal TotalAmount { get; set; }

    public bool IsCancelled { get; set; }

    /// <summary>
    /// Calculates the discount percentage that applies for a given quantity,
    /// according to the business rules:
    /// - 4+ items: 10% discount
    /// - 10-20 items: 20% discount
    /// - Below 4 items: no discount
    /// - Above 20 items: not allowed
    /// </summary>
    public static decimal CalculateDiscountPercentage(int quantity)
    {
        if (quantity > 20)
            throw new DomainException("It's not possible to sell above 20 identical items");

        if (quantity >= 10)
            return 0.20m;

        if (quantity >= 4)
            return 0.10m;

        return 0m;
    }

    /// <summary>
    /// Recalculates discount and total amount based on quantity and unit price.
    /// </summary>
    public void CalculateTotals()
    {
        DiscountPercentage = CalculateDiscountPercentage(Quantity);
        var gross = Quantity * UnitPrice;
        TotalAmount = gross - (gross * DiscountPercentage);
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}
