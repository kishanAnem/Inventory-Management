using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IPaymentService _paymentService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IProductService _productService;

        public SaleService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IPaymentService paymentService,
            IWhatsAppService whatsAppService,
            IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _paymentService = paymentService;
            _whatsAppService = whatsAppService;
            _productService = productService;
        }

        public async Task<SaleDTO> GetByIdAsync(Guid id)
        {
            var cacheKey = $"sale_{id}";
            var cachedSale = await _cacheService.GetAsync<SaleDTO>(cacheKey);
            if (cachedSale != null)
                return cachedSale;

            var sale = await _unitOfWork.Sales
                .GetAll()
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {id} not found");

            var saleDto = _mapper.Map<SaleDTO>(sale);
            await _cacheService.SetAsync(cacheKey, saleDto, TimeSpan.FromMinutes(30));

            return saleDto;
        }

        public async Task<IEnumerable<SaleDTO>> GetAllAsync()
        {
            var sales = await _unitOfWork.Sales
                .GetAll()
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleDTO>>(sales);
        }

        public async Task<SaleDTO> CreateAsync(CreateSaleDTO createSaleDto)
        {
            await ValidateAndUpdateInventory(createSaleDto);

            var sale = _mapper.Map<Sale>(createSaleDto);
            sale.SaleDate = DateTime.UtcNow;
            sale.InvoiceNumber = await GenerateInvoiceNumber();

            // Calculate total amount
            sale.TotalAmount = sale.Items.Sum(item => item.Quantity * item.UnitPrice);

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            // Create payment order
            var orderId = await _paymentService.CreateOrderAsync(
                sale.TotalAmount,
                "INR",
                sale.InvoiceNumber
            );

            // Send WhatsApp notification to customer
            var customer = await _unitOfWork.Customers.GetByIdAsync(sale.CustomerId);
            if (customer != null && !string.IsNullOrEmpty(customer.Phone))
            {
                await _whatsAppService.SendOrderUpdateAsync(
                    customer.Phone,
                    sale.InvoiceNumber,
                    "created"
                );
            }

            return await GetByIdAsync(sale.Id);
        }

        public async Task<bool> CancelAsync(Guid id)
        {
            var sale = await _unitOfWork.Sales
                .GetAll()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return false;

            // Restore inventory
            foreach (var item in sale.Items)
            {
                await _productService.UpdateStockQuantityAsync(
                    item.ProductId,
                    (await _productService.GetStockQuantityAsync(item.ProductId)) + item.Quantity
                );
            }

            sale.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"sale_{id}");

            return true;
        }

        public async Task<IEnumerable<SaleDTO>> GetCustomerSalesAsync(Guid customerId)
        {
            var sales = await _unitOfWork.Sales
                .GetAll()
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Where(s => s.CustomerId == customerId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleDTO>>(sales);
        }

        public async Task<IEnumerable<SaleDTO>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _unitOfWork.Sales
                .GetAll()
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SaleDTO>>(sales);
        }

        public async Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Sales
                .GetAll()
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .SumAsync(s => s.TotalAmount);
        }

        private async Task ValidateAndUpdateInventory(CreateSaleDTO createSaleDto)
        {
            foreach (var item in createSaleDto.Items)
            {
                var currentStock = await _productService.GetStockQuantityAsync(item.ProductId);
                if (currentStock < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product {item.ProductId}. Available: {currentStock}, Requested: {item.Quantity}");
                }

                await _productService.UpdateStockQuantityAsync(
                    item.ProductId,
                    currentStock - item.Quantity
                );
            }
        }

        private async Task<string> GenerateInvoiceNumber()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var count = await _unitOfWork.Sales
                .GetAll()
                .CountAsync(s => s.InvoiceNumber.StartsWith(date));

            return $"{date}-{(count + 1).ToString().PadLeft(4, '0')}";
        }
    }
}
