using Cardify.Core;
using Microsoft.EntityFrameworkCore;
using Cardify.Core.Models;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDbContext<CardifyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Cardify.API")
    ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/api/users/register", async (UserCreateDto dto, CardifyDbContext db) =>
{
    // Validate input (automatic with [ApiController], but manual here)
    if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        return Results.BadRequest("Username, email, and password are required.");

    if (await db.Users.AnyAsync(u => u.Username == dto.Username))
        return Results.BadRequest("Username already exists.");
    if (await db.Users.AnyAsync(u => u.Email == dto.Email))
        return Results.BadRequest("Email already exists.");

    // Hash password
    string passwordHash = HashPassword(dto.Password);

    var user = new User
    {
        Username = dto.Username,
        Email = dto.Email,
        Name = dto.Name,
        PasswordHash = passwordHash,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "User registered successfully." });
});

// Simple password hashing (SHA256 for demo; use a stronger method in production)
static string HashPassword(string password)
{
    using var sha = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}

app.Run();
