using FluentValidation;
using FluentValidation.AspNetCore;
using WarehouseManagement.BackendServer.Mapping;
using WarehouseManagement.BackendServer.Repositories.Implements;
using WarehouseManagement.BackendServer.Repositories.Implements.Authentication;
using WarehouseManagement.BackendServer.Repositories.Interfaces;
using WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication;
using WarehouseManagement.BackendServer.Services.Implementations;
using WarehouseManagement.BackendServer.Services.Implementations.Authentication;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.BackendServer.Services.Interfaces.Authentication;
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingConfig));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuditLogService, AuditLogService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<UserCreateRequestValidator>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ITokenManagement, TokenManagement>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register IUserService and its implementation
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}