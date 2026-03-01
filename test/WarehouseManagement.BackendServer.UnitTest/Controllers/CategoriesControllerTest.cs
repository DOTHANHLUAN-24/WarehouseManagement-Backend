using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Categories;
using WarehouseManagement.ViewModels.Contents.Products;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.UnitTest.Controllers
{
    public class CategoriesControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<CategoriesController>> _mockLogger;

        public CategoriesControllerTest()
        {
            _context = new InMemoryContextFactory().Create();
            _mockLogger = new Mock<ILogger<CategoriesController>>();
        }

        // =========================
        // Constructor
        // =========================

        [Fact]
        public void ShouldCreateInstance_NotNull_ReturnSuccess()
        {
            var controller = new CategoriesController(_context, _mockLogger.Object);

            Assert.NotNull(controller);
        }

        // =========================
        // Post category
        // =========================

        [Fact]
        public async Task PostCategory_ValidInput_Success()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PostCategory(new CategoryCreateRequest
            {
                Name = "Màn hình",
                ParentId = null,
                SeoAlias = "man-hinh",
                SeoDescription = "Màn hình LCD",
                SortOrder = 1
            });

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        // =========================
        // Get by category id
        // =========================

        [Theory]
        [InlineData("Màn hình", null, "man-hinh", "Màn hình LCD", 1, 1, "Màn hình")]
        [InlineData("Pin", null, "pin", "Pin lithium-ion", 1, 1, "Pin")]
        public async Task GetById_HasData_Success(
            string name,
            int? parentId,
            string seoAlias,
            string seoDescription,
            int sortOrder,
            int resultCategoryId,
            string resultCategoryName)
        {
            // Arrange
            _context.Categories.AddRange
                (
                    new Category
                    {
                        Name = name,
                        ParentId = parentId,
                        SeoAlias = seoAlias,
                        SeoDescription = seoDescription,
                        SortOrder = sortOrder
                    }
                );
            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act 
            var result = await controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categoryViewModel = okResult.Value as CategoryViewModel;

            // Assert
            Assert.Equal(resultCategoryId, categoryViewModel!.Id);
            Assert.Equal(resultCategoryName, categoryViewModel!.Name);
        }

        [Fact]
        public async Task GetById_HasNoData_Success()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Get all categories
        // =========================

        [Fact]
        public async Task GetCategories_HasData_Success()
        {
            // Arrange
            _context.Categories.AddRange
               (
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
                       Name = "Pin",
                       ParentId = null,
                       SeoAlias = "pin",
                       SeoDescription = "Màn hình LCD",
                       SortOrder = 1
                   }
               );
            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllCategories();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(okResult.Value);

            // Assert
            Assert.Equal(2, categories.Count());
        }

        [Fact]
        public async Task GetCategories_HasNoData_Success()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllCategories();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(okResult.Value);

            // Assert
            Assert.Empty(categories);
        }

        // =========================
        // Get category paging
        // =========================

        [Theory]
        [InlineData(null, 1, 10, 5)]
        [InlineData("Màn", 1, 5, 3)]
        [InlineData("Pin", 1, 10, 2)]
        [InlineData("data", 1, 10, 0)]
        public async Task GetCategoriesPaging_HasData_ReturnListCategory(
                string? filter,
                int pageIndex,
                int pageSize,
                int countItem
            )
        {
            // Arrange
            _context.AddRange(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1
                },
                new Category
                {
                    Name = "Màn hình LCD",
                    ParentId = 1,
                    SeoAlias = "man-hinh-lcd",
                    SeoDescription = "Màn hình LCD (Liquid Crystal Display) là màn hình hiển thị sử dụng tinh thể lỏng để tạo hình ảnh, hoạt động nhờ đèn nền (backlight) chiếu sáng từ phía sau.",
                    SortOrder = 2
                },
                new Category
                {
                    Name = "Màn hình OLED",
                    ParentId = 1,
                    SeoAlias = "man-hinh-oled",
                    SeoDescription = "Màn hình OLED là màn hình tự phát sáng.",
                    SortOrder = 2
                },
                new Category
                {
                    Name = "Pin",
                    ParentId = null,
                    SeoAlias = "pin",
                    SeoDescription = "Pin để chứa đựng lượng điện giúp cho máy điện thoại có thể hoạt động bình thường",
                    SortOrder = 1
                },
                new Category
                {
                    Name = "Pin lithium-ion",
                    ParentId = 4,
                    SeoAlias = "pin-lithium-ion",
                    SeoDescription = "Pin lithium-ion (Li-ion) là loại pin sạc sử dụng ion lithium để lưu trữ và giải phóng năng lượng điện.",
                    SortOrder = 1
                }
            );
            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetCategoriesPaging(filter, pageIndex, pageSize);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = okResult.Value as Pagination<CategoryViewModel>;

            // Assert
            Assert.Equal(countItem, pagination!.TotalRecords);
        }

        [Fact]
        public async Task GetCategoriesPaging_HasNoData_ReturnListEmpty()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetCategoriesPaging("test", 1, 10);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagination = okResult.Value as Pagination<CategoryViewModel>;

            // Assert
            Assert.Empty(pagination!.Items);
        }

        // =========================
        // Get all products by category id
        // =========================

        [Theory]
        [InlineData(2, 2)]
        [InlineData(1, 2)]
        [InlineData(3, 0)]
        public async Task GetAllProductByCategoryId_HasData_ReturnListProduct(int categoryId, int countOfProduct)
        {
            // Arrange
            _context.AddRange(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1
                },
                new Category
                {
                    Name = "Màn hình LCD",
                    ParentId = 1,
                    SeoAlias = "man-hinh-lcd",
                    SeoDescription = "Màn hình LCD (Liquid Crystal Display) là màn hình hiển thị sử dụng tinh thể lỏng để tạo hình ảnh, hoạt động nhờ đèn nền (backlight) chiếu sáng từ phía sau.",
                    SortOrder = 2
                },
                new Category
                {
                    Name = "Màn hình OLED",
                    ParentId = 1,
                    SeoAlias = "man-hinh-oled",
                    SeoDescription = "Màn hình OLED là màn hình tự phát sáng.",
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

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductByCategoryId(categoryId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(okResult.Value);

            // Assert
            Assert.Equal(countOfProduct, categories.Count());
        }

        [Fact]
        public async Task GetAllProductByCategoryId_HasNoDataInProduct_ReturnEmptyList()
        {
            // Arrange
            _context.Categories.Add(new Category
            {
                Name = "Màn hình",
                ParentId = null,
                SeoAlias = "man-hinh",
                SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                SortOrder = 1
            });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductByCategoryId(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var categories = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(okResult.Value);

            // Assert
            Assert.Empty(categories);
        }

        [Fact]
        public async Task GetAllProductByCategoryId_HasNoDataInCategory_ReturnEmptyList()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.GetAllProductByCategoryId(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Put category with category id
        // =========================

        [Fact]
        public async Task PutCategory_ValidInput_ReturnSuccess()
        {
            // Arrange
            _context.Categories.Add(new Category
            {
                Name = "Màn hình",
                ParentId = null,
                SeoAlias = "man-hinh",
                SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                SortOrder = 1
            });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PutCategory(1, new CategoryUpdateRequest
            {
                Name = "Category name 1",
                SeoAlias = "category-name-1",
                SeoDescription = "Test category seo desc",
                SortOrder = 1,
                ParentId = null,
            });

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutCategory_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PutCategory(1, new CategoryUpdateRequest
            {
                Name = "Category name 1",
                SeoAlias = "category-name-1",
                SeoDescription = "Test category seo desc",
                SortOrder = 1,
                ParentId = null,
            });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutCategory_ConflictParentId_ReturnBadRequest()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = 1,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1
                });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PutCategory(1, new CategoryUpdateRequest
            {
                Name = "Category name 1",
                SeoAlias = "category-name-1",
                SeoDescription = "Test category seo desc",
                SortOrder = 1,
                ParentId = 1,
            });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        // =========================
        // Permanent delete category by category id
        // =========================

        [Fact]
        public async Task PermanentDeleteCategory_HasDataInTrash_ReturnSuccess()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1,
                    IsDeleted = true
                });
            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PermanentDeleteCategory(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PermanentDeleteCategory_HasDataNotInTrash_ReturnBadRequest()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1
                });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PermanentDeleteCategory(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PermanentDeleteCategory_HasNoData_ReturnBadRequest()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.PermanentDeleteCategory(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Soft delete category by category id
        // =========================

        [Fact]
        public async Task SoftDeleteCategory_HasNonDeletedCategory_ReturnOk()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1
                });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.SoftDeleteCategory(1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task SoftDeleteCategory_HasDeletedCategory_ReturnOk()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1,
                    IsDeleted = true
                });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.SoftDeleteCategory(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SoftDeleteCategory_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.SoftDeleteCategory(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Restore Category by category id
        // =========================

        [Fact]
        public async Task RestoreCategory_HasNonDeletedCategory_ReturnOk()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1
                });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.RestoreCategory(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RestoreCategory_HasDeletedCategory_ReturnOk()
        {
            // Arrange
            _context.Categories.Add(
                new Category
                {
                    Name = "Màn hình",
                    ParentId = null,
                    SeoAlias = "man-hinh",
                    SeoDescription = "Màn hình được sử dụng để có thể giúp người dùng quan sát",
                    SortOrder = 1,
                    IsDeleted = true
                });

            await _context.SaveChangesAsync();

            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.RestoreCategory(1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RestoreCategory_HasNoData_ReturnNotFound()
        {
            // Arrange
            var controller = new CategoriesController(_context, _mockLogger.Object);

            // Act
            var result = await controller.RestoreCategory(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
