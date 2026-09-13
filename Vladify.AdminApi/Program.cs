using Vladify.AdminApi.Config;
using Vladify.AdminApi.Extensions;
using Vladify.Application.Extensions;
using Vladify.Infrastructure.Extensions;

EnvLoader.LoadEnvVariables();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.ConfigureInfrastructure();

var app = builder.Build();

await app.Services.MigrateDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalar(builder.Configuration);
}

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureGrpcServices();
app.MapControllers();

await app.RunAsync();
