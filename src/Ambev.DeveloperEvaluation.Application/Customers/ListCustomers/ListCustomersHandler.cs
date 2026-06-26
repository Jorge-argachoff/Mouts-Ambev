using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.ListCustomers;

public class ListCustomersHandler : IRequestHandler<ListCustomersCommand, List<ListCustomersResult>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public ListCustomersHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<List<ListCustomersResult>> Handle(ListCustomersCommand request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<ListCustomersResult>>(customers);
    }
}
