using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SafeVault.Data;
using SafeVault.Services;
using SafeVault.Models;
using SafeVault.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("SafeVaultDb"));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();
