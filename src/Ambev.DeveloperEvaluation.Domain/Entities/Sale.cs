using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sales record (an invoice/order). Follows the External Identities
/// pattern: Customer, Branch and Products are referenced by Id with their
/// description denormalized at the time of the sale.
/// </summary>
public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    /// <summary>External identity of the customer.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Denormalized customer description at the time of sale.</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>External identity of the branch.</summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>Denormalized branch description at the time of sale.</summary>
    public string BranchName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public bool IsCancelled { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<SaleItem> Items { get; set; } = [];

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a new item to the sale, applying the quantity-based discount business rules,
    /// and recalculates the sale's total amount.
    /// </summary>
    public SaleItem AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            SaleId = Id,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        item.CalculateTotals();

        Items.Add(item);
        RecalculateTotal();

        return item;
    }

    /// <summary>Recalculates the sale total from its non-cancelled items.</summary>
    public void RecalculateTotal()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Cancels the entire sale.</summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Cancels a single item within the sale and recalculates the total.</summary>
    public void CancelItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException($"Item with id '{itemId}' was not found in this sale");

        item.Cancel();
        RecalculateTotal();
    }
}
