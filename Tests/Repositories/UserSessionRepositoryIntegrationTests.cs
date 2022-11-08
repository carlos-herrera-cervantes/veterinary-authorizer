using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Repositories;
using ServiceStack.Redis;
using Xunit;

namespace Tests.Repositories;

[Collection("UserSessionRepositoryIntegration")]
public class UserSessionRepositoryIntegrationTests
{
    #region snippet_Properties

    private readonly IRedisClientsManagerAsync _redisClient;

    #endregion

    #region snippet_Constructors

    public UserSessionRepositoryIntegrationTests()
    {
        string connectionString = Environment.GetEnvironmentVariable("REDIS_URI");
        _redisClient = new RedisManagerPool(connectionString);
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should")]
    public async Task SetGetAndDropAsync()
    {
        var userSessionRepository = new UserSessionRepository(_redisClient);
        var key = "jwt:user@example.com";
        var value = "dummy-jwt";

        await userSessionRepository.SetJwtAsync(value, key);
        var jwtBeforeDelete = await userSessionRepository.GetJwtAsync(key);

        Assert.Equal(value, jwtBeforeDelete);

        await userSessionRepository.DropJwtAsync(new List<string>{ key });
        var jwtAfterDelete = await userSessionRepository.GetJwtAsync(key);

        Assert.Null(jwtAfterDelete);
    }

    #endregion
}
