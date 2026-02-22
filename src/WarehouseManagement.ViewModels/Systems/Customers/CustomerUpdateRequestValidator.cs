using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Customers
{
    public class CustomerUpdateRequestValidator : AbstractValidator<CustomerCreateRequest>
    {
        public CustomerUpdateRequestValidator()
        {
            Include(new CustomerBaseValidator<CustomerCreateRequest>());
        }
    }
}
