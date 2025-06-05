using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public PaymentService(
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<PaymentTransaction> ProcessPaymentAsync(
        Guid tenantId,
        string transactionType,
        Guid referenceId,
        decimal amount,
        string paymentMethod,
        string userId)
    {
        var paymentTransaction = new PaymentTransaction
        {
            TenantId = tenantId,
            TransactionType = transactionType,
            ReferenceId = referenceId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentStatus = "Pending",
            TransactionDate = DateTime.UtcNow,
            CreatedBy = userId
        };

        // Start a transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var repository = _unitOfWork.Repository<PaymentTransaction>();
            await repository.AddAsync(paymentTransaction);

            // TODO: Integrate with payment gateway (Razorpay)
            // For now, simulate successful payment
            paymentTransaction.PaymentStatus = "Success";
            paymentTransaction.TransactionId = Guid.NewGuid().ToString();
            paymentTransaction.PaymentGatewayResponse = "Payment processed successfully";

            await repository.UpdateAsync(paymentTransaction);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();

            await _auditService.LogActivityAsync(
                "Payment",
                paymentTransaction.Id,
                "Process",
                userId);

            return paymentTransaction;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<PaymentTransaction?> GetTransactionByIdAsync(Guid transactionId)
    {
        var repository = _unitOfWork.Repository<PaymentTransaction>();
        return await repository.GetByIdAsync(transactionId);
    }

    public async Task<IEnumerable<PaymentTransaction>> GetTransactionHistoryAsync(
        Guid tenantId,
        string transactionType,
        DateTime startDate,
        DateTime endDate)
    {
        var repository = (IPaymentTransactionRepository)_unitOfWork.Repository<PaymentTransaction>();
        return await repository.GetTransactionsByDateRangeAsync(tenantId, startDate, endDate);
    }

    public async Task<decimal> GetTotalPaymentsAsync(
        Guid tenantId,
        string transactionType,
        DateTime startDate,
        DateTime endDate)
    {
        var repository = (IPaymentTransactionRepository)_unitOfWork.Repository<PaymentTransaction>();
        return await repository.GetTotalTransactionsAsync(tenantId, transactionType, startDate, endDate);
    }

    public async Task<bool> RefundPaymentAsync(Guid transactionId, string userId)
    {
        var repository = _unitOfWork.Repository<PaymentTransaction>();
        var transaction = await repository.GetByIdAsync(transactionId);

        if (transaction == null || transaction.PaymentStatus != "Success")
        {
            return false;
        }

        // Start a transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create refund transaction
            var refundTransaction = new PaymentTransaction
            {
                TenantId = transaction.TenantId,
                TransactionType = $"{transaction.TransactionType}Refund",
                ReferenceId = transaction.ReferenceId,
                Amount = -transaction.Amount, // Negative amount for refund
                PaymentMethod = transaction.PaymentMethod,
                PaymentStatus = "Pending",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = userId
            };

            await repository.AddAsync(refundTransaction);

            // TODO: Integrate with payment gateway for refund
            refundTransaction.PaymentStatus = "Success";
            refundTransaction.TransactionId = Guid.NewGuid().ToString();
            await repository.UpdateAsync(refundTransaction);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();

            await _auditService.LogActivityAsync(
                "Payment",
                refundTransaction.Id,
                "Refund",
                userId);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}
