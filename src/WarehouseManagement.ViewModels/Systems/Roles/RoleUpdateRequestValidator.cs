using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Roles
{
    public class RoleUpdateRequestValidator : AbstractValidator<RoleUpdateRequest>
    {
        public RoleUpdateRequestValidator()
        {
            Include(new RoleBaseValidator<RoleUpdateRequest>());
        }
    }
}
