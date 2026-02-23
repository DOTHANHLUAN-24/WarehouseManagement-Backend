using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserPasswordChangeRequestValidator : AbstractValidator<UserPasswordChangeRequest>
    {
        public UserPasswordChangeRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required!")
                .MaximumLength(50).WithMessage("User id has to at least 50 character");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required")
                .MinimumLength(8).WithMessage("Current password has to be at least 8 characters")
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*\-]).+$")
                .WithMessage("Current password must contain upper, lower, number and special character.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("New password has to be at least 8 characters")
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*\-]).+$")
                .WithMessage("New password must contain upper, lower, number and special character.");
        }
    }
}
