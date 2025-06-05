using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetTransactionsByReferenceAsync(string transactionType, Guid referenceId)
    {
        return await _dbSet
            .Where(pt => 
                pt.TransactionType == transactionType && 
                pt.ReferenceId == referenceId && 
                !pt.IsDeleted)
            .OrderByDescending(pt => pt.TransactionDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetTransactionsByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(pt => 
                pt.TenantId == tenantId && 
                !pt.IsDeleted &&
                pt.TransactionDate >= startDate && 
                pt.TransactionDate <= endDate)
            .OrderByDescending(pt => pt.TransactionDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalTransactionsAsync(Guid tenantId, string transactionType, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(pt => 
                pt.TenantId == tenantId && 
                pt.TransactionType == transactionType && 
                !pt.IsDeleted &&
                pt.TransactionDate >= startDate && 
                pt.TransactionDate <= endDate &&
                pt.PaymentStatus == "Success")
            .SumAsync(pt => pt.Amount);
    }
}
