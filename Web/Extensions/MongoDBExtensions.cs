using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Web.Extensions
{
    public static class MongoDBExtensions
    {
        public static IServiceCollection AddMongoDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            string uri = configuration["MongoDb:Uri"];
            var mongoClient = new MongoClient(uri);

            services.AddSingleton<MongoClient>(_ => mongoClient);
            return services;
        }
    }
}
