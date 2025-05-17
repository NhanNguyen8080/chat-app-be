using ChatService.API.DataService;
using ChatService.Repository.UnitOfWork;
using ChatService.Service.Services;

namespace ChatService.API.Extensions
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<SharedDB>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            return services;
        }
    }
}
