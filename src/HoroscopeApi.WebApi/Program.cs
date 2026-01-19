using HoroscopeApi.WebApi.Infrastructure.Setup;

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAppServices();

app.Run();
