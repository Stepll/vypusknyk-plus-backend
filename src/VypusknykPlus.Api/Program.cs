using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using Serilog;
using VypusknykPlus.Api.Middleware;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.Services;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build())
    .CreateBootstrapLogger();

try
{

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

// --- Startup validation ---
var jwtKey = builder.Configuration["Jwt:Key"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT key is not configured. Set Jwt__Key environment variable.");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Database connection string is not configured. Set ConnectionStrings__DefaultConnection environment variable.");
if (string.IsNullOrWhiteSpace(builder.Configuration["Minio:Endpoint"]))
    throw new InvalidOperationException("MinIO endpoint is not configured. Set Minio__Endpoint environment variable.");

// --- Database ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Validation ---
builder.Services.AddValidatorsFromAssemblyContaining<AppDbContext>();
builder.Services.AddScoped<ValidationActionFilter>();

// --- Controllers + JSON ---
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// --- JWT Settings ---
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// --- Email Settings ---
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// --- MinIO Settings ---
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));

// --- MinIO Client (Singleton — thread-safe, manages connection pool internally) ---
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
    return new MinioClient()
        .WithEndpoint(settings.Endpoint)
        .WithCredentials(settings.AccessKey, settings.SecretKey)
        .WithSSL(settings.UseSSL)
        .Build();
});

// --- Services ---
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDesignService, DesignService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IAdminRoleService, AdminRoleService>();
builder.Services.AddScoped<IInfoPageService, InfoPageService>();

// --- CORS ---
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? [ "http://localhost:5173" ];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// --- JWT Authentication ---
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// --- Health Checks ---
builder.Services.AddHealthChecks();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Випускник+ API",
        Version = "v1",
        Description = "API для інтернет-магазину випускних стрічок та аксесуарів"
    });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введіть JWT токен",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Run migrations and initialize infrastructure on startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        startupLogger.LogInformation("Applying pending EF Core migrations...");
        await db.Database.MigrateAsync();
        startupLogger.LogInformation("Migrations applied successfully");
        await InfoPageSeeder.SeedAsync(db);
        startupLogger.LogInformation("Info page seed completed");

        startupLogger.LogInformation("Ensuring MinIO bucket exists...");
        await imageService.EnsureBucketAsync();
        startupLogger.LogInformation("MinIO bucket ready");
    }
    catch (Exception ex)
    {
        startupLogger.LogCritical(ex, "Startup initialization failed");
        throw;
    }
}

// --- Middleware pipeline ---
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/healthz");
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
