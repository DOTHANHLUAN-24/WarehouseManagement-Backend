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
        private readonly string AdminRoleName = "Admin";
        private readonly string UserRoleName = "User";

        /// <summary>
        /// Hàm chính để gọi các quy trình khởi tạo dữ liệu (Seeding) cho cơ sở dữ liệu.
        /// Các hàm được gọi theo thứ tự để đảm bảo tính toàn vẹn của khóa ngoại (Foreign Keys).
        /// </summary>
        public async Task Seed()
        {
            // 1. Khởi tạo dữ liệu người dùng và phân quyền (Identity)
            var adminUserId = await SeedRolesAndUsersAsync();

            // 2. Khởi tạo dữ liệu Khách hàng
            await SeedCustomersAsync();

            // 3. Khởi tạo dữ liệu Hệ thống và Phân quyền truy cập
            await SeedFunctionsAndPermissionsAsync(adminUserId);

            // 4. Khởi tạo dữ liệu Sản phẩm (Catalog)
            await SeedCatalogAsync();

            // 5. Khởi tạo dữ liệu Đánh giá sản phẩm
            await SeedProductCommentsAsync();

            // 6. Khởi tạo dữ liệu Kho hàng
            await SeedWarehousesAsync();

            // 7. Khởi tạo dữ liệu Lịch sử Giao dịch Kho (Stock Transactions)
            await SeedStockTransactionsAsync();
        }

        /// <summary>
        /// Khởi tạo danh sách các Vai trò (Role) và Người dùng (User) mặc định của hệ thống.
        /// </summary>
        /// <returns>Chuỗi ID của tài khoản Admin.</returns>
        private async Task<string> SeedRolesAndUsersAsync()
        {
            if (!roleManager.Roles.Any())
            {
                var roles = new[]
                {
                    AdminRoleName,
                    UserRoleName
                };

                foreach (var roleName in roles)
                {
                    await roleManager.CreateAsync
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

            if (!userManager.Users.Any())
            {
                var users = new List<(User user, string password, string role)>
                {
                    // ===== ADMIN =====
                    (
                        new User
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "admin",
                            FirstName = "Quản trị",
                            LastName = "1",
                            Email = "admin@gmail.com",
                            LockoutEnabled = false,
                            PhoneNumber = "0123456789",
                            IsActive = true
                        },
                        "Admin@123$",
                        AdminRoleName
                    ),

                    // ===== USER 1 =====
                    (
                        new User
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "user1",
                            FirstName = "Người dùng",
                            LastName = "1",
                            Email = "user1@gmail.com",
                            LockoutEnabled = false,
                            PhoneNumber = "0987654321",
                            IsActive = true
                        },
                        "User@123$",
                        UserRoleName
                    ),

                    // ===== USER 2 =====
                    (
                        new User
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "user2",
                            FirstName = "Người dùng",
                            LastName = "2",
                            Email = "user2@gmail.com",
                            LockoutEnabled = false,
                            PhoneNumber = "0912345678",
                            IsActive = true
                        },
                        "User@123$",
                        UserRoleName
                    )
                };

                foreach (var item in users)
                {
                    var result = await userManager.CreateAsync(item.user, item.password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(item.user, item.role);
                    }
                }
            }

            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser != null && !adminUser.IsActive)
            {
                adminUser.IsActive = true;
                await userManager.UpdateAsync(adminUser);
            }
            var user1 = await userManager.FindByNameAsync("user1");
            if (user1 != null && !user1.IsActive)
            {
                user1.IsActive = true;
                await userManager.UpdateAsync(user1);
            }
            var user2 = await userManager.FindByNameAsync("user2");
            if (user2 != null && !user2.IsActive)
            {
                user2.IsActive = true;
                await userManager.UpdateAsync(user2);
            }
            return adminUser!.Id;
        }

        /// <summary>
        /// Khởi tạo dữ liệu Khách hàng (Customer) và Địa chỉ mặc định (CustomerAddress) dựa trên danh sách User.
        /// </summary>
        private async Task SeedCustomersAsync()
        {
            // 1. Tạo Customer
            if (!context.Customers.Any())
            {
                var listUser = await userManager.Users.ToListAsync();

                foreach (var user in listUser)
                {
                    context.Customers.Add
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
                await context.SaveChangesAsync();
            }

            // 2. Tạo Customer Address
            if (!context.CustomerAddresses.Any())
            {
                var customers = await context.Customers.ToListAsync();
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

                context.CustomerAddresses.AddRange(addresses);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Khởi tạo hệ thống Chức năng (Functions), Quyền hạn (Permissions) và gán toàn bộ quyền cho Vai trò Admin.
        /// </summary>
        /// <param name="adminUserId">ID của Admin User dùng để ghi nhận Audit Log.</param>
        private async Task SeedFunctionsAndPermissionsAsync(string adminUserId)
        {
            if (!context.Functions.Any())
            {
                context.Functions.AddRange
                (
                    new Function { Id = "DASHBOARD", Name = "Bảng điều khiển", ParentId = null, SortOrder = 1, Url = "/dashboard" },

                    new Function { Id = "SYSTEM", Name = "Hệ thống", ParentId = null, Url = "/systems" },
                    new Function { Id = "SYSTEM_ROLE", Name = "Nhóm quyền", ParentId = "SYSTEM", SortOrder = 1, Url = "/systems/roles" },
                    new Function { Id = "SYSTEM_USER", Name = "Người dùng", ParentId = "SYSTEM", SortOrder = 2, Url = "/systems/users" },
                    new Function { Id = "SYSTEM_FUNCTION", Name = "Chức năng", ParentId = "SYSTEM", SortOrder = 3, Url = "/systems/functions" },
                    new Function { Id = "SYSTEM_PERMISSION", Name = "Quyền hạn", ParentId = "SYSTEM", SortOrder = 4, Url = "/systems/permissions" },
                    new Function { Id = "SYSTEM_ROLE_PERMISSION", Name = "Quyền hạn theo phân quyền", ParentId = "SYSTEM", SortOrder = 5, Url = "/systems/role-permissions" },

                    new Function { Id = "CONTENT", Name = "Nội dung", ParentId = null, Url = "/contents" },
                    new Function { Id = "CONTENT_CATEGORY", Name = "Danh mục", ParentId = "CONTENT", SortOrder = 1, Url = "/contents/categories" },

                    new Function { Id = "STATISTIC", Name = "Thống kê", ParentId = null, Url = "/statistics" },
                    new Function { Id = "STATISTIC_MONTHLY_INCOME", Name = "Thu nhập theo tháng", ParentId = "STATISTIC", SortOrder = 1, Url = "/statistics/monthly-income" },
                    new Function { Id = "STATISTIC_MONTHLY_HOT_PRODUCT", Name = "Sản phẩm nổi bật theo tháng", ParentId = "STATISTIC", SortOrder = 3, Url = "/statistics/monthly-hot-products" }
                );

                context.AuditLogs.AddRange
                (
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "DASHBOARD",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_ROLE",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_USER",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_FUNCTION",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_PERMISSION",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_ROLE_PERMISSION",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "CONTENT",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "CONTENT_CATEGORY",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "STATISTIC",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "STATISTIC_MONTHLY_INCOME",
                    },
                    new AuditLog
                    {
                        UserId = adminUserId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "STATISTIC_MONTHLY_HOT_PRODUCT",
                    }
                );

                await context.SaveChangesAsync();
            }

            if (!context.Permissions.Any())
            {
                var adminRole = await roleManager.FindByNameAsync(AdminRoleName);
                var functions = context.Functions;
                var listPermissions = new List<Permission>();
                foreach (var function in functions)
                {
                    listPermissions.Add(new Permission(function.Id, "CREATE"));
                    listPermissions.Add(new Permission(function.Id, "UPDATE"));
                    listPermissions.Add(new Permission(function.Id, "DELETE"));
                    listPermissions.Add(new Permission(function.Id, "VIEW"));
                }

                context.Permissions.AddRange(listPermissions);

                await context.SaveChangesAsync();

                foreach (var permission in listPermissions)
                {
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = adminRole!.Id.ToString(),
                        Action = permission.Action,
                        Entity = "Permission",
                        EntityId = permission.Id.ToString()
                    });
                }

                await context.SaveChangesAsync();
            }

            if (!context.RolePermissions.Any())
            {
                var permissions = context.Permissions;
                var listRolePermissions = new List<RolePermission>();
                foreach (var permission in permissions)
                {
                    listRolePermissions.Add(new RolePermission(AdminRoleName, permission.Id));
                }

                context.RolePermissions.AddRange(listRolePermissions);

                await context.SaveChangesAsync();

                foreach (var rolePermission in listRolePermissions)
                {
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = AdminRoleName,
                        Action = "CREATE",
                        Entity = "RolePermission",
                        EntityId = $"{AdminRoleName}-{rolePermission.PermissionId}"
                    });
                }

            }
        }

        /// <summary>
        /// Khởi tạo dữ liệu Danh mục sản phẩm (Categories), Sản phẩm (Products), Biến thể sản phẩm (Variants) và Hình ảnh (Images).
        /// </summary>
        private async Task SeedCatalogAsync()
        {
            // 1. Tạo Category
            if (!context.Categories.Any())
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

                context.Categories.AddRange(root1, root2, root3);
                await context.SaveChangesAsync();

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

                context.Categories.AddRange(children);

                await context.SaveChangesAsync();

                // Audit log
                var allCategories = context.Categories.ToList();
                foreach (var category in allCategories)
                {
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = AdminRoleName,
                        Action = "CREATE",
                        Entity = "Category",
                        EntityId = category.Id.ToString()
                    });
                }
                await context.SaveChangesAsync();
            }

            // 2. Tạo Product
            if (!context.Products.Any())
            {
                var now = DateTime.Now;

                var childCategories = context.Categories
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

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                // Audit log
                var allProducts = context.Products.ToList();
                foreach (var product in allProducts)
                {
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = AdminRoleName,
                        Action = "CREATE",
                        Entity = "Product",
                        EntityId = product.Id.ToString()
                    });
                }
                await context.SaveChangesAsync();
            }

            // 3. Tạo Product Variant
            if (!context.ProductVariants.Any())
            {
                var now = DateTime.Now;

                var products = context.Products.ToList();
                var variants = new List<ProductVariant>();

                foreach (var product in products)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        var random = new Random();
                        variants.Add(new ProductVariant
                        {
                            ProductId = product.Id,
                            Name = $"{product.Name} - Variant {i}",
                            SKU = $"{product.Code}-V{i}",
                            SellingPrice = random.Next(10000, 7000000) + (i * random.Next(1, 8372)),
                            OriginalPrice = random.Next(200000, 4000000) + (i * 10 * random.Next(1, 5683)),
                            StockQuantity = 100 + (i * 10),
                            IsActive = true,
                            CreateDate = now,
                            Status = ProductVariantStatus.Active
                        });
                    }
                }

                context.ProductVariants.AddRange(variants);
                await context.SaveChangesAsync();

                // Audit log
                var allVariants = context.ProductVariants.ToList();
                foreach (var variant in allVariants)
                {
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = AdminRoleName,
                        Action = "CREATE",
                        Entity = "ProductVariant",
                        EntityId = variant.Id.ToString()
                    });
                }
                await context.SaveChangesAsync();
            }

            // 4. Tạo Product Image
            if (!context.ProductImages.Any())
            {
                var now = DateTime.UtcNow;

                var variants = context.ProductVariants.ToList();
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

                context.ProductImages.AddRange(images);
                await context.SaveChangesAsync();

                // Audit log
                var allImages = context.ProductImages.ToList();
                foreach (var image in allImages)
                {
                    context.AuditLogs.Add(new AuditLog
                    {
                        UserId = AdminRoleName,
                        Action = "CREATE",
                        Entity = "ProductImage",
                        EntityId = image.Id.ToString()
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Khởi tạo dữ liệu Đánh giá/Bình luận sản phẩm ngẫu nhiên từ Khách hàng.
        /// </summary>
        private async Task SeedProductCommentsAsync()
        {
            if (!context.ProductComments.Any())
            {
                // Lấy ra các Customer và Product đã được seed ở các bước trước
                var customers = await context.Customers.ToListAsync();
                var products = await context.Products.Take(10).ToListAsync();

                if (customers.Any() && products.Any())
                {
                    var comments = new List<ProductComment>();
                    var random = new Random();
                    var now = DateTime.Now;

                    foreach (var customer in customers)
                    {
                        // Mỗi khách hàng sẽ đánh giá ngẫu nhiên 3 sản phẩm
                        var randomProducts = products.OrderBy(x => Guid.NewGuid()).Take(3).ToList();

                        foreach (var product in randomProducts)
                        {
                            comments.Add(new ProductComment
                            {
                                UserId = customer.UserId,
                                ProductId = product.Id,
                                Content = $"Sản phẩm {product.Name} dùng rất ổn, shop giao hàng cực kỳ nhanh chóng. Rất đáng tiền!",
                                Rating = random.Next(4, 6), // Ngẫu nhiên 4 hoặc 5 sao
                                CreateDate = now.AddDays(-random.Next(1, 30)) // Random ngày bình luận trong 1 tháng trở lại
                            });
                        }
                    }

                    context.ProductComments.AddRange(comments);
                    await context.SaveChangesAsync();

                    // Lưu lịch sử Audit Log
                    var allComments = context.ProductComments.ToList();
                    foreach (var comment in allComments)
                    {
                        context.AuditLogs.Add(new AuditLog
                        {
                            UserId = AdminRoleName,
                            Action = "CREATE",
                            Entity = "ProductComment",
                            EntityId = comment.Id.ToString()
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Khởi tạo thông tin danh sách Kho hàng (Warehouses) ban đầu của hệ thống.
        /// </summary>
        private async Task SeedWarehousesAsync()
        {
            if (!context.Warehouses.Any())
            {
                var now = DateTime.Now;
                context.Warehouses.AddRange
                (
                    new Warehouse
                    {
                        Location = "Số 13 ngõ 9, Phố Nguyễn Văn Huyên, Cầu Giấy, Hà Nội",
                        Capacity = 1000,
                        Email = "KhoB@gmail.com"
                    },
                    new Warehouse
                    {
                        Location = "Số 24 ngõ 8, Phố Trần Văn Ninh, Hà Nội",
                        Capacity = 1000,
                        Email = "KhoA@gmail.com"
                    }
                );

                await context.SaveChangesAsync();

                var allWarehouses = context.Warehouses.ToList();
                foreach (var warehouse in allWarehouses)
                {
                    context.AuditLogs.Add(
                        new AuditLog
                        {
                            UserId = AdminRoleName,
                            Action = "CREATE",
                            Entity = "Warehouse",
                            EntityId = warehouse.Id.ToString()
                        });
                }
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Khởi tạo dữ liệu Lịch sử giao dịch kho (StockTransactions) ban đầu.
        /// </summary>
        private async Task SeedStockTransactionsAsync()
        {
            if (!context.StockTransactions.Any())
            {
                // Lấy ra các biến thể sản phẩm và kho hàng đã được tạo
                var variants = await context.ProductVariants.Take(15).ToListAsync();
                var warehouses = await context.Warehouses.ToListAsync();

                if (variants.Any() && warehouses.Any())
                {
                    var defaultWarehouse = warehouses.First();
                    var transactions = new List<StockTransaction>();
                    var now = DateTime.Now;

                    foreach (var variant in variants)
                    {
                        // Tạo giao dịch nhập kho khởi tạo cho từng sản phẩm
                        transactions.Add(new StockTransaction
                        {
                            ProductId = variant.ProductId,
                            ProductVariantId = variant.Id,
                            WarehouseId = defaultWarehouse.Id,
                            QuantityChange = variant.StockQuantity, // Khởi tạo với đúng số lượng tồn kho hiện tại
                            TransactionType = StockTransactionType.AdjustmentIncrease,
                            Note = "Hệ thống tự động tạo: Nhập kho ban đầu theo số lượng tồn",
                            ReferenceType = (ReferenceType)1, // Ép kiểu sang giá trị Enum hiện có của ReferenceType
                            BalanceAfter = variant.StockQuantity,
                            CreateDate = now.AddDays(-1)
                        });
                    }

                    context.StockTransactions.AddRange(transactions);
                    await context.SaveChangesAsync();

                    // Lưu lịch sử Audit Log
                    var allTransactions = context.StockTransactions.ToList();
                    foreach (var tx in allTransactions)
                    {
                        context.AuditLogs.Add(new AuditLog
                        {
                            UserId = AdminRoleName,
                            Action = "CREATE",
                            Entity = "StockTransaction",
                            EntityId = tx.Id.ToString()
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
