using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating Sale/SaleItem test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid, empty Sale entities (no items).
    /// The generated sale will have valid:
    /// - SaleNumber (a synthetic, unique-looking code)
    /// - CustomerId/CustomerName and BranchId/BranchName (External Identities pattern)
    /// - SaleDate (a recent past date)
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => Guid.NewGuid())
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Date.Past():yyyyMMddHHmmss}-{f.Random.AlphaNumeric(6).ToUpperInvariant()}")
        .RuleFor(s => s.SaleDate, f => f.Date.Past())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerName, f => f.Company.CompanyName())
        .RuleFor(s => s.BranchId, f => f.Random.AlphaNumeric(6).ToUpperInvariant())
        .RuleFor(s => s.BranchName, f => f.Address.City());

    /// <summary>
    /// Generates a valid, empty Sale entity with randomized data and no items.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated header data.</returns>
    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity with a given number of randomly-priced items,
    /// each with a quantity that does not trigger any discount (below 4 units).
    /// </summary>
    /// <param name="itemCount">The number of items to add to the sale.</param>
    /// <returns>A valid Sale entity populated with items.</returns>
    public static Sale GenerateValidSaleWithItems(int itemCount = 1)
    {
        var sale = GenerateValidSale();
        var faker = new Faker();

        for (var i = 0; i < itemCount; i++)
        {
            sale.AddItem(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                GenerateQuantityWithoutDiscount(),
                decimal.Parse(faker.Commerce.Price(1, 100)));
        }

        return sale;
    }

    /// <summary>
    /// Generates a quantity below the discount threshold (1 to 3 items).
    /// Used to test that no discount is applied.
    /// </summary>
    public static int GenerateQuantityWithoutDiscount()
    {
        return new Faker().Random.Int(1, 3);
    }

    /// <summary>
    /// Generates a quantity within the 10% discount tier (4 to 9 items).
    /// </summary>
    public static int GenerateQuantityWithTenPercentDiscount()
    {
        return new Faker().Random.Int(4, 9);
    }

    /// <summary>
    /// Generates a quantity within the 20% discount tier (10 to 20 items).
    /// </summary>
    public static int GenerateQuantityWithTwentyPercentDiscount()
    {
        return new Faker().Random.Int(10, 20);
    }

    /// <summary>
    /// Generates a quantity above the maximum allowed limit (21 or more items),
    /// used for testing that the domain rejects the sale.
    /// </summary>
    public static int GenerateQuantityAboveLimit()
    {
        return new Faker().Random.Int(21, 100);
    }

    /// <summary>
    /// Generates a valid unit price for a sale item.
    /// </summary>
    public static decimal GenerateValidUnitPrice()
    {
        return decimal.Parse(new Faker().Commerce.Price(1, 500));
    }

    /// <summary>
    /// Generates a valid product name for a sale item.
    /// </summary>
    public static string GenerateValidProductName()
    {
        return new Faker().Commerce.ProductName();
    }
}
