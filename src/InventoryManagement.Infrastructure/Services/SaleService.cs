using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;
    private int _lastInvoiceNumber = 0;
    private readonly object _invoiceLock = new();

    public SaleService(
        IUnitOfWork unitOfWork,
        IInventoryService inventoryService,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _saleRepository = (ISaleRepository)unitOfWork.Repository<Sale>();
        _inventoryService = inventoryService;
        _auditService = auditService;
    }

    public async Task<Sale> CreateSaleAsync(Sale sale)
    {
        // Start a transaction since we're performing multiple operations
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Generate invoice number
            sale.InvoiceNumber = await GenerateInvoiceNumberAsync(sale.TenantId);

            // Calculate totals
            sale.SubTotal = sale.Items.Sum(item => item.Quantity * item.UnitPrice);
            sale.DiscountAmount = sale.Items.Sum(item => item.DiscountAmount);
            sale.TotalAmount = sale.SubTotal - sale.DiscountAmount;

            // Validate and update inventory for each item
            foreach (var item in sale.Items)
            {
                if (!await _inventoryService.IsProductAvailableAsync(item.ProductId, item.Quantity))
                {
                    throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
                }

                // Update inventory
                await _inventoryService.UpdateStockQuantityAsync(item.ProductId, item.Quantity, "remove");
            }

            // Save the sale
            var createdSale = await _saleRepository.AddAsync(sale);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();

            // Log the activity
            await _auditService.LogActivityAsync(
                "Sale",
                createdSale.Id,
                "Create",
                sale.CreatedBy);

            return createdSale;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<string> GenerateInvoiceNumberAsync(Guid tenantId)
    {
        lock (_invoiceLock)
        {
            var today = DateTime.UtcNow;
            var prefix = $"{tenantId.ToString()[..4]}-{today:yyMMdd}";
            _lastInvoiceNumber++;
            return $"{prefix}-{_lastInvoiceNumber:D4}";
        }
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(Guid saleId)
    {
        var sale = await _saleRepository.GetByIdAsync(saleId)
            ?? throw new InvalidOperationException($"Sale with ID {saleId} not found.");

        // TODO: Implement PDF generation
        throw new NotImplementedException("PDF generation not implemented yet");
    }

    public async Task<IEnumerable<Sale>> GetSalesReportAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _saleRepository.GetSalesByDateRangeAsync(tenantId, startDate, endDate);
    }

    public async Task<decimal> CalculateTotalRevenueAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _saleRepository.GetTotalSalesAsync(tenantId, startDate, endDate);
    }

    public async Task<bool> ProcessPaymentAsync(Sale sale, string paymentMethod)
    {
        // Start a transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create payment transaction
            var paymentTransaction = new PaymentTransaction
            {
                TenantId = sale.TenantId,
                TransactionType = "Sale",
                ReferenceId = sale.Id,
                Amount = sale.TotalAmount,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = sale.CreatedBy
            };

            var paymentRepo = _unitOfWork.Repository<PaymentTransaction>();
            await paymentRepo.AddAsync(paymentTransaction);

            // TODO: Integrate with payment gateway
            paymentTransaction.PaymentStatus = "Success";
            await paymentRepo.UpdateAsync(paymentTransaction);

            // Update sale status
            sale.PaymentStatus = "Paid";
            sale.AmountPaid = sale.TotalAmount;
            await _saleRepository.UpdateAsync(sale);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();

            await _auditService.LogActivityAsync(
                "Payment",
                paymentTransaction.Id,
                "Process",
                sale.CreatedBy);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
