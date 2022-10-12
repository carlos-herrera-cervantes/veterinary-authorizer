using System.Collections.Generic;
using Domain.Models;
using Services;
using Xunit;

namespace Tests.Services;

[Collection("TokenManager")]
public class TokenManagerTests
{
    #region snippet_Tests

    [Fact(DisplayName = "Should generate a JWT")]
    public void GetJwtShouldReturnToken()
    {
        var tokenManager = new TokenManager();

        var user = new User
        {
            Email = "test@example.com",
            Roles = new List<string> { "Employee" },
            Id = "dummy-id"
        };
        var jwt = tokenManager.GetJwt(user);

        Assert.NotNull(jwt);
        Assert.NotEmpty(jwt);
    }

    #endregion
}
