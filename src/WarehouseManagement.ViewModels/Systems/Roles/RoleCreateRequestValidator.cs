using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Roles
{
    public class RoleCreateRequestValidator : AbstractValidator<RoleCreateRequest>
    {
        public RoleCreateRequestValidator()
        {
            Include(new RoleBaseValidator<RoleCreateRequest>());
        }
    }
}
