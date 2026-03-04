using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Warehouses
{
    public class WarehouseBaseValidator<T> : AbstractValidator<T>
        where T : WarehouseBase
    {
        public WarehouseBaseValidator()
        {
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Warehouse location is required.")
                .MaximumLength(200).WithMessage("Warehouse location must not exceed 200 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Warehouse capacity must be greater than zero.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email format is not match");
        }
    }
}
