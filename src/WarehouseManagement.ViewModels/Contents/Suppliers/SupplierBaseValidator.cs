using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Suppliers
{
    public class SupplierBaseValidator<T> : AbstractValidator<T>
        where T : SupplierBase
    {
        public SupplierBaseValidator()
        {
            RuleFor(x => x.SupplierName)
                .NotEmpty().WithMessage("Supplier name is required")
                .MinimumLength(2).WithMessage("Supplier name must be at least 2 characters")
                .MaximumLength(200).WithMessage("Supplier name can not exceed 200 characters");

            RuleFor(x => x.ContactPerson)
                .MaximumLength(100).WithMessage("Contact person can not exceed 100 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?\d{8,15}$").WithMessage("Phone number must contain only numbers");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(500).WithMessage("Address can not exceed 500 characters");

            RuleFor(x => x.Email)
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                .WithMessage("Email format is not match")
                .Unless(x => string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive is required");

            RuleFor(x => x.IsDeleted)
                .NotNull().WithMessage("IsDeleted is required");
        }
    }
}
