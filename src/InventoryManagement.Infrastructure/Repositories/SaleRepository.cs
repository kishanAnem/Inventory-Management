using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class SaleRepository : GenericRepository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Sale>> GetSalesByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(s => s.Items)
                .ThenInclude(si => si.Product)
            .Include(s => s.Customer)
            .Where(s => s.CustomerId == customerId && !s.IsDeleted)
            .OrderByDescending(s => s.InvoiceDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Sale>> GetSalesByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(s => s.Items)
                .ThenInclude(si => si.Product)
            .Include(s => s.Customer)
            .Where(s => 
                s.TenantId == tenantId && 
                !s.IsDeleted &&
                s.InvoiceDate >= startDate && 
                s.InvoiceDate <= endDate)
            .OrderByDescending(s => s.InvoiceDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalSalesAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(s => 
                s.TenantId == tenantId && 
                !s.IsDeleted &&
                s.InvoiceDate >= startDate && 
                s.InvoiceDate <= endDate)
            .SumAsync(s => s.TotalAmount);
    }
}
