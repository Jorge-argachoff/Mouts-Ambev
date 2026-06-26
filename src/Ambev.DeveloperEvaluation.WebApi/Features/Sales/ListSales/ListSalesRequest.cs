namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Query parameters for listing sales, following the General API pagination/ordering conventions.
/// </summary>
public class ListSalesRequest
{
    public int _page { get; set; } = 1;
    public int _size { get; set; } = 10;
    public string? _order { get; set; }
    public Guid? CustomerId { get; set; }
    public string? BranchId { get; set; }
    public bool? IsCancelled { get; set; }
}
