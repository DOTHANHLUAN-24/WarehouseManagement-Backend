using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.ViewModels.Contents.Products;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ApplicationDbContext _context) : BaseController
    {

        #region Get - Query

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await (
                from p in _context.Products.AsNoTracking()
                join pv in _context.ProductVariants.AsNoTracking()
                    on p.Id equals pv.ProductId
                where p.Id == id
                      && !p.IsDeleted
                      && pv.IsActive
                select new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    Code = p.Code,
                    IsActive = p.IsActive,

                    Price = pv.Price,
                    Quantity = pv.StockQuantity,

                    ImageUrl = _context.ProductImages
                        .Where(i => i.ProductId == p.Id)
                        .OrderBy(i => i.Id)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()!
                }
            ).FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }


        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetAllProductByCategory(int categoryId)
        {
            var listProduct = await _context.Products
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();

            foreach (var product in listProduct)
            {
                var listProductImage = await _context.ProductImages
                    .Where(x => x.ProductId == product.Id && x.IsDefault == true)
                    .ToListAsync();
            }

            return Ok(listProduct);
        }

        [HttpGet("price-between")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var products = await (
                from pv in _context.ProductVariants
                join p in _context.Products on pv.ProductId equals p.Id
                join pi in _context.ProductImages on pv.ProductId equals pi.ProductId
                where pv.Price >= minPrice && pv.Price <= maxPrice && pi.IsDefault == true
                select new
                {
                    p.Id,
                    p.Name,
                    p.Code,
                    pi.ImageUrl,
                    pv.Price,
                    pv.StockQuantity,
                }
            ).Distinct().ToListAsync();

            return Ok(products);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetProductsPaging(string? filter, int pageIndex, int pageSize)
        {
            var query =
                from p in _context.Products
                join pv in _context.ProductVariants on p.Id equals pv.ProductId
                select new
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Price = pv.Price,
                    Quantity = pv.StockQuantity,
                    Description = p.Description,
                    ImageUrl = _context.ProductImages
                        .Where(i => i.ProductId == p.Id)
                        .OrderBy(i => i.Id)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                };


            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Name.ToLower().Contains(filter.ToLower())
                || x.Description.ToLower().Contains(filter.ToLower()));

            var totalRecords = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = items.Select(x => new ProductViewModel
            {
                Id = x.ProductId,
                Name = x.Name,
                Code = x.Code,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
                Quantity = x.Quantity

            }).ToList();


            var pagination = new Pagination<ProductViewModel>
            {
                Items = data,
                TotalRecords = totalRecords
            };

            return Ok(pagination);
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
                   && pi.IsDefault
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

        #endregion

        #region Create entity in product

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
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Code = product.Code,
                    CategoryId = product.CategoryId,
                    IsActive = product.IsActive,

                    Price = request.Price,
                    Quantity = request.InitialStock
                });
            else
                return BadRequest();
        }

        #endregion

        #region Update

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

        #endregion

        #region Price & stock

        [HttpPut("variants/{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromQuery] decimal price)
        {
            var variant = await (
                 from pv in _context.ProductVariants
                 join p in _context.Products on pv.ProductId equals p.Id
                 where pv.Id == id
                       && pv.IsActive
                       && !p.IsDeleted
                 select pv
             ).FirstOrDefaultAsync();


            if (variant == null)
                return NotFound();

            variant.Price = price;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("variants/{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromQuery] int stock)
        {
            var variant = await (
                from pv in _context.ProductVariants
                join p in _context.Products on pv.ProductId equals p.Id
                where pv.Id == id
                      && pv.IsActive
                      && !p.IsDeleted
                select pv
            ).FirstOrDefaultAsync();


            if (variant == null)
                return NotFound();

            variant.StockQuantity = stock;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        #endregion

        #region Image

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetImages(int id)
        {
            var images = await _context.ProductImages
                .Where(x => x.ProductId == id)
                .ToListAsync();

            if (images == null)
                return NotFound();

            return Ok(images);
        }

        [HttpPost("{id}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> images)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id))
                return NotFound();

            var folder = Path.Combine("wwwroot", "images", "products");
            Directory.CreateDirectory(folder);

            var sortOrder = await _context.ProductImages
                .Where(x => x.ProductId == id)
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
                    ProductId = id,
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

        [HttpPut("{id}/thumbnail/{imageId}")]
        public async Task<IActionResult> UpdateThumbInProduct(int id, int imageId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (product == null)
                return NotFound("Product can not find in the system.");

            var listImage = await _context.ProductImages
                .Where(x => x.ProductId == id && !x.IsDeleted).ToListAsync();

            if (!listImage.Any())
                return NotFound("Product does not have any images.");

            var imgOfChoose = listImage.FirstOrDefault(x => x.Id == imageId);

            if (imgOfChoose == null)
                return NotFound("Image does not belong to this product.");

            foreach (var image in listImage)
                image.IsDefault = false;

            imgOfChoose.IsDefault = true;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return Ok(result);
            else
                return BadRequest();
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

        #endregion

        #region Soft delete / trash

        [HttpGet("trash")]
        public async Task<IActionResult> GetProductsInTrash()
        {
            var products = await _context.Products
                .Where(x => x.IsDeleted).ToArrayAsync();

            return Ok(new
            {
                total = products.Count(),
                items = products
            });
        }

        [HttpDelete("{id}/soft-delete")]
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

            foreach (var productImage in productListImage)
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

            var productImages = await _context.ProductImages
                .Where(x => x.ProductId == id)
                .ToListAsync();

            foreach (var productImage in productImages)
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

        #endregion
    }
}