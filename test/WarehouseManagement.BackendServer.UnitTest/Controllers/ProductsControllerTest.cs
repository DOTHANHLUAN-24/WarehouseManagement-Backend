using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Products;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.UnitTest.Controllers
{
    public class ProductsControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;

        public ProductsControllerTest()
        {
            _context = new InMemoryContextFactory().Create();
            _mockLogger = new Mock<ILogger<ProductsController>>();
        }

        // =========================
        // Constructor
        // =========================

        [Fact]
        public void ShouldCreateInstance_NotNull_ReturnSuccess()
        {
            var controller = new ProductsController(_context, _mockLogger.Object);

            Assert.NotNull(controller);
        }

        #region Get -Query

        // =========================
        // Get product by product id
        // =========================

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            // Arrange
            _context.Categories.AddRange(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình LCD",
                    SortOrder = 1
                },
                new Category
                {
                    Name = "Màn hình LCD",
                    ParentId = 1,
                    SeoAlias = "man-hinh-lcd",
                    SeoDescription = "Màn hình LCD (Liquid Crystal Display) là màn hình hiển thị sử dụng tinh thể lỏng để tạo hình ảnh, hoạt động nhờ đèn nền (backlight) chiếu sáng từ phía sau.",
                    SortOrder = 2
                }
            );

            _context.Products.Add(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.Add(
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                }
            );

            _context.ProductImages.Add(new ProductImage
            {
                ProductId = 1,
                ImageUrl = "test",
                IsDefault = true,
                SortOrder = 1
            });

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var productViewModel = okResult.Value as ProductViewModel;

            // Assert
            Assert.Equal("Màn hình LCD IPhone 11", productViewModel!.Name);
        }

        [Fact]
        public async Task GetById_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Get list product by category id
        // =========================

        [Fact]
        public async Task GetAllProductByCategory_HasData_ReturnListProduct()
        {
            // Arrange
            _context.Categories.AddRange(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình LCD",
                    SortOrder = 1
                },
                new Category
                {
                    Name = "Màn hình LCD",
                    ParentId = 1,
                    SeoAlias = "man-hinh-lcd",
                    SeoDescription = "Màn hình LCD (Liquid Crystal Display) là màn hình hiển thị sử dụng tinh thể lỏng để tạo hình ảnh, hoạt động nhờ đèn nền (backlight) chiếu sáng từ phía sau.",
                    SortOrder = 2
                }
            );

            _context.Products.Add(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.Add(
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                }
            );

            _context.ProductImages.Add(
                new ProductImage
                {
                    ProductId = 1,
                    ImageUrl = "test",
                    IsDefault = true,
                    SortOrder = 1
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductByCategory(2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            // Assert
            Assert.Single(listProduct!);
        }

        [Fact]
        public async Task GetAllProductByCategory_HasNoData_ReturnEmptyList()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductByCategory(2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            // Assert
            Assert.Empty(listProduct!);
        }

        // =========================
        // Get list product by price range
        // =========================

        [Theory]
        [InlineData(10000, 55000, 1)]
        [InlineData(55000, 80000, 1)]
        [InlineData(10000, 80000, 2)]
        public async Task GetByPriceRange_HasData_ReturnListProduct(decimal minPrice, decimal maxPrice, int countOfItem)
        {
            // Arrange
            _context.Categories.AddRange(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình LCD",
                    SortOrder = 1
                },
                new Category
                {
                    Name = "Màn hình LCD",
                    ParentId = 1,
                    SeoAlias = "man-hinh-lcd",
                    SeoDescription = "Màn hình LCD (Liquid Crystal Display) là màn hình hiển thị sử dụng tinh thể lỏng để tạo hình ảnh, hoạt động nhờ đèn nền (backlight) chiếu sáng từ phía sau.",
                    SortOrder = 2
                }
            );

            _context.Products.AddRange(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung Galaxy A14",
                    Description = "Màn hình LCD dành cho Samsung Galaxy A14, công nghệ TFT, chất lượng tiêu chuẩn.",
                    Code = "LCD-SSA14",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.AddRange
            (
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                },
                new ProductVariant
                {
                    ProductId = 2,
                    Name = "Hàng mới",
                    SKU = "5a4a56d454",
                    Price = 50000,
                    StockQuantity = 1133,
                }
            );

            _context.ProductImages.AddRange
            (
                new ProductImage
                {
                    ProductId = 1,
                    ImageUrl = "test 1",
                    IsDefault = true,
                    SortOrder = 1
                },
                new ProductImage
                {
                    ProductId = 2,
                    ImageUrl = "test 2",
                    IsDefault = true,
                    SortOrder = 1
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetByPriceRange(minPrice, maxPrice);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            // Assert
            Assert.Equal(countOfItem, listProduct!.Count());

        }


        [Fact]
        public async Task GetByPriceRange_HasNoData_ReturnEmptyList()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetByPriceRange(1541, 54112);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            // Assert
            Assert.Empty(listProduct!);
        }

        // =========================
        // Get products paging
        // =========================

        [Theory]
        [InlineData(null, 1, 10, 2)]
        [InlineData("test", 1, 10, 0)]
        [InlineData("Màn hình", 1, 10, 2)]
        public async Task GetProductsPaging_HasData_ReturnListProduct
            (
                string? filter,
                int pageIndex,
                int pageSize,
                int totalItems
            )
        {
            // Arrange
            _context.Products.AddRange
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung S25 Ultra",
                    Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                    Code = "LCD-S25U",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.AddRange
            (
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                },
                new ProductVariant
                {
                    ProductId = 2,
                    Name = "Hàng mới",
                    SKU = "5a4a56d454",
                    Price = 50000,
                    StockQuantity = 1133,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act

            var result = await controller.GetProductsPaging(filter, pageIndex, pageSize);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = okResult.Value as Pagination<ProductViewModel>;

            // Assert

            Assert.NotNull(pagination);
            Assert.Equal(totalItems, pagination!.TotalRecords);
        }

        [Fact]
        public async Task GetProductsPaging_HasNoData_ReturnListProduct()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetProductsPaging("jdakl", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = okResult.Value as Pagination<ProductViewModel>;

            // Assert

            Assert.Empty(pagination!.Items);
        }

        // =========================
        // Get Products
        // =========================

        [Fact]
        public async Task GetProducts_HasData_ReturnListProduct()
        {
            // Arrange
            _context.Products.AddRange
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung S25 Ultra",
                    Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                    Code = "LCD-S25U",
                    CategoryId = 2
                }
            );

            _context.ProductImages.AddRange
            (
                new ProductImage
                {
                    ProductId = 1,
                    ImageUrl = "test",
                    IsDefault = true,
                    SortOrder = 1
                },
                new ProductImage
                {
                    ProductId = 2,
                    ImageUrl = "test",
                    IsDefault = true,
                    SortOrder = 1
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetProducts();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(okResult.Value);

            // Assert
            Assert.Equal(2, listProduct!.Count());
        }


        [Fact]
        public async Task GetProducts_HasNoData_ReturnListProduct()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetProducts();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(okResult.Value);

            // Assert
            Assert.Empty(listProduct);
        }

        // =========================
        // Get all product variants in product
        // =========================

        [Fact]
        public async Task GetAllProductVariantsInProduct_HasData_ReturnListProduct()
        {
            // Arrange
            _context.Products.AddRange
           (
               new Product
               {
                   Name = "Màn hình LCD IPhone 11",
                   Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                   Code = "LCD-IP11",
                   CategoryId = 2
               },
               new Product
               {
                   Name = "Màn hình LCD Samsung S25 Ultra",
                   Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                   Code = "LCD-S25U",
                   CategoryId = 2
               }
           );

            _context.ProductImages.AddRange
            (
                new ProductImage
                {
                    ProductId = 1,
                    ImageUrl = "test",
                    IsDefault = true,
                    SortOrder = 1
                },
                new ProductImage
                {
                    ProductId = 2,
                    ImageUrl = "test",
                    IsDefault = true,
                    SortOrder = 1
                }
            );

            _context.ProductVariants.AddRange
            (
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                },
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới 2",
                    SKU = "5a4a56d454",
                    Price = 50000,
                    StockQuantity = 1133,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductVariantsInProduct(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<ProductVariantViewModel>>(okResult.Value);

            // Assert
            Assert.Equal(2, listProduct.Count());
        }

        [Fact]
        public async Task GetAllProductVariantsInProduct_HasNoDataInProductVariant_ReturnEmptyListProduct()
        {
            // Arrange
            _context.Products.AddRange
           (
               new Product
               {
                   Name = "Màn hình LCD IPhone 11",
                   Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                   Code = "LCD-IP11",
                   CategoryId = 2
               },
               new Product
               {
                   Name = "Màn hình LCD Samsung S25 Ultra",
                   Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                   Code = "LCD-S25U",
                   CategoryId = 2
               }
           );

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductVariantsInProduct(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listProduct = Assert.IsAssignableFrom<IEnumerable<ProductVariantViewModel>>(okResult.Value);

            // Assert
            Assert.Empty(listProduct);
        }

        [Fact]
        public async Task GetAllProductVariantsInProduct_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductVariantsInProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Get product detail
        // =========================

        [Fact]
        public async Task GetProductDetail_HasData_ReturnProductDetail()
        {
            // Arrange
            _context.Products.AddRange
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung S25 Ultra",
                    Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                    Code = "LCD-S25U",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.AddRange
            (
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                },
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới 2",
                    SKU = "5a4a56d454",
                    Price = 50000,
                    StockQuantity = 1133,
                }
            );

            _context.ProductComments.AddRange(
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = string.Empty,
                    Content = "Sản phẩm tốt, chất lượng ổn định.",
                    Rating = 5,
                    ParentId = null,
                },
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = string.Empty,
                    Content = "Sản phẩm hiện tại vẫn còn nhiều thiếu sót",
                    Rating = 5,
                    ParentId = null,
                }
            );

            _context.ProductImages.AddRange
            (
                new ProductImage
                {
                    ProductId = 1,
                    ImageUrl = "test",
                    IsDefault = true,
                    SortOrder = 1
                },
                new ProductImage
                {
                    ProductId = 1,
                    ImageUrl = "test 2",
                    IsDefault = false,
                    SortOrder = 2
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetProductDetail(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var productDetail = okResult.Value as ProductDetailViewModel;

            // Assert
            Assert.NotNull(productDetail);
        }

        [Fact]
        public async Task GetProductDetail_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetProductDetail(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Get all comment in product
        // =========================

        [Fact]
        public async Task GetAllCommentInProduct_HasData_ReturnListComment()
        {
            // Arrange
            _context.Products.AddRange
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung S25 Ultra",
                    Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                    Code = "LCD-S25U",
                    CategoryId = 2
                }
            );

            _context.ProductComments.AddRange(
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = string.Empty,
                    Content = "Sản phẩm tốt, chất lượng ổn định.",
                    Rating = 5,
                    ParentId = null,
                },
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = string.Empty,
                    Content = "Sản phẩm hiện tại vẫn còn nhiều thiếu sót",
                    Rating = 5,
                    ParentId = null,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllCommentInProduct(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listComment = Assert.IsAssignableFrom<IEnumerable<ProductComment>>(okResult.Value);

            // Assert
            Assert.Equal(2, listComment!.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetAllCommentInProduct_HasNoDataInProductComment_ReturnEmptyListComment(int productId)
        {
            // Arrange
            _context.Products.AddRange
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung S25 Ultra",
                    Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                    Code = "LCD-S25U",
                    CategoryId = 2
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllCommentInProduct(productId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listComment = Assert.IsAssignableFrom<IEnumerable<ProductComment>>(okResult.Value);

            // Assert
            Assert.Empty(listComment);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetAllCommentInProduct_HasNoDataInProduct_ReturnNotFound(int productId)
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllCommentInProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Create entity in product

        // =========================
        // Post product
        // =========================

        [Fact]
        public async Task PostProduct_ValidInput_ReturnSuccess()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình LCD",
                    SortOrder = 1
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PostProduct(
                new ProductCreateRequest
                {
                    Name = "product 1",
                    Description = "description of product 1",
                    CategoryId = 1,
                    Code = "code of product 1",
                    Price = 10000,
                    InitialStock = 45646,
                    SKU = "sku of product 1"
                }
            );

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        // =========================
        // Post comment in product
        // =========================

        [Fact]
        public async Task PostCommentInProduct_HasDataAndValidInput_ReturnSuccess()
        {
            // Arrange
            _context.Products.AddRange
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                },
                new Product
                {
                    Name = "Màn hình LCD Samsung S25 Ultra",
                    Description = "Màn hình LCD thay thế cho Samsung S25 Ultra.",
                    Code = "LCD-S25U",
                    CategoryId = 2
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PostCommentInProduct(1,
                new ProductCommentCreateRequest
                {
                    UserId = string.Empty,
                    Content = "Sản phẩm tốt, chất lượng ổn định.",
                    Rating = 5,
                    ParentId = null,
                }
            );

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PostCommentInProduct_HasNoDataAndValidInput_ReturnSuccess()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PostCommentInProduct(1,
                new ProductCommentCreateRequest
                {
                    UserId = string.Empty,
                    Content = "Sản phẩm tốt, chất lượng ổn định.",
                    Rating = 5,
                    ParentId = null,
                }
            );

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Update

        // =========================
        // Update status product
        // =========================

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task UpdateStatusProduct_HasData_ReturnObject(bool beforeChange, bool afterChange)
        {
            // Arrange
            _context.Products.Add(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2,
                    IsActive = beforeChange
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.ChangeStatusProduct(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<ProductStatusViewModel>(okResult.Value);

            // Assert
            Assert.Equal(afterChange, data.IsActive);
        }

        [Fact]
        public async Task UpdateStatusProduct_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.ChangeStatusProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Update comment in product
        // =========================

        [Fact]
        public async Task UpdateCommentInProduct_HasData_ReturnSuccess()
        {
            // Arrange
            _context.Products.Add(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            _context.ProductComments.AddRange(
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = string.Empty,
                    Content = "Sản phẩm tốt, chất lượng ổn định.",
                    Rating = 5,
                    ParentId = null,
                },
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = string.Empty,
                    Content = "Sản phẩm hiện tại vẫn còn nhiều thiếu sót",
                    Rating = 5,
                    ParentId = null,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdateCommentInProduct(1, 1, new ProductCommentUpdateRequest
            {
                Content = "Comment mới",
            });
            var okResult = Assert.IsType<OkObjectResult>(result);
            var comment = Assert.IsType<ProductComment>(okResult.Value);

            // Assert
            Assert.Equal("Comment mới", comment.Content);
        }

        [Fact]
        public async Task UpdateCommentInProduct_CommentIsNotYour_ReturnBadRequest()
        {
            // Arrange
            _context.Products.Add(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            _context.ProductComments.AddRange(
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = "ajkldkl",
                    Content = "Sản phẩm tốt, chất lượng ổn định.",
                    Rating = 5,
                    ParentId = null,
                },
                new ProductComment
                {
                    ProductId = 1,
                    ProductVariantId = 1,
                    UserId = "akljdklalda",
                    Content = "Sản phẩm hiện tại vẫn còn nhiều thiếu sót",
                    Rating = 5,
                    ParentId = null,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdateCommentInProduct(1, 1, new ProductCommentUpdateRequest
            {
                Content = "Comment mới",
            });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCommentInProduct_HasNotDataInProduct_ReturnBadRequest()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdateCommentInProduct(1, 1, new ProductCommentUpdateRequest
            {
                Content = "Comment mới",
            });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCommentInProduct_HasNotDataInProductComment_ReturnNotFound()
        {
            // Arrange
            _context.Products.Add(
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdateCommentInProduct(1, 1, new ProductCommentUpdateRequest
            {
                Content = "Comment mới",
            });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Price & stock

        // =========================
        // Update price
        // =========================

        [Theory]
        [InlineData(46521, 1454)]
        [InlineData(4651, 145654)]
        public async Task UpdatePrice_HasDataAndValidInput_ReturnSuccess(decimal beforePrice, decimal afterPrice)
        {
            // Arrange
            _context.Products.Add
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.Add
            (
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = beforePrice,
                    StockQuantity = 4654,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdatePrice(1, afterPrice);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<ProductVariantViewModel>(okResult.Value);

            // Assert
            Assert.Equal(afterPrice, product.Price);
        }

        [Fact]
        public async Task UpdatePrice_HasNoDataInProduct_ReturnNotFound()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdatePrice(1, 564123);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Update price
        // =========================

        [Theory]
        [InlineData(46521, 1454)]
        [InlineData(4651, 145654)]
        public async Task UpdateStock_HasData_ReturnSuccess(int beforeStock, int afterStock)
        {
            // Arrange
            _context.Products.Add
            (
                new Product
                {
                    Name = "Màn hình LCD IPhone 11",
                    Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                    Code = "LCD-IP11",
                    CategoryId = 2
                }
            );

            _context.ProductVariants.Add
            (
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 455123,
                    StockQuantity = beforeStock,
                }
            );

            await _context.SaveChangesAsync();

            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdateStock(1, afterStock);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<ProductVariantViewModel>(okResult.Value);

            // Assert
            Assert.Equal(afterStock, product.StockQuantity);
        }

        [Fact]
        public async Task UpdateStock_HasNoDataInProduct_ReturnNotFound()
        {
            // Arrange
            var controller = new ProductsController(_context, _mockLogger.Object);

            // Act
            var result = await controller.UpdateStock(1, 564123);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion
    }
}