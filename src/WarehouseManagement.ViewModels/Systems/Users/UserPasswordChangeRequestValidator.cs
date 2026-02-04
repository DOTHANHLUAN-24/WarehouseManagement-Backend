using System.Collections.Generic;
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
                .MinimumLength(8).WithMessage("Current password has to at least 8 character")
                .Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
                .WithMessage("Current password is not match complexity rules.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("New password has to at least 8 character")
                .Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
                .WithMessage("New password is not match complexity rules.");
        }
    }
}
