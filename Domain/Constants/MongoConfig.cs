using System;

namespace Domain.Constants;

public static class MongoConfig
{
    public static readonly string DefaultDatabase = Environment.GetEnvironmentVariable("DEFAULT_DB");

    public static readonly string ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");

    public static readonly string Uri = Environment.GetEnvironmentVariable("MONGODB_URI");
}
