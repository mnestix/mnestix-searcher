using MnestixSearcher.ApiServices;
using MnestixSearcher.ApiServices.Settings;
using MnestixSearcher.Schemes.Query;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AasSearchDatabaseSettings>(
    builder.Configuration.GetSection("AasSearcherDatabase"));

builder.Services.AddRepositoryServices(builder.Configuration);
builder.Services.AddMnestixServices(builder.Configuration);

//Add GraphQl
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting();

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

app.MapGraphQL();

app.MapControllers();

app.Run();
