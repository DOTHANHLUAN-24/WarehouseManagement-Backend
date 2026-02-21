using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Customers
{
    public class CustomerBaseValidator<T> : AbstractValidator<T>
    where T : CustomerBase
    {
        public CustomerBaseValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
               .Matches("^(\\+84|0)\\d{9,10}$").WithMessage("Phone number format is not valid");
        }
    }
}
