using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public record ListProductsCommand : IRequest<List<ListProductsResult>>;
