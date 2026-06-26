using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventLogRepository _saleEventLogRepository;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(ISaleRepository saleRepository, ISaleEventLogRepository saleEventLogRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _saleEventLogRepository = saleEventLogRepository;
        _logger = logger;
    }

    public async Task<CancelSaleResponse> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("Event published: {Event} - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            nameof(SaleCancelledEvent), sale.Id, sale.SaleNumber);

        await _saleEventLogRepository.LogAsync(new SaleEventLog
        {
            EventType = nameof(SaleCancelledEvent),
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            Details = "Sale cancelled"
        }, cancellationToken);

        return new CancelSaleResponse { Success = true };
    }
}
