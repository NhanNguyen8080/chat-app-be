﻿namespace ChatService.API.Extensions
{
    public static class CorsExtension
    {
        public static IServiceCollection AddCorsExtensions(this IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("chatApp",
                        builder =>
                        {
                            builder.WithOrigins("http://localhost:5173")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
            });

            return services;
        }

        public static WebApplication UseCors(this WebApplication app)
        {
            app.UseCors("chatApp");

            return app;
        }
    }
}
