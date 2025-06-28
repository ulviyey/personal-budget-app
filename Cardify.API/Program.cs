using Cardify.Core;
using Cardify.Core.Models;
using Cardify.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDbContext<CardifyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Cardify.API")
    ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/api/users/register", async (UserCreateDto dto, IUserService userService) =>
{
    var result = await userService.RegisterUserAsync(dto);
    if (!result)
        return Results.BadRequest("Registration failed. Username or email may already exist.");
    
    return Results.Ok(new { message = "User registered successfully." });
});

app.MapPost("/api/users/login", async (UserLoginDto dto, IUserService userService) =>
{
    var result = await userService.LoginUserAsync(dto);
    if (result == null)
        return Results.BadRequest("Invalid username/email or password.");
    
    return Results.Ok(result);
});

// Card CRUD Endpoints
app.MapGet("/api/cards", async (int userId, ICardService cardService) =>
{
    var cards = await cardService.GetUserCardsAsync(userId);
    return Results.Ok(cards);
});

app.MapGet("/api/cards/{id}", async (int id, int userId, ICardService cardService) =>
{
    var card = await cardService.GetCardByIdAsync(id, userId);
    if (card == null)
        return Results.NotFound("Card not found.");

    return Results.Ok(card);
});

app.MapPost("/api/cards", async (CardCreateDto dto, int userId, ICardService cardService) =>
{
    var cardId = await cardService.CreateCardAsync(dto, userId);
    if (cardId == 0)
        return Results.BadRequest("Card creation failed. Please check your input.");

    return Results.Ok(new { message = "Card created successfully.", cardId });
});

app.MapPut("/api/cards/{id}", async (int id, CardUpdateDto dto, int userId, ICardService cardService) =>
{
    var result = await cardService.UpdateCardAsync(id, dto, userId);
    if (!result)
        return Results.NotFound("Card not found.");

    return Results.Ok(new { message = "Card updated successfully." });
});

app.MapDelete("/api/cards/{id}", async (int id, int userId, ICardService cardService) =>
{
    var result = await cardService.DeleteCardAsync(id, userId);
    if (!result)
        return Results.NotFound("Card not found.");

    return Results.Ok(new { message = "Card deleted successfully." });
});

// Transaction CRUD Endpoints
app.MapGet("/api/transactions", async (int userId, int? cardId, string? type, DateTime? fromDate, DateTime? toDate, ITransactionService transactionService) =>
{
    var transactions = await transactionService.GetUserTransactionsAsync(userId, cardId, type, fromDate, toDate);
    return Results.Ok(transactions);
});

app.MapGet("/api/transactions/{id}", async (int id, int userId, ITransactionService transactionService) =>
{
    var transaction = await transactionService.GetTransactionByIdAsync(id, userId);
    if (transaction == null)
        return Results.NotFound("Transaction not found.");

    return Results.Ok(transaction);
});

app.MapPost("/api/transactions", async (TransactionCreateDto dto, int userId, ITransactionService transactionService) =>
{
    var transactionId = await transactionService.CreateTransactionAsync(dto, userId);
    if (transactionId == 0)
        return Results.BadRequest("Transaction creation failed. Please check your input.");

    return Results.Ok(new { message = "Transaction created successfully.", transactionId });
});

app.MapPut("/api/transactions/{id}", async (int id, TransactionUpdateDto dto, int userId, ITransactionService transactionService) =>
{
    var result = await transactionService.UpdateTransactionAsync(id, dto, userId);
    if (!result)
        return Results.NotFound("Transaction not found.");

    return Results.Ok(new { message = "Transaction updated successfully." });
});

app.MapDelete("/api/transactions/{id}", async (int id, int userId, ITransactionService transactionService) =>
{
    var result = await transactionService.DeleteTransactionAsync(id, userId);
    if (!result)
        return Results.NotFound("Transaction not found.");

    return Results.Ok(new { message = "Transaction deleted successfully." });
});

// Dashboard/Overview Endpoint
app.MapGet("/api/dashboard", async (int userId, IDashboardService dashboardService) =>
{
    try
    {
        var dashboardData = await dashboardService.GetDashboardDataAsync(userId);
        return Results.Ok(dashboardData);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

// User Logout Endpoint
app.MapPost("/api/users/logout", async (int userId, IUserService userService) =>
{
    var result = await userService.LogoutUserAsync(userId);
    if (!result)
        return Results.BadRequest("User not found.");

    return Results.Ok(new { message = "Logout successful." });
});

// Password Change Endpoint
app.MapPost("/api/users/change-password", async (PasswordChangeDto dto, int userId, IUserService userService) =>
{
    var result = await userService.ChangePasswordAsync(userId, dto);
    if (!result)
        return Results.BadRequest("Password change failed. Please check your current password.");

    return Results.Ok(new { message = "Password changed successfully." });
});

app.Run();
