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
    public class ProductsController(ApplicationDbContext _context, ILogger<ProductsController> _logger) : BaseController
    {

        #region Get - Query

        /// <summary>
        /// Get product detail by id.
        /// </summary>
        /// <param name="id">Product id.</param>
        /// <returns>Product detail with default variant and thumbnail image.</returns>
        /// <response code="200">Return product information.</response>
        /// <response code="404">Product not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Begin GetById product. ProductId: {ProductId}", id);

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
            {
                _logger.LogWarning("GetById product failed. ProductId: {ProductId}", id);

                return NotFound();
            }

            _logger.LogInformation("GetById product success. ProductId: {ProductId}", id);

            return Ok(product);
        }

        /// <summary>
        /// Get all product with filtered by category
        /// </summary>
        /// <param name="categoryId">Category id</param>
        /// <returns>List of products</returns>
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetAllProductByCategory(int categoryId)
        {
            _logger.LogInformation("Begin GetAllProductByCategory API. CategoryId = {categoryId}", categoryId);

            var listProduct = await _context.Products
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();

            foreach (var product in listProduct)
            {
                var listProductImage = await _context.ProductImages
                    .Where(x => x.ProductId == product.Id && x.IsDefault == true)
                    .ToListAsync();
            }

            _logger.LogInformation("GetAllProductByCategory success. CategoryId = {categoryId}", categoryId);

            return Ok(listProduct);
        }

        /// <summary>
        /// Get products whose prices are within a specified range.
        /// </summary>
        /// <param name="minPrice">Lower bound of product price.</param>
        /// <param name="maxPrice">Upper bound of product price.</param>
        /// <returns>
        /// A list of products including id, name, code, default image, price and stock quantity.
        /// </returns>
        /// <response code="200">Products retrieved successfully.</response>
        /// <response code="400">Invalid minPrice or maxPrice.</response>
        [HttpGet("price-between")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            _logger.LogInformation("Begin GetByPriceRange API. MinPrice = {minPrice}, MaxPrice = {maxPrice}", minPrice, maxPrice);

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

            _logger.LogInformation("GetByPriceRange success. MinPrice = {minPrice}, MaxPrice = {maxPrice}", minPrice, maxPrice);

            return Ok(products);
        }

        /// <summary>
        /// Get paged products filtered by name.
        /// </summary>
        /// <param name="filter">Search keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns>Products list filtered by keyword</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetProductsPaging(string? filter, int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Begin GetProductsPaging API. Filter = {filter}", filter);

            if (pageIndex <= 0)
            {
                _logger.LogWarning("Invalid PageIndex. Reset to 1. PageIndex={PageIndex}", pageIndex);

                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                _logger.LogWarning("Invalid PageSize. Reset to 10. PageSize={PageSize}", pageSize);

                pageSize = 10;
            }

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
            {
                _logger.LogInformation("GetProductsPaging with filter applied. Filter={Filter}", filter);

                query = query.Where(x => x.Name.Contains(filter)
                || x.Description.Contains(filter));
            }


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

            _logger.LogInformation("GetProductsPaging API success to find all categories container keyword: {filter}.", filter);

            return Ok(pagination);
        }

        /// <summary>
        /// Get all products in the system
        /// </summary>
        /// <returns>List of products</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("Begin GetProducts API");

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

            _logger.LogInformation("GetProducts API success to get all categories in system.");

            return Ok(productList);
        }

        /// <summary>
        /// Get all product variant with product id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>List of product variants</returns>
        [HttpGet("{id}/product-variant")]
        public async Task<IActionResult> GetAllProductVariantsInProduct(int id)
        {
            _logger.LogInformation("Begin GetAllProductVariants API");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Not found the product with id = {id}", id);

                return NotFound();
            }

            var productVariantsInDB = await _context.ProductVariants.Where(x => x.ProductId == id).ToListAsync();
            product.ProductVariants = productVariantsInDB;

            var result = new List<ProductVariantViewModel>();

            if (product.ProductVariants.Count != 0)
            {
                _logger.LogInformation("Get list of product variant with product id");

                foreach (var productVariant in product.ProductVariants)
                {
                    result.Add(new ProductVariantViewModel
                    {
                        ProductVariantId = productVariant.Id,
                        Name = product.Name,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        Code = product.Code,
                        IsActive = product.IsActive,
                        SKU = productVariant.SKU,
                        Price = productVariant.Price,
                        StockQuantity = productVariant.StockQuantity,
                        IsActiveInVariant = productVariant.IsActive
                    });
                }
            }

            _logger.LogInformation("Success to get product variant API. Id = {id}", id);

            return Ok(result);
        }

        /// <summary>
        /// Get detail of product with product id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Information of product</returns>
        [HttpGet("get-detail/{id}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            _logger.LogInformation("Begin GetProductDetail API");

            var product = await (
                from p in _context.Products.AsNoTracking()
                join pv in _context.ProductVariants.AsNoTracking()
                    on p.Id equals pv.ProductId
                join pc in _context.ProductComments.AsNoTracking()
                    on p.Id equals pc.ProductId
                where p.Id == id
                      && !p.IsDeleted
                      && pv.IsActive
                      && !pc.IsDeleted
                select new ProductDetailViewModel
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
                        .FirstOrDefault()!,

                    UserId = string.Empty, // Todo: Get user id
                    Content = pc.Content,
                    Rating = pc.Rating,
                    ParentId = pc.ParentId,
                    IsApproved = pc.IsApproved
                }
            ).FirstOrDefaultAsync();

            if (product == null)
            {
                _logger.LogInformation("Not found information of product");

                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Get all comment in product with product id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>List comment of product</returns>
        [HttpGet("{id}/comment")]
        public async Task<IActionResult> GetAllCommentInProduct(int id)
        {
            _logger.LogInformation("Begin GetAllCommentInProduct API");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Not found product with id = {id}", id);

                return NotFound();
            }

            var comments = await _context.ProductComments.Where(x => x.ProductId == id).ToListAsync();

            _logger.LogInformation("Success GetAllCommentInProduct API. Id = {id}", id);

            return Ok(comments);
        }

        #endregion

        #region Create entity in product

        /// <summary>
        /// Creates a new product with a default product variant.
        /// </summary>
        /// <param name="request">Product creation data sent via form-data.</param>
        /// <returns>
        /// Returns the created product if successful; otherwise returns BadRequest.
        /// </returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Product creation failed</response>
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] ProductCreateRequest request)
        {
            _logger.LogInformation("Begin PostProduct API");

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
            {
                _logger.LogInformation("PostProduct API success. Id = {id} ", product.Id);

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
            }
            else
            {
                _logger.LogInformation("PostProduct API failed to save changes");

                return BadRequest();
            }
        }

        /// <summary>
        /// Post comment into product with product id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <param name="request">Comment model</param>
        /// <returns>Result of process</returns>
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> PostCommentInProduct(int id, [FromForm] ProductCommentCreateRequest request)
        {
            _logger.LogInformation("Begin PostCommentInProduct API");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Begin PostCommentInProduct API");

                return NotFound();
            }

            var commentInProduct = new ProductComment
            {
                ProductId = id,
                ProductVariantId = request.ProductVariantId,
                UserId = string.Empty,
                Content = request.Content,
                Rating = request.Rating,
                ParentId = request.ParentId,
            };

            _context.ProductComments.Add(commentInProduct);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Success to post comment in product. Product id = {id}", id);

                return Ok(commentInProduct);
            }

            _logger.LogWarning("Fail to post comment in product. Product id = {id}", id);

            return BadRequest();
        }

        #endregion

        #region Update

        /// <summary>
        /// Change status to show or hidden product
        /// </summary>
        /// <param name="productId">Product id</param>
        /// <returns>Result of status and id of product</returns>
        [HttpPut("{productId}/status")]
        public async Task<IActionResult> ChangeStatusProduct(int productId)
        {
            _logger.LogInformation("Begin ChangeStatusProduct API");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Get Product not found. Id = {id}", productId);

                return NotFound();
            }

            product.IsActive = !product.IsActive;
            product.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("ChangeStatusProduct success. Id = {id}", product.Id);

            return Ok(
                new ProductStatusViewModel
                {
                    Id = product.Id,
                    IsActive = product.IsActive
                }
            );
        }

        /// <summary>
        /// Update comment in product with product id and comment id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <param name="commentId">Comment id</param>
        /// <param name="request">Comment model</param>
        /// <returns>Result of process</returns>
        [HttpPut("{id}/comment/{commentId}")]
        public async Task<IActionResult> UpdateCommentInProduct(int id, int commentId, [FromForm] ProductCommentUpdateRequest request)
        {
            _logger.LogInformation("Begin UpdateCommentInProduct API");

            string currentUserId = string.Empty; // Todo: Get current user id

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Not fount the product with id = {id}", id);

                return NotFound();
            }

            var comment = await _context.ProductComments
                .Where(x => x.ProductId == id && x.Id == commentId)
                .SingleOrDefaultAsync();
            if(comment == null)
            {
                _logger.LogInformation("Not found comment with id = {commentId}", commentId);

                return NotFound();
            }

            if(comment.UserId == currentUserId)
            {
                comment.Content = request.Content;

                comment.IsDeleted = request.IsDeleted;

                await _context.SaveChangesAsync();

                return Ok(comment);
            }
            else
            {
                _logger.LogWarning("This comment is not yours.");

                return BadRequest();
            }
        }

        #endregion

        #region Price & stock

        /// <summary>
        /// Update price of product
        /// </summary>
        /// <param name="id">Product id</param>
        /// <param name="price">Price want to change</param>
        /// <returns>Result of update process</returns>
        [HttpPut("variants/{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromQuery] decimal price)
        {
            _logger.LogInformation("Begin UpdatePrice API. Id = {id}", id);

            var variant = await (
                 from pv in _context.ProductVariants
                 join p in _context.Products on pv.ProductId equals p.Id
                 where pv.Id == id
                       && pv.IsActive
                       && !p.IsDeleted
                 select pv
             ).FirstOrDefaultAsync();


            if (variant == null)
            {
                _logger.LogWarning("UpdatePrice not found the product. Id = {id}", id);

                return NotFound();
            }

            variant.Price = price;
            await _context.SaveChangesAsync();

            _logger.LogInformation("UpdatePrice by id and new price success. Id = {id}", id);

            return NoContent();
        }

        /// <summary>
        /// Update stock of product
        /// </summary>
        /// <param name="id">Product id</param>
        /// <param name="stock">New stock to update</param>
        /// <returns>Result of update process</returns>
        [HttpPut("variants/{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromQuery] int stock)
        {
            _logger.LogInformation("Begin UpdateStock API. Id = {id}", id);

            var variant = await (
                from pv in _context.ProductVariants
                join p in _context.Products on pv.ProductId equals p.Id
                where pv.Id == id
                      && pv.IsActive
                      && !p.IsDeleted
                select pv
            ).FirstOrDefaultAsync();


            if (variant == null)
            {
                _logger.LogWarning("UpdateStock not found the product. Id = {id}", id);

                return NotFound();
            }

            variant.StockQuantity = stock;
            await _context.SaveChangesAsync();

            _logger.LogInformation("UpdateStock by id and new stock success. Id = {id}", id);

            return NoContent();
        }


        #endregion

        #region Image

        /// <summary>
        /// Get all image of product by product id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns></returns>
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetImages(int id)
        {
            _logger.LogInformation("Begin GetImages API");

            var images = await _context.ProductImages
                .Where(x => x.ProductId == id)
                .ToListAsync();

            if (images == null)
            {
                _logger.LogWarning("GetImages not found. Id = {id}", id);

                return NotFound();
            }

            _logger.LogInformation("GetImages by product id success. Id = {id}", id);

            return Ok(images);
        }

        /// <summary>
        /// Update images to product by product id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <param name="images">List images</param>
        /// <returns>Result of update prod</returns>
        [HttpPost("{id}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> images)
        {
            _logger.LogInformation("Begin UploadImages API");

            if (!await _context.Products.AnyAsync(x => x.Id == id))
            {
                _logger.LogWarning("UploadImages not found the product by id. Id = {id}", id);

                return NotFound();
            }

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
                    _logger.LogInformation("Set image to thumb image in the product. Product id = {id}", id);

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

            _logger.LogInformation("UploadImages success. Id = {id}", id);

            return Ok(productImages);
        }

        /// <summary>
        /// Update product thumbnail by selecting an image from the product image list.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="imageId">Image ID to be set as thumbnail.</param>
        /// <returns>
        /// Returns 200 OK if the thumbnail is updated successfully, otherwise returns an error response.
        /// </returns>
        [HttpPut("{id}/thumbnail/{imageId}")]
        public async Task<IActionResult> UpdateThumbInProduct(int id, int imageId)
        {
            _logger.LogInformation("Begin updating thumbnail for ProductId={ProductId}, ImageId={ImageId}", id, imageId);

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (product == null)
            {
                _logger.LogInformation("UpdateThumbInProduct not found product. Id = {id}", id);

                return NotFound("Product can not find in the system.");
            }

            var listImage = await _context.ProductImages
                .Where(x => x.ProductId == id && !x.IsDeleted).ToListAsync();

            if (!listImage.Any())
            {
                _logger.LogInformation("The product don't have any image");

                return NotFound("Product does not have any images.");
            }

            var imgOfChoose = listImage.FirstOrDefault(x => x.Id == imageId);

            if (imgOfChoose == null)
            {
                _logger.LogInformation("Can not found the image with id = {id}", imageId);

                return NotFound("Image does not belong to this product.");
            }

            foreach (var image in listImage)
                image.IsDefault = false;

            imgOfChoose.IsDefault = true;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("UpdateThumbInProduct API success. Id = {id}", id);

                return Ok(result);
            }
            else
            {
                _logger.LogWarning("UpdateThumbInProduct API failed. Id = {id}", id);

                return BadRequest();
            }
        }

        /// <summary>
        /// Deleted the image of product
        /// </summary>
        /// <param name="imageId">Image id</param>
        /// <returns>Result of deleted process</returns>
        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            _logger.LogInformation("Begin DeleteProductImage API");

            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                _logger.LogInformation("Can not found the image. Id = {id}", imageId);

                return NotFound();
            }

            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                _logger.LogInformation("Found the image with ImageURl");

                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    image.ImageUrl.TrimStart('/')
                );

                if (System.IO.File.Exists(filePath))
                {
                    _logger.LogInformation("Found the image in the url = {filePath}", filePath);

                    System.IO.File.Delete(filePath);
                }
            }

            image.IsDeleted = true;
            image.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("DeleteProductImage success. Image id = {imageId}", imageId);

            return Ok(new
            {
                image.Id,
                image.IsDeleted
            });
        }

        #endregion

        #region Soft delete / trash

        /// <summary>
        /// Get all products that are currently in the trash (soft-deleted).
        /// </summary>
        /// <returns>List of soft-deleted products.</returns>
        [HttpGet("trash")]
        public async Task<IActionResult> GetProductsInTrash()
        {
            _logger.LogInformation("Begin GetProductsInTrash API");

            var products = await _context.Products
                .Where(x => x.IsDeleted).ToArrayAsync();

            _logger.LogInformation("Found {Count} products in trash.", products.Length);

            return Ok(new
            {
                total = products.Count(),
                items = products
            });
        }

        /// <summary>
        /// Soft delete a product and move it to trash.
        /// This will also soft delete related images and discontinue all variants.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>The soft-deleted product.</returns>
        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteProduct(int id)
        {
            _logger.LogInformation("Begin SoftDeleteProduct API");

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogInformation("Soft delete failed. Product ID {ProductId} not found.", id);

                return NotFound();
            }

            if (product.IsDeleted)
            {
                _logger.LogWarning("Product already in the trash");

                return BadRequest("Product already in trash.");
            }

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
            {
                _logger.LogInformation("SoftDeleteProduct success. Id = {id}", id);

                return Ok(product);
            }
            else
            {
                _logger.LogWarning("SoftDeleteProduct failed to save changes");

                return BadRequest();
            }
        }

        /// <summary>
        /// Restore a soft-deleted product from trash.
        /// This will also restore related images and reactivate variants.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>The restored product.</returns>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            _logger.LogInformation("Begin RestoreProduct API");

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogInformation("Not found the product in the system");

                return NotFound();
            }

            if (!product.IsDeleted)
            {
                _logger.LogWarning("Not found the product in trash. Product id = {id}", id);

                return BadRequest();
            }

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
            {
                _logger.LogInformation("RestoreProduct success. Product id = {id}", id);

                return Ok(product);
            }
            else
            {
                _logger.LogWarning("RestoreProduct failed to save changes. Product id = {id}", id);

                return BadRequest();
            }
        }

        /// <summary>
        /// Permanently delete a product.
        /// The product must be soft-deleted before performing this action.
        /// This will remove related images, variants, and physical image files.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content if deletion succeeds.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> PermanentDeleteProduct(int id)
        {
            _logger.LogInformation("Begin PermanentDeleteProduct API");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Not found product in the system. Id = {id}", id);

                return NotFound();
            }

            if (!product.IsDeleted)
            {
                _logger.LogWarning("Product must be soft-deleted before permanent deletion.");

                return BadRequest("Product must be soft-deleted before permanent deletion.");
            }

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
                    {
                        _logger.LogInformation("Found the image in the url = {filePath}", filePath);

                        System.IO.File.Delete(filePath);
                    }
                }
            }

            var productVariants = await _context.ProductVariants
                .Where(v => v.ProductId == id)
                .ToListAsync();

            _context.ProductVariants.RemoveRange(productVariants);
            _context.ProductImages.RemoveRange(productImages);
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            _logger.LogInformation("PermanentDeleteProduct success. Id = {id}", id);

            return NoContent();
        }

        #endregion
    }
}