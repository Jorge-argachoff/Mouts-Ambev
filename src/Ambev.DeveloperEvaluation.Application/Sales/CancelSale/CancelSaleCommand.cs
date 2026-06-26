using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public record CancelSaleCommand : IRequest<CancelSaleResponse>
{
    public Guid Id { get; }

    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}
