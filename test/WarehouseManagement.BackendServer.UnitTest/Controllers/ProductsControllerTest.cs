using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Products;

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
                });

            _context.Products.Add(new Product
            {
                Name = "Màn hình LCD IPhone 11",
                Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                Code = "LCD-IP11",
                CategoryId = 2
            });

            _context.ProductVariants.Add(
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                });

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
                });

            _context.Products.Add(new Product
            {
                Name = "Màn hình LCD IPhone 11",
                Description = "Màn hình LCD thay thế cho iPhone 11, tấm nền IPS, đã bao gồm cảm ứng.",
                Code = "LCD-IP11",
                CategoryId = 2
            });

            _context.ProductVariants.Add(
                new ProductVariant
                {
                    ProductId = 1,
                    Name = "Hàng mới",
                    SKU = "54d5644d6a",
                    Price = 76000,
                    StockQuantity = 4654,
                });

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
        public async Task GetByPriceRange_HasData_ReturnListProduct(decimal minPrice, decimal maxPrice, int countOfItem )
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
                });

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
                });

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

            _context.ProductImages.AddRange(
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
                });

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
        #endregion
    }
}
