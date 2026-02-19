using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.Customers;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ApplicationDbContext _context, ILogger<CustomersController> _logger) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] CustomerCreateRequest request)
        {
            var customer = new Customer
            {
                UserId = String.Empty, // Todo: Get user id
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                CreateDate = DateTime.Now
            };

            _context.Customers.Add(customer);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            else
                return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            var customerViewModel = CreateCustomerViewModel(customer);

            return Ok(customerViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = _context.Customers;
            var customerViewModels = await customers
                .Select(customer => CreateCustomerViewModel(customer))
                .ToListAsync();

            return Ok(customerViewModels);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetCustomersPaging(string? filter, int pageIndex, int pageSize)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.FullName.ToLower().Contains(filter.ToLower()));

            var totalRecords = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var data = items.Select(customer => CreateCustomerViewModel(customer)).ToList();

            var pagination = new Pagination<CustomerViewModel>
            {
                Items = data,
                TotalRecords = totalRecords,
            };

            return Ok(pagination);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerUpdateRequest request)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            customer.FullName = request.FullName;
            customer.PhoneNumber = request.PhoneNumber;
            customer.LastModifiedDate = DateTime.Now;

            _context.Customers.Update(customer);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCustomerStatus(int id, CustomerStatus status)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            if (customer.Status == CustomerStatus.Banned)
                return BadRequest("Banned customer cannot change status");

            if (customer.Status == status)
                return BadRequest("Status is already applied");

            customer.Status = status;
            customer.LastModifiedDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();
            if(result > 0) return NoContent();
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            if (customer.Status == CustomerStatus.Banned)
                return BadRequest("Banned customer cannot be deleted");

            if (customer.Status == CustomerStatus.Inactive)
                return BadRequest("Customer already deleted");

            customer.Status = CustomerStatus.Inactive;
            customer.LastModifiedDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();
            if (result > 0) return NoContent();
            return BadRequest();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            if (customer.Status == CustomerStatus.Banned)
                return BadRequest("Banned customer cannot be restored");

            if (customer.Status == CustomerStatus.Active)
                return BadRequest("Customer already active");

            customer.Status = CustomerStatus.Active;
            customer.LastModifiedDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();
            if (result > 0) return NoContent();
            return BadRequest();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers(string? keyword, CustomerStatus? status, DateTime? fromDate, DateTime? toDate, int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x =>
                    EF.Functions.Like(x.FullName, $"%{keyword}%"));

            if (status is not null)
                query = query.Where(x => x.Status == status);

            if (fromDate is not null)
                query = query.Where(x => x.CreateDate >= fromDate.Value.Date);

            if (toDate is not null)
            {
                var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreateDate <= endDate);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreateDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = items
                .Select(CreateCustomerViewModel)
                .ToList();

            return Ok(new Pagination<CustomerViewModel>
            {
                Items = data,
                TotalRecords = totalRecords
            });
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(
            CustomerStatus status,
            int pageIndex = 1,
            int pageSize = 10)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Customers
                .AsNoTracking()
                .Where(x => x.Status == status);

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreateDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => CreateCustomerViewModel(x))
                .ToListAsync();

            return Ok(new Pagination<CustomerViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            });
        }

        [HttpGet("check-phone")]
        public async Task<IActionResult> CheckPhoneExists(string phone)
        {
            var exists = await _context.Customers
                .AsNoTracking()
                .AnyAsync(x => x.PhoneNumber == phone);

            return Ok(new { exists });
        }

        private static CustomerViewModel CreateCustomerViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
            };
        }
    }
}
