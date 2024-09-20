using Common.DataAccess.Users;
using Common.Exceptions.Handler;
using Identity.Api.Models;
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

    // Swagger.
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()

    .AddExceptionHandler<ServiceWideExceptionHandler>()

    // Add services.
    .AddScoped<IUserRepository>(_ => new UserRepository(connectionString))
    .AddSingleton<IConfiguration>(builder.Configuration)
    .AddSingleton<IPasswordHasher, PasswordHasher>()
    .AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigin");

app.MapPost("/login", async (IAuthService authService, ILogger<Program> logger, LoginRequest model) =>
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
    return Results.Ok(new LoginResponse(token)); // ToDo: Add refresh token.
})
    .WithName("Login")
    .WithSummary("Login")
    .WithDescription("Login")
    .Produces<LoginResponse>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

app.UseExceptionHandler(options => { });

if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.Run();