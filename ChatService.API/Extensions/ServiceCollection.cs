using ChatService.API.DataService;
using ChatService.Service.Services;

namespace ChatService.API.Extensions
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<SharedDB>();
            services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
            return services;
        }
    }
}
