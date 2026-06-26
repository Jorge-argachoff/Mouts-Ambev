using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for retrieving a paginated, ordered list of sales.
/// </summary>
public class ListSalesCommand : IRequest<ListSalesResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    /// <summary>e.g. "saleDate desc" or "totalAmount asc, saleNumber"</summary>
    public string? Order { get; set; }

    public Guid? CustomerId { get; set; }
    public string? BranchId { get; set; }
    public bool? IsCancelled { get; set; }
}
