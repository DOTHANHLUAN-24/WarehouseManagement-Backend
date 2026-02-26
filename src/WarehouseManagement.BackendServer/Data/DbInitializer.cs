using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Data.Enums;

namespace WarehouseManagement.BackendServer.Data
{
    public class DbInitializer(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly string AdminRoleName = "Admin";
        private readonly string UserRoleName = "User";

        public async Task Seed()
        {
            string userRoleAdminId = Guid.NewGuid().ToString();
            string user1RoleUserId = Guid.NewGuid().ToString();
            string user2RoleUserId = Guid.NewGuid().ToString();

            #region Role

            if (!_roleManager.Roles.Any())
            {
                var roles = new[]
                {
                    AdminRoleName,
                    UserRoleName
                };

                foreach (var roleName in roles)
                {
                    await _roleManager.CreateAsync
                    (
                        new IdentityRole
                        {
                            Id = roleName,
                            Name = roleName,
                            NormalizedName = roleName.ToUpper()
                        }
                    );
                }
            }

            #endregion

            #region User

            if (!_userManager.Users.Any())
            {
                var users = new List<(User user, string password, string role)>
                {
                    // ===== ADMIN =====
                    (
                        new User
                        {
                            Id = userRoleAdminId,
                            UserName = "admin",
                            FirstName = "Quản trị",
                            LastName = "1",
                            Email = "admin@gmail.com",
                            LockoutEnabled = false,
                            PhoneNumber = "0123456789"
                        },
                        "Admin@123$",
                        AdminRoleName
                    ),

                    // ===== USER 1 =====
                    (
                        new User
                        {
                            Id = user1RoleUserId,
                            UserName = "user1",
                            FirstName = "Người dùng",
                            LastName = "1",
                            Email = "user1@gmail.com",
                            LockoutEnabled = false,
                            PhoneNumber = "0987654321"
                        },
                        "User@123$",
                        UserRoleName
                    ),

                    // ===== USER 2 =====
                    (
                        new User
                        {
                            Id = user2RoleUserId,
                            UserName = "user2",
                            FirstName = "Người dùng",
                            LastName = "2",
                            Email = "user2@gmail.com",
                            LockoutEnabled = false,
                            PhoneNumber = "0912345678"
                        },
                            "User@123$",
                            UserRoleName
                        )
                };

                foreach (var item in users)
                {
                    var result = await _userManager.CreateAsync(item.user, item.password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(item.user, item.role);
                    }
                }
            }

            #endregion

            #region Customer

            if (!_context.Customers.Any())
            {
                var listUser = await _userManager.Users.ToListAsync();

                foreach (var user in listUser)
                {
                    _context.Customers.Add
                    (
                        new Customer
                        {
                            UserId = user.Id,
                            FullName = $"{user.FirstName} {user.LastName}",
                            PhoneNumber = user.PhoneNumber!,
                            Status = CustomerStatus.Active,
                            CreateDate = DateTime.Now
                        }
                    );
                }
            }

            #endregion

            #region Customer address

            if (!_context.CustomerAddresses.Any())
            {
                var customers = await _context.Customers.ToListAsync();
                var random = new Random();

                var streets = new[]
                {
                    "Nguyễn Trãi", "Trần Phú", "Lê Lợi", "Phạm Văn Đồng",
                    "Cầu Giấy", "Xuân Thủy", "Hoàng Quốc Việt", "Kim Mã",
                    "Giải Phóng", "Nguyễn Văn Cừ"
                };

                var districts = new[]
                {
                    "Ba Đình", "Hoàn Kiếm", "Đống Đa", "Cầu Giấy",
                    "Thanh Xuân", "Hai Bà Trưng", "Long Biên", "Nam Từ Liêm"
                };

                var cities = new[]
                {
                    "Hà Nội",
                    "Hồ Chí Minh",
                    "Đà Nẵng"
                };

                var addresses = new List<CustomerAddress>();

                foreach (var customer in customers)
                {
                    var houseNumber = random.Next(1, 500);
                    var street = streets[random.Next(streets.Length)];
                    var district = districts[random.Next(districts.Length)];
                    var city = cities[random.Next(cities.Length)];

                    addresses.Add(new CustomerAddress
                    {
                        CustomerId = customer.Id,
                        AddressLine = $"{houseNumber} {street}, {district}",
                        City = city,
                        IsDefault = true,
                        IsDeleted = false
                    });
                }

                _context.CustomerAddresses.AddRange(addresses);
                await _context.SaveChangesAsync();
            }

            #endregion

            #region Function
            if (!_context.Functions.Any())
            {
                _context.Functions.AddRange(
                    new Function { Id = "DASHBOARD", Name = "Bảng điều khiển", ParentId = null, SortOrder = 1, Url = "/dashboard" },

                    new Function { Id = "SYSTEM", Name = "Hệ thống", ParentId = null, Url = "/systems" },
                    new Function { Id = "SYSTEM_ROLE", Name = "Nhóm quyền", ParentId = "SYSTEM", SortOrder = 1, Url = "/systems/roles" },
                    new Function { Id = "SYSTEM_USER", Name = "Người dùng", ParentId = "SYSTEM", SortOrder = 2, Url = "/systems/users" },
                    new Function { Id = "SYSTEM_FUNCTION", Name = "Chức năng", ParentId = "SYSTEM", SortOrder = 3, Url = "/systems/functions" },
                    new Function { Id = "SYSTEM_PERMISSION", Name = "Quyền hạn", ParentId = "SYSTEM", SortOrder = 4, Url = "/systems/permissions" },
                    new Function { Id = "SYSTEM_ROLE_PERMISSION", Name = "Quyền hạn theo phân quyền", ParentId = "SYSTEM", SortOrder = 5, Url = "/systems/role-permissions" },

                    new Function { Id = "CONTENT", Name = "Nội dung", ParentId = null, Url = "/contents" },
                    new Function { Id = "CONTENT_CATEGORY", Name = "Danh mục", ParentId = "CONTENT", SortOrder = 1, Url = "/contents/categories" },
                    new Function { Id = "CONTENT_COMMENT", Name = "Bình luận", ParentId = "CONTENT", SortOrder = 2, Url = "/contents/comments" },

                    new Function { Id = "STATISTIC", Name = "Thống kê", ParentId = null, Url = "/statistics" },
                    new Function { Id = "STATISTIC_MONTHLY_NEWMEMBER", Name = "Đăng ký từng tháng", ParentId = "STATISTIC", SortOrder = 1, Url = "/statistics/monthly-new-members" },
                    new Function { Id = "STATISTIC_MONTHLY_COMMENT", Name = "Bình luận theo tháng", ParentId = "STATISTIC", SortOrder = 2, Url = "/statistics/monthly-new-comments" },
                    new Function { Id = "STATISTIC_MONTHLY_HOT_PRODUCT", Name = "Sản phẩm nổi bật theo tháng", ParentId = "STATISTIC", SortOrder = 3, Url = "/statistics/monthly-hot-products" }
                    );
                await _context.SaveChangesAsync();
            }
            #endregion

            #region Permission
            if (!_context.Permissions.Any())
            {
                var adminRole = await _roleManager.FindByNameAsync(AdminRoleName);
                var functions = _context.Functions;
                foreach (var function in functions)
                {
                    _context.Permissions.Add(new Permission(function.Id, "CREATE"));
                    _context.Permissions.Add(new Permission(function.Id, "UPDATE"));
                    _context.Permissions.Add(new Permission(function.Id, "DELETE"));
                    _context.Permissions.Add(new Permission(function.Id, "VIEW"));
                }

                await _context.SaveChangesAsync();
            }
            #endregion

            #region Role Permission

            if (!_context.RolePermissions.Any())
            {
                var permissions = _context.Permissions;
                foreach (var permission in permissions)
                {
                    _context.RolePermissions.Add(new RolePermission(AdminRoleName, permission.Id));
                }

                await _context.SaveChangesAsync();
            }

            #endregion

            #region Category

            if (!_context.Categories.Any())
            {
                var now = DateTime.Now;

                // ===== ROOT =====
                var root1 = new Category
                {
                    Name = "Linh kiện phần cứng",
                    SeoAlias = "linh-kien-phan-cung",
                    SeoDescription = "Các linh kiện phần cứng",
                    CreateDate = now,
                    SortOrder = 1
                };

                var root2 = new Category
                {
                    Name = "Phụ kiện thay thế",
                    SeoAlias = "phu-kien-thay-the",
                    SeoDescription = "Các phụ kiện thay thế",
                    CreateDate = now,
                    SortOrder = 2
                };

                var root3 = new Category
                {
                    Name = "Công cụ sửa chữa",
                    SeoAlias = "cong-cu-sua-chua",
                    SeoDescription = "Dụng cụ sửa chữa",
                    CreateDate = now,
                    SortOrder = 3
                };

                _context.Categories.AddRange(root1, root2, root3);
                await _context.SaveChangesAsync();

                // ===== CHILD =====
                var children = new List<Category>
                {
                    new Category { Name = "Màn hình", SeoAlias = "man-hinh", ParentId = root1.Id, CreateDate = now },
                    new Category { Name = "Pin", SeoAlias = "pin", ParentId = root1.Id, CreateDate = now },

                    new Category { Name = "Khay SIM", SeoAlias = "khay-sim", ParentId = root2.Id, CreateDate = now },
                    new Category { Name = "Kính lưng", SeoAlias = "kinh-lung", ParentId = root2.Id, CreateDate = now },

                    new Category { Name = "Bộ tua vít", SeoAlias = "bo-tua-vit", ParentId = root3.Id, CreateDate = now },
                    new Category { Name = "Máy ép kính", SeoAlias = "may-ep-kinh", ParentId = root3.Id, CreateDate = now }
                };

                _context.Categories.AddRange(children);

                await _context.SaveChangesAsync();
            }

            #endregion

            #region Product

            if (!_context.Products.Any())
            {
                var now = DateTime.Now;

                var childCategories = _context.Categories
                    .Where(x => x.ParentId != null)
                    .ToList();

                var products = new List<Product>();

                foreach (var cat in childCategories)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        products.Add(new Product
                        {
                            Name = $"{cat.Name} mẫu {i}",
                            CategoryId = cat.Id,
                            Description = $"Sản phẩm {cat.Name} mẫu {i}",
                            Code = $"{cat.SeoAlias.ToUpper()}-{i:D3}",
                            IsActive = true,
                            IsDeleted = false,
                            CreateDate = now
                        });
                    }
                }

                _context.Products.AddRange(products);
                await _context.SaveChangesAsync();
            }

            #endregion

            #region Product variant

            if (!_context.ProductVariants.Any())
            {
                var now = DateTime.Now;

                var products = _context.Products.ToList();
                var variants = new List<ProductVariant>();

                foreach (var product in products)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        variants.Add(new ProductVariant
                        {
                            ProductId = product.Id,
                            Name = $"{product.Name} - Variant {i}",
                            SKU = $"{product.Code}-V{i}",
                            Price = 100000 + (i * 50000),
                            StockQuantity = 100 + (i * 10),
                            IsActive = true,
                            CreateDate = now,
                            Status = ProductVariantStatus.Active
                        });
                    }
                }

                _context.ProductVariants.AddRange(variants);
                await _context.SaveChangesAsync();
            }

            #endregion

            #region Product image

            if (!_context.ProductImages.Any())
            {
                var now = DateTime.UtcNow;

                var variants = _context.ProductVariants.ToList();
                var images = new List<ProductImage>();

                foreach (var variant in variants)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        images.Add(new ProductImage
                        {
                            ProductId = variant.ProductId, // giữ theo model của bạn
                            ImageUrl = $"/images/products/{variant.SKU!.ToLower()}-{i}.jpg",
                            SortOrder = i,
                            CreateDate = now,
                            IsDefault = i == 1, // ⭐ ảnh đầu là default
                            IsDeleted = false
                        });
                    }
                }

                _context.ProductImages.AddRange(images);
                await _context.SaveChangesAsync();
            }

            #endregion

            #region ProductComment

            if (!_context.ProductComments.Any())
            {
                var now = DateTime.UtcNow;
                var random = new Random();

                var variants = _context.ProductVariants.ToList();

                var sampleContents = new[]
                {
                    "Sản phẩm rất tốt",
                    "Đóng gói cẩn thận",
                    "Chất lượng ổn trong tầm giá",
                    "Giao hàng nhanh",
                    "Sẽ ủng hộ lần sau",
                    "Không như mong đợi",
                    "Hoạt động ổn định",
                    "Rất hài lòng",
                    "Tạm ổn",
                    "Đáng tiền"
                };

                // ===== PHASE 1: ROOT COMMENTS =====
                var rootComments = new List<ProductComment>();

                foreach (var variant in variants)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        rootComments.Add(
                            new ProductComment
                            {
                                ProductId = variant.ProductId,
                                ProductVariantId = variant.Id,
                                UserId = userRoleAdminId,
                                Content = sampleContents[random.Next(sampleContents.Length)],
                                Rating = random.Next(3, 6),
                                ParentId = null,
                                IsApproved = true,
                                IsDeleted = false,
                                CreateDate = now.AddMinutes(-random.Next(0, 5000))
                            }
                        );
                    }
                }

                _context.ProductComments.AddRange(rootComments);
                await _context.SaveChangesAsync(); // ⭐ cực kỳ quan trọng

                // ===== PHASE 2: REPLIES =====
                var replies = new List<ProductComment>();

                foreach (var root in rootComments)
                {
                    // mỗi variant có 3 reply → chia đều
                    if (random.Next(0, 3) == 0)
                    {
                        replies.Add
                        (
                            new ProductComment
                            {
                                ProductId = root.ProductId,
                                ProductVariantId = root.ProductVariantId,
                                UserId = random.Next(0, 2) == 0 ? user1RoleUserId : user2RoleUserId,
                                Content = "Shop phản hồi: cảm ơn bạn!",
                                Rating = random.Next(4, 6),
                                ParentId = root.Id, // ✅ giờ đã có Id thật
                                IsApproved = true,
                                IsDeleted = false,
                                CreateDate = now.AddMinutes(-random.Next(0, 5000))
                            }
                        );
                    }
                }

                _context.ProductComments.AddRange(replies);
                await _context.SaveChangesAsync();
            }

            #endregion
        }
    }
}

