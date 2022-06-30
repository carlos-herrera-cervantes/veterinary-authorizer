using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;

namespace Web.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisClient(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration["Redis:ConnectionString"];
            services.AddSingleton<IRedisClientsManagerAsync>(c => new RedisManagerPool(connectionString));
            return services;
        }
    }
}
