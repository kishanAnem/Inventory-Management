using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Customer>> GetCustomersByTenantAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && !c.IsDeleted && c.IsActive)
            .ToListAsync();
    }

    public async Task<Customer?> GetByMobileNumberAsync(string mobileNumber, Guid tenantId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => 
                c.TenantId == tenantId && 
                c.Mobile == mobileNumber && 
                !c.IsDeleted);
    }

    public async Task<IReadOnlyList<Customer>> SearchCustomersAsync(string searchTerm, Guid tenantId)
    {
        return await _dbSet
            .Where(c => 
                c.TenantId == tenantId && 
                !c.IsDeleted &&
                (c.Name.Contains(searchTerm) || 
                c.Mobile.Contains(searchTerm) || 
                c.Email!.Contains(searchTerm)))
            .ToListAsync();
    }
}
