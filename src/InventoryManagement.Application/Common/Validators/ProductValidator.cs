using FluentValidation;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Common.Validators
{
    public class ProductValidator : AbstractValidator<CreateProductDTO>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.SKU)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CategoryTypeId)
                .NotEmpty();
        }
    }

    public class UpdateProductValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            Include(new ProductValidator());
        }
    }
}
