using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale and SaleItem entities, focused on the
/// quantity-based discount business rules.
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "Purchases below 4 items should have no discount")]
    public void Given_QuantityBelowFour_When_AddingItem_Then_NoDiscountIsApplied()
    {
        var sale = SaleTestData.GenerateValidSale();
        var quantity = SaleTestData.GenerateQuantityWithoutDiscount();
        var unitPrice = SaleTestData.GenerateValidUnitPrice();

        var item = sale.AddItem(Guid.NewGuid(), SaleTestData.GenerateValidProductName(), quantity, unitPrice);

        Assert.Equal(0m, item.DiscountPercentage);
        Assert.Equal(quantity * unitPrice, item.TotalAmount);
    }

    [Theory(DisplayName = "Purchases of 4 to 9 items should have a 10% discount")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_QuantityBetweenFourAndNine_When_AddingItem_Then_TenPercentDiscountIsApplied(int quantity)
    {
        var sale = SaleTestData.GenerateValidSale();

        var item = sale.AddItem(Guid.NewGuid(), "Beer", quantity, 10m);

        Assert.Equal(0.10m, item.DiscountPercentage);
        Assert.Equal(quantity * 10m * 0.90m, item.TotalAmount);
    }

    [Fact(DisplayName = "Random quantities within the 4-9 range should have a 10% discount")]
    public void Given_RandomQuantityInTenPercentTier_When_AddingItem_Then_TenPercentDiscountIsApplied()
    {
        var sale = SaleTestData.GenerateValidSale();
        var quantity = SaleTestData.GenerateQuantityWithTenPercentDiscount();
        var unitPrice = SaleTestData.GenerateValidUnitPrice();

        var item = sale.AddItem(Guid.NewGuid(), SaleTestData.GenerateValidProductName(), quantity, unitPrice);

        Assert.Equal(0.10m, item.DiscountPercentage);
        Assert.Equal(quantity * unitPrice * 0.90m, item.TotalAmount);
    }

    [Theory(DisplayName = "Purchases of 10 to 20 items should have a 20% discount")]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_QuantityBetweenTenAndTwenty_When_AddingItem_Then_TwentyPercentDiscountIsApplied(int quantity)
    {
        var sale = SaleTestData.GenerateValidSale();

        var item = sale.AddItem(Guid.NewGuid(), "Beer", quantity, 10m);

        Assert.Equal(0.20m, item.DiscountPercentage);
        Assert.Equal(quantity * 10m * 0.80m, item.TotalAmount);
    }

    [Fact(DisplayName = "Random quantities within the 10-20 range should have a 20% discount")]
    public void Given_RandomQuantityInTwentyPercentTier_When_AddingItem_Then_TwentyPercentDiscountIsApplied()
    {
        var sale = SaleTestData.GenerateValidSale();
        var quantity = SaleTestData.GenerateQuantityWithTwentyPercentDiscount();
        var unitPrice = SaleTestData.GenerateValidUnitPrice();

        var item = sale.AddItem(Guid.NewGuid(), SaleTestData.GenerateValidProductName(), quantity, unitPrice);

        Assert.Equal(0.20m, item.DiscountPercentage);
        Assert.Equal(quantity * unitPrice * 0.80m, item.TotalAmount);
    }

    [Fact(DisplayName = "Purchases above 20 identical items should not be allowed")]
    public void Given_QuantityAboveTwenty_When_AddingItem_Then_DomainExceptionIsThrown()
    {
        var sale = SaleTestData.GenerateValidSale();
        var quantity = SaleTestData.GenerateQuantityAboveLimit();

        Assert.Throws<DomainException>(() => sale.AddItem(Guid.NewGuid(), "Beer", quantity, 10m));
    }

    [Fact(DisplayName = "Sale total should be the sum of its non-cancelled items")]
    public void Given_MultipleItems_When_RecalculatingTotal_Then_TotalReflectsItemAmounts()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 5, 10m);
        sale.AddItem(Guid.NewGuid(), "Soda", 2, 5m);

        Assert.Equal((5 * 10m * 0.90m) + (2 * 5m), sale.TotalAmount);
    }

    [Fact(DisplayName = "Cancelling an item should exclude it from the sale total")]
    public void Given_SaleWithItems_When_CancellingItem_Then_TotalIsRecalculatedWithoutIt()
    {
        var sale = SaleTestData.GenerateValidSale();
        var item = sale.AddItem(Guid.NewGuid(), "Beer", 5, 10m);
        sale.AddItem(Guid.NewGuid(), "Soda", 2, 5m);

        sale.CancelItem(item.Id);

        Assert.True(item.IsCancelled);
        Assert.Equal(2 * 5m, sale.TotalAmount);
    }

    [Fact(DisplayName = "Cancelling the sale should mark it as cancelled")]
    public void Given_Sale_When_Cancelled_Then_IsCancelledIsTrue()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems();

        sale.Cancel();

        Assert.True(sale.IsCancelled);
    }
}
