using Kirov_DA_test.Middleware;
using Kirov_DA_test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IVulnerabilityStore, VulnerabilityStore>();
builder.Services.AddSingleton<IScanTaskStore, ScanTaskStore>();

builder.Services.AddSingleton<ScanBackgroundService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ScanBackgroundService>());

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
