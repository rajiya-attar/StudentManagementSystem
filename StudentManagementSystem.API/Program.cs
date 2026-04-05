using StudentManagementSystem.API.Extensions;
using StudentManagementSystem.API.Middleware;
using StudentManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Authorization
builder.Services.AddAuthorization();

// Add Application Services
builder.Services.AddApplicationServices();

// Add Swagger Documentation
builder.Services.AddSwaggerDocumentation();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management System API V1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Student Management System API";
    options.DefaultModelsExpandDepth(-1);
    options.DisplayRequestDuration();
});

// Use CORS
app.UseCors("AllowAll");

// Use Global Exception Handling Middleware
app.UseGlobalExceptionHandling();

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Use HTTPS Redirection
app.UseHttpsRedirection();

// Map Controllers
app.MapControllers();

// Ensure database and tables are created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Delete and recreate database to ensure clean state
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        
        // Verify tables exist by trying to query
        var userCount = dbContext.Users.Count();
        Console.WriteLine($"Database initialized. Users table has {userCount} records.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"Stack: {ex.StackTrace}");
    }
}

app.Run();
