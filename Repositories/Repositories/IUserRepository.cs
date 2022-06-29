using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;

namespace Repositories.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetOneAsync(Expression<Func<User, bool>> expression);

        public Task CreateAsync(User user);

        public Task UpdateByIdAsync(string id, User user);
    }
}
