using Vladify.AdminApi.Extensions;
using Vladify.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.ConfigureInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Vladify API v1");

        var clientId = builder.Configuration["Auth0Options:ClientId"];

        options.OAuthClientId(clientId);

        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
