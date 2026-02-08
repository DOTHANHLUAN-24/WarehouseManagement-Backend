using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Roles
{
    public class RoleBaseValidator<T> : AbstractValidator<T> 
        where T : RoleBase
    {
        public RoleBaseValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id value is required!")
                .MaximumLength(50).WithMessage("Role id can't over limit 50 characters!");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name value is required!");
        }
    }
}
