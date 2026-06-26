using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public record CancelSaleItemCommand : IRequest<CancelSaleItemResponse>
{
    public Guid SaleId { get; }
    public Guid ItemId { get; }

    public CancelSaleItemCommand(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
    }
}
