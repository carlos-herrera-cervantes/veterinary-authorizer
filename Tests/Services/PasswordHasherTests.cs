using Services;
using Xunit;

namespace Tests.Services;

[Collection("PasswordHasher")]
public class PasswordHasherTests
{
    #region snippet_Tests

    [Theory(DisplayName = "Should hash and compare password")]
    [InlineData("secret123", true)]
    public void HashAndComparePassword(string password, bool expected)
    {
        var passwordHasher = new PasswordHasher();
        var hashedPassword = passwordHasher.Hash(password, 10);
        var result = passwordHasher.Verify(password, hashedPassword);
        Assert.Equal(expected, result);
    }

    #endregion
}
