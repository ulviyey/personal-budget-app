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

app.MapPost("/api/users/login", async (UserLoginDto dto, CardifyDbContext db) =>
{
    // Validate input
    if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
        return Results.BadRequest("Username/email and password are required.");

    // Find user by username or email
    var user = await db.Users.FirstOrDefaultAsync(u => 
        u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

    if (user == null)
        return Results.BadRequest("Invalid username/email or password.");

    // Verify password
    string hashedPassword = HashPassword(dto.Password);
    if (user.PasswordHash != hashedPassword)
        return Results.BadRequest("Invalid username/email or password.");

    if (!user.IsActive)
        return Results.BadRequest("Account is deactivated.");

    // Update last login time (optional)
    user.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(new { 
        message = "Login successful.",
        user = new { 
            id = user.Id, 
            username = user.Username, 
            email = user.Email, 
            name = user.Name 
        }
    });
});

// Card CRUD Endpoints
app.MapGet("/api/cards", async (int userId, CardifyDbContext db) =>
{
    var cards = await db.Cards
        .Where(c => c.UserId == userId)
        .Select(c => new
        {
            c.Id,
            c.CardType,
            c.LastFourDigits,
            c.CardHolderName,
            c.CardColorStart,
            c.CardColorEnd,
            c.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(cards);
});

app.MapGet("/api/cards/{id}", async (int id, int userId, CardifyDbContext db) =>
{
    var card = await db.Cards
        .Where(c => c.Id == id && c.UserId == userId)
        .Select(c => new
        {
            c.Id,
            c.CardType,
            c.LastFourDigits,
            c.CardHolderName,
            c.CardColorStart,
            c.CardColorEnd,
            c.CreatedAt,
            c.UpdatedAt
        })
        .FirstOrDefaultAsync();

    if (card == null)
        return Results.NotFound("Card not found.");

    return Results.Ok(card);
});

app.MapPost("/api/cards", async (CardCreateDto dto, int userId, CardifyDbContext db) =>
{
    // Validate input
    if (string.IsNullOrWhiteSpace(dto.CardType) || string.IsNullOrWhiteSpace(dto.LastFourDigits) || string.IsNullOrWhiteSpace(dto.CardHolderName))
        return Results.BadRequest("Card type, last four digits, and card holder name are required.");

    // Check if user exists
    var user = await db.Users.FindAsync(userId);
    if (user == null)
        return Results.BadRequest("User not found.");

    var card = new Card
    {
        CardType = dto.CardType,
        LastFourDigits = dto.LastFourDigits,
        CardHolderName = dto.CardHolderName,
        CardColorStart = dto.CardColorStart,
        CardColorEnd = dto.CardColorEnd,
        UserId = userId,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = userId
    };

    db.Cards.Add(card);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Card created successfully.", cardId = card.Id });
});

app.MapPut("/api/cards/{id}", async (int id, CardUpdateDto dto, int userId, CardifyDbContext db) =>
{
    var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    if (card == null)
        return Results.NotFound("Card not found.");

    // Update only provided fields
    if (!string.IsNullOrWhiteSpace(dto.CardType))
        card.CardType = dto.CardType;
    if (!string.IsNullOrWhiteSpace(dto.LastFourDigits))
        card.LastFourDigits = dto.LastFourDigits;
    if (!string.IsNullOrWhiteSpace(dto.CardHolderName))
        card.CardHolderName = dto.CardHolderName;
    if (!string.IsNullOrWhiteSpace(dto.CardColorStart))
        card.CardColorStart = dto.CardColorStart;
    if (!string.IsNullOrWhiteSpace(dto.CardColorEnd))
        card.CardColorEnd = dto.CardColorEnd;

    card.UpdatedAt = DateTime.UtcNow;
    card.UpdatedBy = userId;

    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Card updated successfully." });
});

app.MapDelete("/api/cards/{id}", async (int id, int userId, CardifyDbContext db) =>
{
    var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    if (card == null)
        return Results.NotFound("Card not found.");

    db.Cards.Remove(card);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Card deleted successfully." });
});

// Transaction CRUD Endpoints
app.MapGet("/api/transactions", async (int userId, int? cardId, string? type, DateTime? fromDate, DateTime? toDate, CardifyDbContext db) =>
{
    var query = db.Transactions.Where(t => t.UserId == userId);

    // Apply filters
    if (cardId.HasValue)
        query = query.Where(t => t.CardId == cardId.Value);
    if (!string.IsNullOrWhiteSpace(type))
        query = query.Where(t => t.Type == type);
    if (fromDate.HasValue)
        query = query.Where(t => t.Date >= fromDate.Value);
    if (toDate.HasValue)
        query = query.Where(t => t.Date <= toDate.Value);

    var transactions = await query
        .Include(t => t.Card)
        .OrderByDescending(t => t.Date)
        .Select(t => new
        {
            t.Id,
            t.Name,
            t.Date,
            t.Amount,
            t.Type,
            Card = new
            {
                t.Card!.Id,
                t.Card.CardType,
                t.Card.LastFourDigits,
                t.Card.CardHolderName
            },
            t.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(transactions);
});

app.MapGet("/api/transactions/{id}", async (int id, int userId, CardifyDbContext db) =>
{
    var transaction = await db.Transactions
        .Include(t => t.Card)
        .Where(t => t.Id == id && t.UserId == userId)
        .Select(t => new
        {
            t.Id,
            t.Name,
            t.Date,
            t.Amount,
            t.Type,
            Card = new
            {
                t.Card!.Id,
                t.Card.CardType,
                t.Card.LastFourDigits,
                t.Card.CardHolderName
            },
            t.CreatedAt,
            t.UpdatedAt
        })
        .FirstOrDefaultAsync();

    if (transaction == null)
        return Results.NotFound("Transaction not found.");

    return Results.Ok(transaction);
});

app.MapPost("/api/transactions", async (TransactionCreateDto dto, int userId, CardifyDbContext db) =>
{
    // Validate input
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Amount <= 0 || string.IsNullOrWhiteSpace(dto.Type))
        return Results.BadRequest("Name, amount, and type are required.");

    // Check if user exists
    var user = await db.Users.FindAsync(userId);
    if (user == null)
        return Results.BadRequest("User not found.");

    // Check if card exists and belongs to user
    var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == dto.CardId && c.UserId == userId);
    if (card == null)
        return Results.BadRequest("Card not found or does not belong to user.");

    var transaction = new Transaction
    {
        Name = dto.Name,
        Date = dto.Date,
        Amount = dto.Amount,
        Type = dto.Type,
        CardId = dto.CardId,
        UserId = userId,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = userId
    };

    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Transaction created successfully.", transactionId = transaction.Id });
});

app.MapPut("/api/transactions/{id}", async (int id, TransactionUpdateDto dto, int userId, CardifyDbContext db) =>
{
    var transaction = await db.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    if (transaction == null)
        return Results.NotFound("Transaction not found.");

    // Update only provided fields
    if (!string.IsNullOrWhiteSpace(dto.Name))
        transaction.Name = dto.Name;
    if (dto.Date.HasValue)
        transaction.Date = dto.Date.Value;
    if (dto.Amount.HasValue && dto.Amount.Value > 0)
        transaction.Amount = dto.Amount.Value;
    if (!string.IsNullOrWhiteSpace(dto.Type))
        transaction.Type = dto.Type;

    // If CardId is being updated, validate it belongs to user
    if (dto.CardId.HasValue && dto.CardId.Value != transaction.CardId)
    {
        var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == dto.CardId.Value && c.UserId == userId);
        if (card == null)
            return Results.BadRequest("Card not found or does not belong to user.");
        
        transaction.CardId = dto.CardId.Value;
    }

    transaction.UpdatedAt = DateTime.UtcNow;
    transaction.UpdatedBy = userId;

    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Transaction updated successfully." });
});

app.MapDelete("/api/transactions/{id}", async (int id, int userId, CardifyDbContext db) =>
{
    var transaction = await db.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    if (transaction == null)
        return Results.NotFound("Transaction not found.");

    db.Transactions.Remove(transaction);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Transaction deleted successfully." });
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
