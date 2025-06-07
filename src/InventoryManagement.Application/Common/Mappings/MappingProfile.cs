using AutoMapper;
using InventoryManagement.Core.Entities;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryType.Name));
            CreateMap<CreateProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();

            CreateMap<Customer, CustomerDTO>();
            CreateMap<CreateCustomerDTO, Customer>();
            CreateMap<UpdateCustomerDTO, Customer>();

            CreateMap<Sale, SaleDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));
            CreateMap<CreateSaleDTO, Sale>();

            CreateMap<SaleItem, SaleItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<CreateSaleItemDTO, SaleItem>();
        }
    }
}
