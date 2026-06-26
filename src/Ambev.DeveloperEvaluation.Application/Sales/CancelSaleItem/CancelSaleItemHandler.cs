using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventLogRepository _saleEventLogRepository;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(ISaleRepository saleRepository, ISaleEventLogRepository saleEventLogRepository, ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _saleEventLogRepository = saleEventLogRepository;
        _logger = logger;
    }

    public async Task<CancelSaleItemResponse> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        sale.CancelItem(request.ItemId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("Event published: {Event} - SaleId: {SaleId}, ItemId: {ItemId}",
            nameof(ItemCancelledEvent), sale.Id, request.ItemId);

        await _saleEventLogRepository.LogAsync(new SaleEventLog
        {
            EventType = nameof(ItemCancelledEvent),
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            Details = $"Item {request.ItemId} cancelled"
        }, cancellationToken);

        return new CancelSaleItemResponse { Success = true };
    }
}
