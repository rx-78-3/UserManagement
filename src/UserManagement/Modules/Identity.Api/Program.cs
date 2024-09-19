using Common.DataAccess.Users;
using Common.Exceptions.Handler;
using Identity.Api;
using Identity.Api.Services;
using Identity.Api.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration["ConnectionStrings:SqlServer"]!;

// Add services to the container.
builder.Services
    .AddCors(options =>
     {
         options.AddPolicy(
             "AllowSpecificOrigin",
             policy => policy.WithOrigins(configuration["ServiceAddresses:FrontendUrl"]!)
                 .AllowAnyHeader()
                 .AllowAnyMethod());
     })
    .AddExceptionHandler<ServiceWideExceptionHandler>()
    .AddScoped<IUserRepository>(_ => new UserRepository(connectionString))
    .AddSingleton<IConfiguration>(builder.Configuration)
    .AddSingleton<IPasswordHasher, PasswordHasher>()
    .AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigin");

app.MapPost("/login", async (IAuthService authService, ILogger<Program> logger, LoginModel model) =>
{
    if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
    {
        logger.LogWarning("The username or password is empty.");
        return Results.BadRequest();
    }

    var user = await authService.ValidateUserAsync(model.UserName, model.Password);

    if (user == null)
    {
        logger.LogWarning($"Failed login attempt made by user: {model.UserName}.");
        return Results.Unauthorized();
    }

    if (!user.IsActive)
    {
        logger.LogWarning($"User {model.UserName} is not active.");
        return Results.Problem("User is not active", statusCode: StatusCodes.Status403Forbidden);
    }

    var token = authService.GenerateJwtToken(user);

    logger.LogInformation($"User {model.UserName} logged in successfully.");
    return Results.Ok(new { Token = token }); // ToDo: Add refresh token.
})
    .WithName("Login")
    .WithSummary("Login")
    .WithDescription("Login")
    .Produces(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

app.UseExceptionHandler(options => { });

app.Run();