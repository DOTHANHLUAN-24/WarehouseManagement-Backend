using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateRequestValidator()
        {
            Include(new UserBaseValidator<UserCreateRequest>());

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password has to be at least 8 characters")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*\-]).+$")
            .WithMessage("Password must contain upper, lower, number and special character.");
        }
    }
}
