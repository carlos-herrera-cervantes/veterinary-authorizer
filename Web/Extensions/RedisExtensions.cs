using System;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;

namespace Web.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisClient(this IServiceCollection services)
    {
        services.AddSingleton<IRedisClientsManagerAsync>(c => new RedisManagerPool(Domain.Constants.RedisConfig.Uri));
        return services;
    }
}
