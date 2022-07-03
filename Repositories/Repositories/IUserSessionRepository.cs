using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IUserSessionRepository
    {
        public Task SetJwtAsync(string jwt, string key);

        public Task DropJwtAsync(string key);
    }
}
