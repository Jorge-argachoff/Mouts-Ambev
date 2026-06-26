using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventLogRepository _saleEventLogRepository;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository saleRepository, ISaleEventLogRepository saleEventLogRepository, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _saleEventLogRepository = saleEventLogRepository;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        sale.CustomerId = command.CustomerId;
        sale.CustomerName = command.CustomerName;
        sale.BranchId = command.BranchId;
        sale.BranchName = command.BranchName;

        sale.Items.Clear();
        foreach (var item in command.Items)
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        sale.RecalculateTotal();

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("Event published: {Event} - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            nameof(SaleModifiedEvent), updatedSale.Id, updatedSale.SaleNumber);

        await _saleEventLogRepository.LogAsync(new SaleEventLog
        {
            EventType = nameof(SaleModifiedEvent),
            SaleId = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            Details = $"Sale updated, new total amount {updatedSale.TotalAmount:C}"
        }, cancellationToken);

        return new UpdateSaleResult
        {
            Id = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            TotalAmount = updatedSale.TotalAmount
        };
    }
}
