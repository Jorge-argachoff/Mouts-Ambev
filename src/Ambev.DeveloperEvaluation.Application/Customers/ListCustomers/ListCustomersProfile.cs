using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Customers.ListCustomers;

public class ListCustomersProfile : Profile
{
    public ListCustomersProfile()
    {
        CreateMap<Customer, ListCustomersResult>();
    }
}
