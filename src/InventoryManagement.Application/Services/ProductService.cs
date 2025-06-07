using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ProductDTO> GetByIdAsync(Guid id)
        {
            var cacheKey = $"product_{id}";
            var cachedProduct = await _cacheService.GetAsync<ProductDTO>(cacheKey);
            if (cachedProduct != null)
                return cachedProduct;

            var product = await _unitOfWork.Products
                .GetAll()
                .Include(p => p.CategoryType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            var productDto = _mapper.Map<ProductDTO>(product);
            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(30));

            return productDto;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _unitOfWork.Products
                .GetAll()
                .Include(p => p.CategoryType)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> CreateAsync(CreateProductDTO createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<ProductDTO> UpdateAsync(UpdateProductDTO updateProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(updateProductDto.Id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {updateProductDto.Id} not found");

            _mapper.Map(updateProductDto, product);
            
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"product_{product.Id}");

            return await GetByIdAsync(product.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return false;

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"product_{id}");

            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _unitOfWork.Products
                .GetAll()
                .AnyAsync(p => p.Id == id);
        }

        public async Task<int> GetStockQuantityAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            return product.StockQuantity;
        }

        public async Task UpdateStockQuantityAsync(Guid id, int quantity)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            product.StockQuantity = quantity;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"product_{id}");
        }
    }
}
