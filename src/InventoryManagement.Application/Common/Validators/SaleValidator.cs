using FluentValidation;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Common.Validators
{
    public class SaleItemValidator : AbstractValidator<CreateSaleItemDTO>
    {
        public SaleItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThan(0);

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0);
        }
    }

    public class SaleValidator : AbstractValidator<CreateSaleDTO>
    {
        public SaleValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required");

            RuleForEach(x => x.Items)
                .SetValidator(new SaleItemValidator());
        }
    }
}
