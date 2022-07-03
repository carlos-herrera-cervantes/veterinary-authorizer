using System;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;

namespace Web.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisClient(this IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("REDIS_URI");
            services.AddSingleton<IRedisClientsManagerAsync>(c => new RedisManagerPool(connectionString));
            return services;
        }
    }
}
