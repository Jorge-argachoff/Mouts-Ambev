namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}
