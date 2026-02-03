using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Role
{
    public class RoleCreateRequestValidator : AbstractValidator<RoleCreateRequest>
    {
        public RoleCreateRequestValidator()
        {
            Include(new RoleBaseValidator<RoleCreateRequest>());
        }
    }
}
