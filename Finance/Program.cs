using Microsoft.EntityFrameworkCore;
using Finance.Data;  // FinanceContext'in bulundu�u namespace


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext ayarlar�n� ekleyin
builder.Services.AddDbContext<FinanceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger ekleme (e�er gerekiyorsa)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

builder.Services.AddDbContext<FinanceContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Uygulamay� yap�land�r�n.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();//

app.UseAuthorization();

app.MapControllers();

app.Run();
