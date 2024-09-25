using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Finance.Data;
using Finance.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog yap�land�rmas�
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(@"C:\Logs\FinanceApp\log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Redis ba�lant�s�
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    configuration.ResolveDns = true;
    return ConnectionMultiplexer.Connect(configuration);
});

// Veritaban� ba�lant�s�
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FinanceContext>(options =>
    options.UseSqlServer(connectionString));

// JWT kimlik do�rulamas� ayarlar�
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

// Swagger yap�land�rmas�
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Finance API",
        Version = "v1"
    });

    // Swagger i�in JWT ayarlar�
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Bearer token'� 'Bearer {token}' format�nda girin.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Servisleri ekleme
builder.Services.AddScoped<IDataAccessService, DataAccessService>();
builder.Services.AddScoped<IAuthService, AuthService>();  
builder.Services.AddScoped<IStockTransService, StockTransService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInvoiceDetailsService, InvoiceDetailsService>();
builder.Services.AddScoped<IActTransService, ActTransService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();

// IStockService ve StockService'i DI sistemine ekliyoruz
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddControllers();

// CORS yap�land�rmas�
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Geli�tirme ortam�nda Swagger'� etkinle�tir
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Finance API v1");
    });
}

// HTTPS y�nlendirme
app.UseHttpsRedirection();

// CORS devreye al
app.UseCors("AllowAll");

// Kimlik do�rulama ve yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// Controller'lar� yap�land�rma
app.MapControllers();

// Uygulama ba�lat�l�rken hata yakalama ve loglama
try
{
    Log.Information("Uygulama ba�lat�l�yor...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama ba�lat�lamad�.");
}
finally
{
    Log.CloseAndFlush();
}
