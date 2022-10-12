using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Web.Extensions;

public static class MongoDBExtensions
{
    public static IServiceCollection AddMongoDbClient(this IServiceCollection services)
    {
        string uri = Environment.GetEnvironmentVariable("MONGODB_URI");
        var mongoClient = new MongoClient(uri);

        services.AddSingleton<IMongoClient>(_ => mongoClient);
        return services;
    }
}
