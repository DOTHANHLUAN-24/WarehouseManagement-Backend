using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.ViewModels.Contents.Products;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ApplicationDbContext _context) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] ProductCreateRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Code = request.Code,
                IsActive = request.IsActive,
                CreateDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var variant = new ProductVariant
            {
                ProductId = product.Id,
                Name = product.Name,
                SKU = request.SKU,
                Price = request.Price,
                StockQuantity = request.InitialStock,
                IsActive = true,
                Status = ProductVariantStatus.Active,
                CreateDate = DateTime.UtcNow
            };

            _context.ProductVariants.Add(variant);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            else
                return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);


            if (product == null)
                return NotFound();

            var productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Code = product.Code,
                IsActive = product.IsActive,
            };

            return Ok(productViewModel);
        }

        [HttpPost("{productId}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImages(
            int productId,
            [FromForm] List<IFormFile> images)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == productId))
                return NotFound();

            var folder = Path.Combine("wwwroot", "images", "products");
            Directory.CreateDirectory(folder);

            var sortOrder = await _context.ProductImages
                .Where(x => x.ProductId == productId)
                .CountAsync();

            var productImages = new List<ProductImage>();


            foreach (var file in images)
            {
                bool isDefault = false;
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                if (sortOrder == 0)
                {
                    isDefault = true;
                }

                productImages.Add(new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = $"/images/products/{fileName}",
                    SortOrder = sortOrder++,
                    IsDefault = isDefault,
                    CreateDate = DateTime.UtcNow
                });
            }

            _context.ProductImages.AddRange(productImages);
            await _context.SaveChangesAsync();

            return Ok(productImages);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var productList = await (
                from p in _context.Products
                join pi in _context.ProductImages
                    on p.Id equals pi.ProductId
                where 
                    !p.IsDeleted
                   && !pi.IsDeleted
                select new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    Code = p.Code,
                    IsActive = p.IsActive,
                    ImageUrl = pi.ImageUrl,
                    IsDefault = pi.IsDefault
                }
            ).ToListAsync();

            return Ok(productList);
        }

        [HttpPut("{productId}/status")]
        public async Task<IActionResult> ChangeStatusProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            product.IsActive = !product.IsActive;
            product.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                product.Id,
                product.IsActive
            });
        }

        [HttpPut("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            if (product.IsDeleted)
                return BadRequest("Product already in trash.");

            product.IsDeleted = true;

            var productListImage = await _context.ProductImages
                .Where(x => x.ProductId == id).ToListAsync();

            foreach(var productImage in productListImage)
            {
                productImage.IsDeleted = true;
            }

            var productVariants = await _context.ProductVariants
                .Where(v => v.ProductId == id)
                .ToListAsync();

            foreach (var productVariant in productVariants)
            {
                productVariant.Status = ProductVariantStatus.Discontinued;
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok(product);
            else
                return BadRequest();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            product.IsDeleted = false;

            var productListImage = await _context.ProductImages
                 .Where(x => x.ProductId == id).ToListAsync();

            foreach (var productImage in productListImage)
            {
                productImage.IsDeleted = false;
            }

            var productVariants = await _context.ProductVariants
                .Where(v => v.ProductId == id)
                .ToListAsync();

            foreach (var productVariant in productVariants)
            {
                productVariant.Status = ProductVariantStatus.Active;
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return Ok(product);
            else
                return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> PermanentDeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            if (!product.IsDeleted)
                return BadRequest("Product must be soft-deleted before permanent deletion.");

            var productImages = await _context.ProductImages
                .Where(x => x.ProductId == id)
                .ToListAsync();

            foreach (var image in productImages)
            {
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        image.ImageUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
            }

            var productVariants = await _context.ProductVariants
                .Where(v => v.ProductId == id)
                .ToListAsync();

            _context.ProductVariants.RemoveRange(productVariants);
            _context.ProductImages.RemoveRange(productImages);
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                return NotFound();

            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    image.ImageUrl.TrimStart('/')
                );

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            image.IsDeleted = true;
            image.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                image.Id,
                image.IsDeleted
            });
        }

    }
}