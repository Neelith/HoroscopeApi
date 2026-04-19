using HoroscopeApi.WebApi.Constants;
using HoroscopeApi.WebApi.Infrastructure.Setup.Authorization.ApiKeyPolicy;

namespace HoroscopeApi.WebApi.Endpoints.Healthz;

public sealed class HealthEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("healthz")
            .AllowAnonymous()
            .WithTags(Tags.Healthz)
            .WithDescription("Health check endpoints")
            .WithMetadata(new ApiKeyScopeMetadata("healthz"));

        group.MapGet("", async () =>
            {
                return Results.Ok(new { status = "Healthy" });
            })
            .WithName("GetHealth")
            .WithDescription("Get the health status of the API.")
            .Produces(StatusCodes.Status200OK);
    
    }
}