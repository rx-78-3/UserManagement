using Carter;
using Common.DataAccess.Users;
using Common.DataAccess.Users.Migrations;
using Common.Exceptions.Handler;
using Common.Middleware;
using Common.PipelineBehaviors;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var assembly = typeof(Program).Assembly;
var connectionString = configuration["ConnectionStrings:SqlServer"]!;
var masterConnectionString = configuration["ConnectionStrings:MasterSqlServer"]!;
var jwtConfigurationSection = configuration.GetSection("JwtSettings");
var secretKey = jwtConfigurationSection["SecretKey"]!;
var issuer = jwtConfigurationSection["Issuer"]!;
var audience = jwtConfigurationSection["Audience"]!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

// Add services to the container.
builder.Services
    .AddCarter()
    .AddCors(options =>
    {
        options.AddPolicy(
            "AllowSpecificOrigin",
            policy => policy.WithOrigins(configuration["ServiceAddresses:FrontendUrl"]!)
                .AllowAnyHeader()
                .AllowAnyMethod());
    })
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(assembly);
        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    })
    .AddValidatorsFromAssembly(assembly)

    // Swagger.
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()

    .AddExceptionHandler<ServiceWideExceptionHandler>()

    // Add services.
    .AddScoped<IUserRepository>(_ => new UserRepository(connectionString))

    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = key
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserIsActiveClaimMiddleware>();
app.MapCarter();
app.UseExceptionHandler(options => { });

await DatabaseInitialization.InitializeDatabase(masterConnectionString, connectionString);
if (app.Environment.IsDevelopment())
{
    await TestDataInitialization.InitializeTestData(connectionString);
    
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.Run();