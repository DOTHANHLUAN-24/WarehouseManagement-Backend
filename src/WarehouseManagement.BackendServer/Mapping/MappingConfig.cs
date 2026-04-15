using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Categories;
using WarehouseManagement.ViewModels.Contents.Products;
using WarehouseManagement.ViewModels.Contents.StockTransactions;
using WarehouseManagement.ViewModels.Systems.AuditLogs;
using WarehouseManagement.ViewModels.Systems.Functions;
using WarehouseManagement.ViewModels.Systems.Login;
using WarehouseManagement.ViewModels.Systems.Permissions;
using WarehouseManagement.ViewModels.Systems.Roles;
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // User
            CreateMap<UserCreateRequest, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UserViewModel>();
            CreateMap<LoginRequestModel, User>();

            // Role
            CreateMap<RoleCreateRequest, IdentityRole>();
            CreateMap<IdentityRole, RoleViewModel>();

            // Category
            CreateMap<CategoryCreateRequest, Category>();
            CreateMap<Category, CategoryViewModel>();

            // Product
            CreateMap<ProductCreateRequest, Product>();
            CreateMap<ProductVariant, ProductVariantViewModel>();

            // Function
            CreateMap<FunctionCreateRequest, Function>();
            CreateMap<Function, FunctionViewModel>();

            // Permission
            CreateMap<Permission, PermissionScreenViewModel>();
            CreateMap<RolePermission, PermissionInRoleViewModel>();


            // StockTransaction
            CreateMap<StockTransactionCreateRequest, StockTransaction>();
            CreateMap<StockTransaction, StockTransactionViewModel>();

            // AuditLog
            CreateMap<AuditLogCreateRequest, AuditLog>();
            CreateMap<AuditLogViewModel, AuditLog>();
        }
    }
}
