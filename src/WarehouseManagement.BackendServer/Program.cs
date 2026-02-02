using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Configure Serilog
// =======================
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
Log.Logger.Information("Application is building.........");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

// DbInitializer
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

// =======================
// Seed database
// =======================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        Log.Information("Seeding data...");
        var dbInitializer = services.GetRequiredService<DbInitializer>();
        await dbInitializer.Seed();
    }
    catch (Exception ex)
    {
        // Log bằng Serilog
        Log.Fatal(ex, "An error occurred while seeding the database.");

        // Log bằng ILogger (fallback)
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");

        throw; // dừng app nếu seed fail
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
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