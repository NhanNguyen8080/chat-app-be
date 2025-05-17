using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace ChatService.API.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            //This setup is useful if API uses JWT-based authentication and test it easily in Swagger.
            services.AddSwaggerGen(options =>
            {
                //This defines the security scheme that Swagger will use for authentication.
                options.AddSecurityDefinition("Bearer JWT" /* The name of the security scheme */,
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization", //The name of the header where the token will be passed
                        Type = SecuritySchemeType.Http, //Specifies that this is an HTTP authentication scheme.
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Fill in the JWT Token that follows this format (Bearer [token])",
                    });
                //This enforces the security requirement globally,
                //meaning all API endpoints will require a Bearer token for authentication.
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }

                });
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Example = new OpenApiString("2000-01-01")
                });
            });
            return services;
        }
        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }
    }
}
