using Carter;
using Common.DataAccess.Users;
using Common.DataAccess.Users.Migrations;
using Common.Exceptions.Handler;
using Common.PipelineBehaviors;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var assembly = typeof(Program).Assembly;
var connectionString = configuration["ConnectionStrings:SqlServer"]!;
var masterConnectionString = configuration["ConnectionStrings:MasterSqlServer"]!;

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
    .AddExceptionHandler<ServiceWideExceptionHandler>()
    .AddScoped<IUserRepository>(_ => new UserRepository(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
app.UseCors("AllowSpecificOrigin");
app.UseExceptionHandler(options => { });

await DatabaseInitialization.InitializeDatabase(masterConnectionString, connectionString);
if (app.Environment.IsDevelopment())
{
    await TestDataInitialization.InitializeTestData(connectionString);
}

app.Run();