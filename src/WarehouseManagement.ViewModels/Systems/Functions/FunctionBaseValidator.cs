using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Functions
{
    public class FunctionBaseValidator<T> : AbstractValidator<T>
        where T : FunctionBase
    {
        public FunctionBaseValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id value is required")
                .MaximumLength(50).WithMessage("Function id cannot over limit 50 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name value is required")
                .MaximumLength(200).WithMessage("Name cannot over limit 200 characters");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Url value is required")
                .MaximumLength(200).WithMessage("Url cannot over limit 200 characters");

            RuleFor(x => x.ParentId)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.ParentId))
                .WithMessage("ParentId cannot over limit 50 characters");
        }
    }
}
