using System;

namespace Domain.Constants;

public static class RedisConfig
{
    public static readonly string Uri = Environment.GetEnvironmentVariable("REDIS_URI");
}
