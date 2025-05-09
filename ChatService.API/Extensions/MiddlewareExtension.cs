using ChatService.API.Middlewares;

namespace ChatService.API.Extensions
{
    public static class MiddlewareExtension
    { 
        public static WebApplication UseJwtMiddleware(this WebApplication app)
        {
            app.UseMiddleware<JwtMiddleware>();
            return app;
        }

        public static WebApplication UseErrorHandlingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            return app;
        }
    }
}
