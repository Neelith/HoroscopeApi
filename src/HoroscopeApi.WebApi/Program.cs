using HoroscopeApi.WebApi.Infrastructure.Setup;

// Create the web application builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddAppServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline
app.UseAppServices();

app.Run();
