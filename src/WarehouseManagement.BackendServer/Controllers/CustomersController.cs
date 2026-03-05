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
    public class CustomersController
        (
            ApplicationDbContext _context, 
            ILogger<CustomersController> _logger
        ) : BaseController
    {
        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="request">Customer model</param>
        /// <returns>Result of create process</returns>
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] CustomerCreateRequest request)
        {
            _logger.LogInformation("Begin PostCustomer API");

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
            {
                _logger.LogInformation("PostCustomer API success. Id={Id}", customer.Id);

                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            }
            else
            {
                _logger.LogError("PostCustomer API failed to save changes. Id={Id}", customer.Id);

                return BadRequest();
            }
        }

        /// <summary>
        /// Get a customer by id
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>Result of process</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Begin GetCustomerById. Id={id}", id);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Get Customer not found. Id={id}", id);

                return NotFound();
            }

            var customerViewModel = CreateCustomerViewModel(customer);

            _logger.LogInformation("Get Customer by id success. Id={Id}", id);

            return Ok(customerViewModel);
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns>List of customer</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            _logger.LogInformation("Begin GetCustomers API");

            var customers = _context.Customers;
            var customerViewModels = await customers
                .Select(customer => CreateCustomerViewModel(customer))
                .ToListAsync();

            _logger.LogInformation("Get Customers success. Total={Total}", customerViewModels.Count);
            return Ok(customerViewModels);
        }

        /// <summary>
        /// Get customers with paging and filter by name
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetCustomersPaging(string? filter, int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Begin GetCustomersPaging API. Filter={Filter}, PageIndex={PageIndex}, PageSize={PageSize}", filter, pageIndex, pageSize);

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                _logger.LogInformation("Apply filter to GetCustomersPaging API. Filter={Filter}", filter);

                query = query.Where(x => x.FullName.Contains(filter));
            }

            var totalRecords = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var data = items.Select(customer => CreateCustomerViewModel(customer)).ToList();

            var pagination = new Pagination<CustomerViewModel>
            {
                Items = data,
                TotalRecords = totalRecords,
            };

            _logger.LogInformation("GetCustomersPaging API success. TotalRecords={TotalRecords}, PageIndex={PageIndex}, PageSize={PageSize}", totalRecords, pageIndex, pageSize);

            return Ok(pagination);
        }

        /// <summary>
        /// Update a customer by id
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <param name="request">Customer model</param>
        /// <returns>Result of update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerUpdateRequest request)
        {
            _logger.LogInformation("Begin PutCustomer API. Id={Id}", id);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("PutCustomer API failed. Customer not found. Id={Id}", id);

                return NotFound();
            }

            customer.FullName = request.FullName;
            customer.PhoneNumber = request.PhoneNumber;
            customer.LastModifiedDate = DateTime.Now;

            _context.Customers.Update(customer);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("PutCustomer API success. Id={Id}", id);

                return NoContent();
            }

            _logger.LogError("PutCustomer API failed to save changes. Id={Id}", id);

            return BadRequest();
        }

        /// <summary>
        /// Update customer status by id
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <param name="status">Customer status</param>
        /// <returns>Result of update status process</returns>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCustomerStatus(int id, CustomerStatus status)
        {
            _logger.LogInformation("Begin UpdateCustomerStatus API. Id={Id}, Status={Status}", id, status);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("UpdateCustomerStatus API failed. Customer not found. Id={Id}", id);

                return NotFound();
            }

            if (customer.Status == CustomerStatus.Banned)
            {
                _logger.LogWarning("UpdateCustomerStatus API failed. Banned customer cannot change status. Id={Id}", id);

                return BadRequest("Banned customer cannot change status");
            }

            if (customer.Status == status)
            {
                _logger.LogWarning("UpdateCustomerStatus API failed. Status is already applied. Id={Id}, Status={Status}", id, status);

                return BadRequest("Status is already applied");
            }

            customer.Status = status;
            customer.LastModifiedDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("UpdateCustomerStatus API success. Id={Id}, Status={Status}", id, status);

                return NoContent();
            }

            _logger.LogError("UpdateCustomerStatus API failed to save changes. Id={Id}, Status={Status}", id, status);

            return BadRequest();
        }

        /// <summary>
        /// Delete a customer by id (set status to Inactive)
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>Result of delete process</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            _logger.LogInformation("Begin DeleteCustomer API. Id={Id}", id);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("DeleteCustomer API failed. Customer not found. Id={Id}", id);

                return NotFound();
            }

            if (customer.Status == CustomerStatus.Banned)
            {
                _logger.LogWarning("DeleteCustomer API failed. Banned customer cannot be deleted. Id={Id}", id);

                return BadRequest("Banned customer cannot be deleted");
            }

            if (customer.Status == CustomerStatus.Inactive)
            {
                _logger.LogWarning("DeleteCustomer API failed. Customer already deleted. Id={Id}", id);

                return BadRequest("Customer already deleted");
            }

            customer.Status = CustomerStatus.Inactive;
            customer.LastModifiedDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("DeleteCustomer API success. Id={Id}", id);

                return NoContent();
            }

            _logger.LogError("DeleteCustomer API failed to save changes. Id={Id}", id);

            return BadRequest();
        }

        /// <summary>
        /// Restore a customer by id (set status to Active)
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>Result of restore process</returns>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreCustomer(int id)
        {
            _logger.LogInformation("Begin RestoreCustomer API. Id={Id}", id);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("RestoreCustomer API failed. Customer not found. Id={Id}", id);

                return NotFound();
            }

            if (customer.Status == CustomerStatus.Banned)
            {
                _logger.LogWarning("RestoreCustomer API failed. Banned customer cannot be restored. Id={Id}", id);

                return BadRequest("Banned customer cannot be restored");
            }

            if (customer.Status == CustomerStatus.Active)
            {
                _logger.LogWarning("RestoreCustomer API failed. Customer already active. Id={Id}", id);

                return BadRequest("Customer already active");
            }

            customer.Status = CustomerStatus.Active;
            customer.LastModifiedDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("RestoreCustomer API success. Id={Id}", id);

                return NoContent();
            }

            _logger.LogError("RestoreCustomer API failed to save changes. Id={Id}", id);

            return BadRequest();
        }

        /// <summary>
        /// Filter customers by keyword, status and create date range with paging
        /// </summary>
        /// <param name="keyword">Keyword filter</param>
        /// <param name="status">Status filter</param>
        /// <param name="fromDate">Start date</param>
        /// <param name="toDate">End date</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Result of filter process</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers(string? keyword, CustomerStatus? status, DateTime? fromDate, DateTime? toDate, int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Begin SearchCustomers API. Keyword={Keyword}, Status={Status}, FromDate={FromDate}, ToDate={ToDate}, PageIndex={PageIndex}, PageSize={PageSize}", keyword, status, fromDate, toDate, pageIndex, pageSize);

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                _logger.LogInformation("Apply keyword filter to SearchCustomers API. Keyword={Keyword}", keyword);

                query = query.Where(x =>
                    EF.Functions.Like(x.FullName, $"%{keyword}%"));
            }
                

            if (status is not null)
            {
                _logger.LogInformation("Apply status filter to SearchCustomers API. Status={Status}", status);

                query = query.Where(x => x.Status == status);
            }

            if (fromDate is not null)
            {
                _logger.LogInformation("Apply fromDate filter to SearchCustomers API. FromDate={FromDate}", fromDate);

                query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
            }

            if (toDate is not null)
            {
                _logger.LogInformation("Apply toDate filter to SearchCustomers API. ToDate={ToDate}", toDate);

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

            _logger.LogInformation("SearchCustomers API success. TotalRecords={TotalRecords}, PageIndex={PageIndex}, PageSize={PageSize}", totalRecords, pageIndex, pageSize);

            return Ok(new Pagination<CustomerViewModel>
            {
                Items = data,
                TotalRecords = totalRecords
            });
        }

        /// <summary>
        /// Get customers by status with paging
        /// </summary>
        /// <param name="status">Filter status</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List customer filter by status</returns>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(
            CustomerStatus status,
            int pageIndex = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Begin GetByStatus API. Status={Status}, PageIndex={PageIndex}, PageSize={PageSize}", status, pageIndex, pageSize);

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

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

            _logger.LogInformation("GetByStatus API success. TotalRecords={TotalRecords}, PageIndex={PageIndex}, PageSize={PageSize}", totalRecords, pageIndex, pageSize);

            return Ok(new Pagination<CustomerViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            });
        }

        /// <summary>
        /// Check if a phone number already exists in the system
        /// </summary>
        /// <param name="phone">Phone check</param>
        /// <returns>Result of check process</returns>
        [HttpGet("check-phone")]
        public async Task<IActionResult> CheckPhoneExists(string phone)
        {
            _logger.LogInformation("Begin CheckPhoneExists API. Phone={Phone}", phone);

            var exists = await _context.Customers
                .AsNoTracking()
                .AnyAsync(x => x.PhoneNumber == phone);

            _logger.LogInformation("CheckPhoneExists API success. Phone={Phone}, Exists={Exists}", phone, exists);

            return Ok(new { exists });
        }

        /// <summary>
        /// Create a CustomerViewModel from a Customer entity
        /// </summary>
        /// <param name="customer">Customer model</param>
        /// <returns>Customer view model</returns>
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
