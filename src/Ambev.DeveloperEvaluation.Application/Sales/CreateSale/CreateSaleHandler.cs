using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventLogRepository _saleEventLogRepository;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository saleRepository, ISaleEventLogRepository saleEventLogRepository, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _saleEventLogRepository = saleEventLogRepository;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = GenerateSaleNumber(),
            SaleDate = command.SaleDate ?? DateTime.UtcNow,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        foreach (var item in command.Items)
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        _logger.LogInformation("Event published: {Event} - SaleId: {SaleId}, SaleNumber: {SaleNumber}",
            nameof(SaleCreatedEvent), createdSale.Id, createdSale.SaleNumber);

        await _saleEventLogRepository.LogAsync(new SaleEventLog
        {
            EventType = nameof(SaleCreatedEvent),
            SaleId = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            Details = $"Sale created with total amount {createdSale.TotalAmount:C}"
        }, cancellationToken);

        return new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            TotalAmount = createdSale.TotalAmount
        };
    }

    private static string GenerateSaleNumber() => $"SALE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";
}
