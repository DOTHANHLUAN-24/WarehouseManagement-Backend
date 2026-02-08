using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Functions
{
    public class FunctionCreateRequestValidator : AbstractValidator<FunctionCreateRequest>
    {
        public FunctionCreateRequestValidator() 
        {
            Include(new FunctionBaseValidator<FunctionCreateRequest>());
        }
    }
}
