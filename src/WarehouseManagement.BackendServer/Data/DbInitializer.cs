using Microsoft.AspNetCore.Identity;
using WarehouseManagement.BackendServer.Data.Entities;

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

        public async Task Seed()
        {
            #region Role
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(
                    new IdentityRole()
                    {
                        Id = AdminRoleName,
                        Name = AdminRoleName,
                        NormalizedName = AdminRoleName.ToUpper(),
                    });
            }
            #endregion

            #region User
            if (!_userManager.Users.Any())
            {
                var result = await _userManager.CreateAsync(
                    new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "admin",
                        FirstName = "Quản trị",
                        LastName = "1",
                        Email = "thanhluan24dev@gmail.com",
                        LockoutEnabled = false,
                    }, "Admin@123$");
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync("admin");
                    await _userManager.AddToRoleAsync(user!, AdminRoleName);
                }
            }
            #endregion

            #region Function
            if (!_context.Functions.Any())
            {
                _context.Functions.AddRange(
                    new Function { Id = "DASHBOARD", Name = "Bảng điều khiển", ParentId = null, SortOrder = 1, Url = "/dashboard" },

                    new Function { Id = "SYSTEM", Name = "Hệ thống", ParentId = null, Url = "/systems"},
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
            if(!_context.Permissions.Any())
            {
                var adminRole = await _roleManager.FindByNameAsync(AdminRoleName);
                var functions = _context.Functions;
                foreach(var function in functions)
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

            if(!_context.RolePermissions.Any())
            {
                var permissions = _context.Permissions;
                foreach (var permission in permissions)
                {
                    _context.RolePermissions.Add(new RolePermission(AdminRoleName, permission.Id));
                }

                await _context.SaveChangesAsync();
            }

            #endregion
        }
    }
}

