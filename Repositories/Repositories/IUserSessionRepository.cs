using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IUserSessionRepository
    {
        public Task SetJwt(string jwt, string key);

        public Task DropJwt(string key);
    }
}
