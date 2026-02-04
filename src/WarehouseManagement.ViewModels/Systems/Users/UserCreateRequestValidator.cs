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
                .MinimumLength(8).WithMessage("Password has to at least 8 character")
                .Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
                .WithMessage("Password is not match complexity rules.");
        }
    }
}
