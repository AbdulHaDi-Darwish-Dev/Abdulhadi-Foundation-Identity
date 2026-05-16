namespace Abdulhadi.Foundation.Identity.Api.Extensions;

public static class CorsAppExtensions
{
    public static WebApplication UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("DefaultPolicy");

        return app;
    }
}