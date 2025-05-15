using MnestixSearcher;
using MnestixSearcher.AasSearcher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AasSearchDatabaseSettings>(
    builder.Configuration.GetSection("AasSearcherDatabase"));
builder.Services.AddSingleton<AasSearcherService>();

builder.Services.AddOpenApi();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
