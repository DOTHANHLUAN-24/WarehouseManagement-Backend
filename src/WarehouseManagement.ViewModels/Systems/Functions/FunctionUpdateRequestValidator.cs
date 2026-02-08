using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Functions
{
    public class FunctionUpdateRequestValidator : AbstractValidator<FunctionUpdateRequest>
    {
        public FunctionUpdateRequestValidator() 
        {
            Include(new FunctionBaseValidator<FunctionUpdateRequest>());
        }
    }
}
