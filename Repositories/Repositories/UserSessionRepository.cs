using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace Repositories.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    #region snippet_Properties

    private readonly IRedisClientsManagerAsync _redisClients;

    #endregion

    #region snippet_Constructors

    public UserSessionRepository(IRedisClientsManagerAsync redisClients)
        => _redisClients = redisClients;

    #endregion

    #region snippet_ActionMethods

    public async Task DropJwtAsync(IEnumerable<string> keys)
    {
        var client = await _redisClients.GetClientAsync();
        await client.RemoveAllAsync(keys);
    }

    public async Task SetJwtAsync(string jwt, string key)
    {
        var client = await _redisClients.GetClientAsync();
        await client.SetAsync<string>(key, jwt);
    }

    #endregion
}
