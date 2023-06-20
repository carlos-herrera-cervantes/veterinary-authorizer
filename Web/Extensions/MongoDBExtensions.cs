using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Domain.Constants;

namespace Web.Extensions;

public static class MongoDBExtensions
{
    public static IServiceCollection AddMongoDbClient(this IServiceCollection services)
    {
        var mongoClient = new MongoClient(MongoConfig.Uri);
        services.AddSingleton<IMongoClient>(_ => mongoClient);
        return services;
    }
}
