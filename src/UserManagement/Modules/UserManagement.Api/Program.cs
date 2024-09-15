using Carter;
using Common.DataAccess.Users;
using Common.Exceptions.Handler;
using Common.PipelineBehaviors;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var congiguration = builder.Configuration;
var assembly = typeof(Program).Assembly;

// Add services to the container.
builder.Services
    .AddCarter()
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(assembly);
        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    })
    .AddValidatorsFromAssembly(assembly)
    .AddExceptionHandler<ServiceWideExceptionHandler>()
    .AddScoped<IUserRepository>(_ => new UserRepository(congiguration["ConnectionStrings:SqlServer"]!));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
app.UseExceptionHandler(options => { });

app.Run();