using ChatService.Repository.Data;
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
