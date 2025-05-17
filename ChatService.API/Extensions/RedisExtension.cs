using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Microsoft.Extensions;

namespace ChatService.API.Extensions
{
    public static class RedisExtension
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = GetConnectionStringsRedis();
                options.InstanceName = "ChatService";
            });

            return services;
        }

        private static string GetConnectionStringsRedis()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var strConn = config["ConnectionStrings:Redis"];
            return strConn;
        }
    }
}
