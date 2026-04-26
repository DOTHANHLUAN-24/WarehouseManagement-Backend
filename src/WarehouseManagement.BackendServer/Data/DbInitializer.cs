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

        public async Task Seed()
        {
            string userRoleAdminId = Guid.NewGuid().ToString();
            string user1RoleUserId = Guid.NewGuid().ToString();
            string user2RoleUserId = Guid.NewGuid().ToString();

            #region Role

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

            #endregion

            #region User

            if (!userManager.Users.Any())
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
                    var result = await userManager.CreateAsync(item.user, item.password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(item.user, item.role);
                    }
                }
            }

            #endregion

            #region Customer

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
            }

            #endregion

            #region Customer address

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

            #endregion


            #region Function
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
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "DASHBOARD",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_ROLE",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_USER",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_FUNCTION",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_PERMISSION",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "SYSTEM_ROLE_PERMISSION",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "CONTENT",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "CONTENT_CATEGORY",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "STATISTIC",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "STATISTIC_MONTHLY_INCOME",
                    },
                    new AuditLog
                    {
                        UserId = userRoleAdminId,
                        Action = "CREATE",
                        Entity = "Function",
                        EntityId = "STATISTIC_MONTHLY_HOT_PRODUCT",
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Permission
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
            #endregion

            #region Role Permission

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

            #endregion

            #region Category

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
            }

            #endregion

            #region Product

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
            }

            #endregion

            #region Product variant

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
            }

            #endregion

            #region Product image

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
            }

            #endregion

            #region Warehouse

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
            }
        }

        #endregion
    }
}
