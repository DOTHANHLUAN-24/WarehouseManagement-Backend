using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Categories;
using WarehouseManagement.ViewModels.Contents.Products;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ApplicationDbContext _context, ILogger<CategoriesController> _logger) : BaseController
    {
        /// <summary>
        /// Create a new Category
        /// </summary>
        /// <param name="request">Category model</param>
        /// <returns>Results of the add process</returns>
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryCreateRequest request)
        {
            _logger.LogInformation("Begin PostCategory API");

            var category = new Category
            {
                Name = request.Name,
                ParentId = request.ParentId,
                SeoAlias = request.SeoAlias,
                SeoDescription = request.SeoDescription,
                SortOrder = request.SortOrder,
            };

            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("PostCategory API success. Id={Id}", category.Id);

                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
            }
            else
            {
                _logger.LogError("PostCategory API failed to save changes");

                return BadRequest();
            }
        }

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>The category with the id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Begin GetCategoryById. Id={Id}", id);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Get Category not found. Id={id}", id);

                return NotFound();
            }

            var categoryViewModel = CreateCategoryViewModel(category);

            _logger.LogInformation("Get Category by id success. Id={id}", id);

            return Ok(categoryViewModel);
        }

        /// <summary>
        /// Get all categories in the system
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("Begin GetCategories API");

            var categories = _context.Categories;
            var categoryViewModels = await categories
                .Select(category => CreateCategoryViewModel(category)).ToListAsync();

            _logger.LogInformation("GetCategories API success to get all categories in system.");

            return Ok(categoryViewModels);
        }

        /// <summary>
        /// Get paged categories filtered by id or name.
        /// </summary>
        /// <param name="filter">Search keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns>Categories list filtered by keyword</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetCategoriesPaging(string? filter, int pageIndex, int pageSize)
        {
            _logger.LogInformation("Begin GetCategoriesPaging API. Filter = {filter}", filter);

            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                _logger.LogInformation("GetCategoriesPaging with filter applied. Filter={Filter}", filter);

                query = query.Where(x => x.Name.ToLower().Contains(filter));
            }

            var totalRecords = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var data = items.Select(category => CreateCategoryViewModel(category)).ToList();

            var pagination = new Pagination<CategoryViewModel>
            {
                Items = data,
                TotalRecords = totalRecords
            };

            _logger.LogInformation("GetCategoriesPaging API success to find all categories container keyword: {filter}.", filter);

            return Ok(pagination);
        }

        /// <summary>
        /// Get all product by category id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>List of products dependency category</returns>
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetAllProduct(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Get all product api: Category not found. Id={Id}", id);

                return NotFound();
            }

            var listProductInCategory = await _context.Products
                .Where(x => x.CategoryId == id)
                .Select(product => new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = id,
                    IsDeleted = product.IsDeleted,
                }).ToListAsync();

            return Ok(listProductInCategory);
        }

        /// <summary>
        /// Update a category by id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <param name="request">Category model</param>
        /// <returns>Results of the update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryUpdateRequest request)
        {
            _logger.LogInformation("Begin PutCategory API. Id={Id}", id);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("PUT Category not found. Id={Id}", id);

                return NotFound();
            }

            if (id == request.ParentId)
            {
                return BadRequest();
            }

            category.Name = request.Name;
            category.ParentId = request.ParentId;
            category.SortOrder = request.SortOrder;
            category.SeoAlias = request.SeoAlias;
            category.SeoDescription = request.SeoDescription;


            _context.Categories.Update(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("PUT Category success. Id={Id}", id);

                return NoContent();
            }

            _logger.LogError("PUT Category failed to save changes. Id={Id}", id);

            return BadRequest();
        }

        /// <summary>
        /// Permanently deletes a category from the system.
        /// This action cannot be undone.
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>
        /// The deleted category information if the operation succeeds;
        /// otherwise, an appropriate error response.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> PermanentDeleteCategory(int id)
        {
            _logger.LogInformation("Begin PermanentDeleteCategory API. Id={id}", id);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Permanent delete category failed. Category not found. Id={id}", id);

                return NotFound();
            }

            if (!category.IsDeleted)
            {
                _logger.LogWarning("Permanent delete category rejected. Category is not soft-deleted yet. Id={id}", id);

                return BadRequest("Category must be soft-deleted before permanent deletion.");
            }


            _context.Categories.Remove(category);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Permanent delete category succeeded. Id={id}", id);

                var categoryViewModel = CreateCategoryViewModel(category);
                return Ok(categoryViewModel);
            }

            _logger.LogError("Permanent delete category failed. Database did not persist changes. Id={id}", id);

            return BadRequest();
        }

        /// <summary>
        /// Move category to trash (soft delete).
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>Result of soft delete</returns>
        [HttpDelete("{id}/trash")]
        public async Task<IActionResult> SoftDeleteCategory(int id)
        {
            _logger.LogInformation("Begin SoftDeleteCategory. Id={id}", id);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Soft delete failed. Category not found. Id={id}", id);

                return NotFound();
            }

            if (category.IsDeleted)
            {
                _logger.LogInformation("Category already soft deleted. Id={id}", id);

                return BadRequest("Category already in trash.");
            }

            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Soft delete category success. Id={id}", id);

            return Ok();
        }

        /// <summary>
        /// Restore category in the trash
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>Results of process</returns>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            _logger.LogInformation("Begin RestoreCategory. Id={id}", id);

            var category = await _context.Categories
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            if (!category.IsDeleted)
                return BadRequest("Category is not in trash.");

            category.IsDeleted = false;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Restore category success. Id={id}", id);
            return Ok();
        }

        /// <summary>
        /// Create a new Category as Category View Model
        /// </summary>
        /// <param name="category">Category entity</param>
        /// <returns>Category View Model</returns>
        private static CategoryViewModel CreateCategoryViewModel(Category category)
        {
            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                SortOrder = category.SortOrder,
                SeoAlias = category.SeoAlias,
                SeoDescription = category.SeoDescription,
                IsDeleted = category.IsDeleted
            };
        }
    }
}
