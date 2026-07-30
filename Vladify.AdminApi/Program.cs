using Vladify.AdminApi.Extensions;
using Vladify.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.ConfigureInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalar(builder.Configuration);
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.ConfigureGrpcServices();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
