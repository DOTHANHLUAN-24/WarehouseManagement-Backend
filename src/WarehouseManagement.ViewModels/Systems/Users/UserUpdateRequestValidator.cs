using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateRequestValidator()
        {
            RuleFor(x => x.FirstName)
               .NotEmpty().WithMessage("First name is required")
               .MaximumLength(50).WithMessage("First name cannot over limit 50 character");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot over limit 50 character");
        }
    }
}
