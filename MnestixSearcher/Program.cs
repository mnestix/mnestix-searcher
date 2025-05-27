using Microsoft.OpenApi.Models;
using MnestixSearcher.ApiServices;
using MnestixSearcher.ApiServices.Settings;
using MnestixSearcher.Authorization;
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

// in some classes we need the base url of the request
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("customApiKeyPolicy", policyBuilder => policyBuilder
    .AddRequirements(new ApiKeyRequirement()));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. X-API-KEY: {apikey}",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGraphQL();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
