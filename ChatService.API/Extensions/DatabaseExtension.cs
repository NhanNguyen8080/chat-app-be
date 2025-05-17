using ChatService.Repository.Data;
using ChatService.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatService.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services)
        {
            services.AddDbContext<ChatDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionString(), b => b.MigrationsAssembly("ChatService.API"));
            }, ServiceLifetime.Scoped);
            return services;
        }

        public static WebApplication SeedData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

                var userRole = new Role { RoleName = "User" };
                context.Roles.Add(userRole);
                context.SaveChanges();
            }
            return app;
        }

        private static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            return config["ConnectionStrings:DefaultConnection"];
        }
    }
}
