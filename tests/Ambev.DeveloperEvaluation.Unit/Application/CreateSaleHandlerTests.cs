using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventLogRepository _saleEventLogRepository;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _saleEventLogRepository = Substitute.For<ISaleEventLogRepository>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _saleEventLogRepository, _logger);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(call => Task.FromResult(call.Arg<Sale>()));
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then applies discount and returns total")]
    public async Task Handle_ValidRequest_AppliesDiscountAndReturnsTotal()
    {
        // Given
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = "BR-001",
            BranchName = "Downtown",
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Beer",
                    Quantity = 10,
                    UnitPrice = 10m
                }
            ]
        };

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalAmount.Should().Be(10 * 10m * 0.80m);
        result.SaleNumber.Should().NotBeNullOrEmpty();
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a sale with no items When creating sale Then throws validation exception")]
    public async Task Handle_NoItems_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = "BR-001",
            BranchName = "Downtown"
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given an item quantity above 20 When creating sale Then throws validation exception")]
    public async Task Handle_QuantityAboveTwenty_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = "BR-001",
            BranchName = "Downtown",
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Beer",
                    Quantity = 21,
                    UnitPrice = 10m
                }
            ]
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
