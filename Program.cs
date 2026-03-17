using MyApp.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IMockValidationService, MockValidationService>();
builder.Services.AddScoped<IMockAmendmentService, MockAmendmentService>();
builder.Services.AddSingleton<IMockPaymentService, MockPaymentService>();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

// API routes
app.MapControllers(); // This replaces the app.MapGet("/api/hello", ...)

app.MapFallbackToFile("/index.html");

app.Run();