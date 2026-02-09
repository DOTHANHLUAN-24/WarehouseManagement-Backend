using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.Functions;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionsController(ApplicationDbContext _context, ILogger<FunctionsController> _logger) : BaseController
    {
        /// <summary>
        /// Create a new function
        /// </summary>
        /// <param name="request">Function model</param>
        /// <returns>Results of the add process</returns>
        [HttpPost]
        public async Task<IActionResult> PostFunction([FromBody] FunctionCreateRequest request)
        {
            _logger.LogInformation("Begin PostFunction API");

            var function = new Function
            {
                Id = request.Id,
                Name = request.Name,
                Url = request.Url,
                SortOrder = request.SortOrder,
                ParentId = request.ParentId,
                Icon = request.Icon
            };

            _context.Functions.Add(function);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("PostFunction API success. Id={Id}", function.Id);

                return CreatedAtAction( nameof(GetById), new { id = function.Id }, function );
            }

            _logger.LogError("PostFunction API failed to save changes. Id={Id}", request.Id);

            return BadRequest();
        }

        /// <summary>
        /// Get a function by id
        /// </summary>
        /// <param name="id">Function id</param>
        /// <returns>The function with the id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.LogInformation("Begin GetFunctionById. Id={id}", id);

            var function = await _context.Functions.FindAsync(id);
            if (function == null)
            {
                _logger.LogWarning("Get Function not found. Id={id}", id);

                return NotFound();
            }

            var functionViewModel = CreateFunctionViewModel(function);

            _logger.LogInformation( "Get Function by id success. Id={Id}", id );

            return Ok(functionViewModel);
        }

        /// <summary>
        /// Get all functions in the system
        /// </summary>
        /// <returns>List of functions</returns>
        [HttpGet]
        public async Task<IActionResult> GetFunctions()
        {
            _logger.LogInformation("Begin GetFunctions API");

            var functions = _context.Functions;
            var functionViewModels = await functions
                .Select(f => CreateFunctionViewModel(f)).ToListAsync();

            _logger.LogInformation("GetFunctions API success to get all functions in system.");

            return Ok(functionViewModels);
        }

       

        /// <summary>
        /// Get paged functions filtered by id or name.
        /// </summary>
        /// <param name="filter">Search keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns>Functions list filtered by keyword</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetFunctionsPaging(string? filter, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Begin GetFunctionsPaging API. Filter = {filter}", filter);

            var query = _context.Functions.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                _logger.LogInformation( "GetFunctionsPaging with filter applied. Filter={Filter}", filter);

                query = query.Where(x => x.Id.ToLower().Contains(filter)
                || x.Name.ToLower().Contains(filter));
            }

            var totalRecord = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(f => CreateFunctionViewModel(f))
                .ToListAsync();

            var pagination = new Pagination<FunctionViewModel>
            {
                Items = items,
                TotalRecords = totalRecord
            };

            _logger.LogInformation("GetFunctionsPaging API success to find all functions container keyword: {filter}.", filter);

            return Ok(pagination);
        }

        /// <summary>
        /// Update a function by id
        /// </summary>
        /// <param name="id">Function id</param>
        /// <param name="request">Function model</param>
        /// <returns>Results of the update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFunction(string id, [FromBody] FunctionUpdateRequest request)
        {
            _logger.LogInformation("Begin PutFunction API. Id={Id}", id);

            if (id != request.Id)
            {
                _logger.LogWarning("PUT Function id mismatch. RouteId={RouteId}, BodyId={BodyId}", id, request.Id);

                return BadRequest("Route id and body id do not match.");
            }

            var function = await _context.Functions.FindAsync(id);
            if (function == null)
            {
                _logger.LogInformation("PUT Function not found. Id={Id}", id);
                return NotFound();
            }

            function.Name = request.Name;
            function.ParentId = request.ParentId;
            function.SortOrder = request.SortOrder;
            function.Url = request.Url;
            function.Icon = request.Icon;

            _context.Functions.Update(function);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation( "PUT Function success. Id={Id}", id );

                return NoContent();
            }

            _logger.LogError( "PUT Function failed to save changes. Id={Id}", id );

            return BadRequest();
        }

        /// <summary>
        /// Delete a function by id
        /// </summary>
        /// <param name="id">Function id</param>
        /// <returns>Results of the delete process</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFunction(string id)
        {
            _logger.LogInformation("Begin DeleteFunction API. Id={id}", id);

            var function = await _context.Functions.FindAsync(id);
            if (function == null)
            {
                _logger.LogWarning("Delete function failed, can't not find the function. Id={id}", id);
                
                return NotFound();
            }


            _context.Functions.Remove(function);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Delete function success. Id={id}", id);

                var functionViewModel = CreateFunctionViewModel(function);
                return Ok(functionViewModel);
            }

            _logger.LogError("DELETE Function failed to delete function. Id={id}", id);

            return BadRequest();
        }

        /// <summary>
        /// Create a new Function View Model
        /// </summary>
        /// <param name="f">Function entity</param>
        /// <returns>Function View Model</returns>
        private static FunctionViewModel CreateFunctionViewModel(Function f)
        {
            return new FunctionViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Url = f.Url,
                SortOrder = f.SortOrder,
                ParentId = f.ParentId!,
                Icon = f.Icon
            };
        }
    }
}
