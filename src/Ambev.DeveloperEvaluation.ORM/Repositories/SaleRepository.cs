using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        // Items are always recreated with fresh Ids when a sale is updated (see UpdateSaleHandler).
        // Because their Id is already set to a non-default value before being attached to the
        // already-tracked Sale's collection, EF Core's change detection infers them as Unchanged/
        // Modified instead of Added, generating UPDATE statements for rows that don't exist yet
        // and causing a DbUpdateConcurrencyException. Force the correct state explicitly.
        foreach (var item in sale.Items)
        {
            var entry = _context.Entry(item);
            if (entry.State is EntityState.Unchanged or EntityState.Modified or EntityState.Detached)
                entry.State = EntityState.Added;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<Sale> Query()
    {
        return _context.Sales.Include(s => s.Items).AsQueryable();
    }
}
