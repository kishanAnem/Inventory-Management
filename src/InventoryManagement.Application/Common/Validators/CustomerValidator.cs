using FluentValidation;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Common.Validators
{
    public class CustomerValidator : AbstractValidator<CreateCustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Address)
                .MaximumLength(200);
        }
    }

    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDTO>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            Include(new CustomerValidator());
        }
    }
}
