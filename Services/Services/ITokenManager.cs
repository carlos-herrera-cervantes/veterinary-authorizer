using Domain.Models;

namespace Services;

public interface ITokenManager
{
    public string GetJwt(User user);
}
