using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Customers.ListCustomers;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers;

/// <summary>
/// Controller for retrieving customers
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : BaseController
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all customers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<List<ListCustomersResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListCustomers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListCustomersCommand(), cancellationToken);

        return Ok(new ApiResponseWithData<List<ListCustomersResult>>
        {
            Success = true,
            Message = "Customers retrieved successfully",
            Data = result
        });
    }
}
