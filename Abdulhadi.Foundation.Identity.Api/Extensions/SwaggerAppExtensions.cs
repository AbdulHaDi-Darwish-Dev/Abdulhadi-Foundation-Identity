namespace API.Extensions
{
    public static class SwaggerAppExtensions
    {
        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}