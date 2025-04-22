using ChatService.API.DataService;

namespace ChatService.API.Extensions
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<SharedDB>();

            return services;
        }
    }
}
