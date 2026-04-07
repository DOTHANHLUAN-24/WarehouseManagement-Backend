using WarehouseManagement.BackendServer.Mapping;
using WarehouseManagement.BackendServer.Services.Implementations.Authentication;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.BackendServer.Services.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingConfig));
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IRoleService, RoleService>();
            return services;
        }
    }
}
