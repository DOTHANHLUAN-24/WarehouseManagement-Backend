using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.DependencyInjection;
using WarehouseManagement.BackendServer.Swagger;
using WarehouseManagement.BackendServer.Helpers;
using WarehouseManagement.ViewModels.Systems.Roles;

internal class Program
{
    private static async Task Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = WebApplication.CreateBuilder(args);

        var frontendUrl = builder.Configuration["FrontendUrl"] ?? "https://warehouse-management-front-end.vercel.app";

        //
        // =======================
        // CORS (PHẢI TRƯỚC BUILD)
        // =======================
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins(frontendUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        // SERILOG
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
        Log.Information("Application is building...");

        builder.Services.AddControllers();

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<RoleCreateRequestValidator>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Warehouse Management API", Version = "v1" });

            options.CustomSchemaIds(type => type.FullName);

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Enter JWT token",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        { jwtSecurityScheme, Array.Empty<string>() }
            });
            // Add example operation filter for purchase request bodies
            options.OperationFilter<PurchaseRequestExampleOperationFilter>();
        });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        builder.Services
            .AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<DbInitializer>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                ValidAudience = builder.Configuration["JwtConfig:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)
                ),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                NameClaimType = JwtRegisteredClaimNames.Name,
                RoleClaimType = ClaimTypes.Role,

                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddApplicationServices();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                // Apply any pending EF Core migrations before seeding
                Log.Information("Applying database migrations (if any)...");
                var db = services.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                Log.Information("Seeding data...");
                var dbInitializer = services.GetRequiredService<DbInitializer>();
                await dbInitializer.Seed();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Seed error");
                throw;
            }
        }

        i// Xóa bỏ hoặc comment block IF này lại
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Cấu hình này giúp khi vào thẳng link gốc .onrender.com là giao diện Swagger hiện ra luôn, không cần gõ thêm /swagger
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse Management API v1");
            options.RoutePrefix = string.Empty;
        });
        // }

        app.UseMiddleware<ErrorWrappingMiddleware>();
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseSerilogRequestLogging();

        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        try
        {
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}