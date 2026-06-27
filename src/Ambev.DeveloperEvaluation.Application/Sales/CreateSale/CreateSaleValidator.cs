using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand that defines validation rules for sale creation.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(s => s.CustomerId).NotEmpty();
        RuleFor(s => s.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.BranchId).NotEmpty();
        RuleFor(s => s.BranchName).NotEmpty().MaximumLength(100);
        RuleFor(s => s.Items).NotEmpty().WithMessage("A sale must have at least one item");

        RuleFor(pedido => pedido.Items)
           .Must(VerificarQuantidadePorItem)
           .WithMessage("The total quantity for a single item cannot exceed 20.");

        RuleForEach(s => s.Items).ChildRules(item =>
        {

            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(100);
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20)
                .WithMessage("It's not possible to sell above 20 identical items");
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
    private bool VerificarQuantidadePorItem(List<CreateSaleItemCommand> itens)
    {
        if (itens == null) return true;

        // Agrupa por ItemId e soma as quantidades de cada grupo
        var quantidadesAgrupadas = itens
            .GroupBy(i => i.ProductId)
            .Select(grupo => new 
            { 
                ItemId = grupo.Key, 
                QuantidadeTotal = grupo.Sum(i => i.Quantity) 
            });

        // Retorna falso se QUALQUER item agrupado ultrapassar a quantidade de 20
        return quantidadesAgrupadas.All(item => item.QuantidadeTotal <= 20);
    }
}
