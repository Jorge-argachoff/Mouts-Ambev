using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

/// <summary>
/// Controller for retrieving products
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<List<ListProductsResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProducts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListProductsCommand(), cancellationToken);

        return Ok(new ApiResponseWithData<List<ListProductsResult>>
        {
            Success = true,
            Message = "Products retrieved successfully",
            Data = result
        });
    }
}
