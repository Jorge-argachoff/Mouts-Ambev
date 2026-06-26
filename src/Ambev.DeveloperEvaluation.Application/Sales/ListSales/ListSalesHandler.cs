using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesCommand requests, applying filtering,
/// ordering and pagination as described in the General API conventions.
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ListSalesResult> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var query = _saleRepository.Query();

        if (request.CustomerId.HasValue)
            query = query.Where(s => s.CustomerId == request.CustomerId.Value);

        if (!string.IsNullOrWhiteSpace(request.BranchId))
            query = query.Where(s => s.BranchId == request.BranchId);

        if (request.IsCancelled.HasValue)
            query = query.Where(s => s.IsCancelled == request.IsCancelled.Value);

        query = ApplyOrdering(query, request.Order);

        var totalCount = await query.CountAsync(cancellationToken);

        var page = request.Page < 1 ? 1 : request.Page;
        var size = request.Size < 1 ? 10 : request.Size;

        var sales = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new ListSalesResult
        {
            Items = _mapper.Map<List<GetSaleResult>>(sales),
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)size),
            TotalCount = totalCount
        };
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(s => s.SaleDate);

        IOrderedQueryable<Sale>? ordered = null;

        foreach (var clause in order.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = clause.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var field = parts[0].ToLowerInvariant();
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            var isFirst = ordered == null;

            ordered = (field, descending, isFirst) switch
            {
                ("salenumber", false, true) => query.OrderBy(s => s.SaleNumber),
                ("salenumber", true, true) => query.OrderByDescending(s => s.SaleNumber),
                ("salenumber", false, false) => ordered!.ThenBy(s => s.SaleNumber),
                ("salenumber", true, false) => ordered!.ThenByDescending(s => s.SaleNumber),

                ("totalamount", false, true) => query.OrderBy(s => s.TotalAmount),
                ("totalamount", true, true) => query.OrderByDescending(s => s.TotalAmount),
                ("totalamount", false, false) => ordered!.ThenBy(s => s.TotalAmount),
                ("totalamount", true, false) => ordered!.ThenByDescending(s => s.TotalAmount),

                ("customername", false, true) => query.OrderBy(s => s.CustomerName),
                ("customername", true, true) => query.OrderByDescending(s => s.CustomerName),
                ("customername", false, false) => ordered!.ThenBy(s => s.CustomerName),
                ("customername", true, false) => ordered!.ThenByDescending(s => s.CustomerName),

                ("branchname", false, true) => query.OrderBy(s => s.BranchName),
                ("branchname", true, true) => query.OrderByDescending(s => s.BranchName),
                ("branchname", false, false) => ordered!.ThenBy(s => s.BranchName),
                ("branchname", true, false) => ordered!.ThenByDescending(s => s.BranchName),

                (_, false, true) => query.OrderBy(s => s.SaleDate),
                (_, true, true) => query.OrderByDescending(s => s.SaleDate),
                (_, false, false) => ordered!.ThenBy(s => s.SaleDate),
                (_, true, false) => ordered!.ThenByDescending(s => s.SaleDate),
            };
        }

        return ordered ?? query.OrderByDescending(s => s.SaleDate);
    }
}
