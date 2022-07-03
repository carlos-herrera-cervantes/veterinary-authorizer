using System.Threading.Tasks;
using Domain.Models;

namespace Repositories.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetByStringFieldAsync(string field, string value);

        public Task CreateAsync(User user);

        public Task UpdateByIdAsync(string id, User user);
    }
}
