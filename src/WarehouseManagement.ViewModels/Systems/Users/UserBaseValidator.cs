using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserBaseValidator<T> : AbstractValidator<T>
        where T : UserBase
    {
        public UserBaseValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email format is not match");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches("^(\\+84|0)\\d{9,10}$").WithMessage("Phone number format is not valid");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot over limit 50 character");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot over limit 50 character");
        }
    }
}
