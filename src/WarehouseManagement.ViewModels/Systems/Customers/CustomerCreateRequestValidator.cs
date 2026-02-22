using FluentValidation;

namespace WarehouseManagement.ViewModels.Systems.Customers
{
    public class CustomerCreateRequestValidator : AbstractValidator<CustomerCreateRequest>
    {
        public CustomerCreateRequestValidator()
        {
            Include(new CustomerBaseValidator<CustomerCreateRequest>());
        }
    }
}
