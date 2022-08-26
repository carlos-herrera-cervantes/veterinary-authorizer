using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;

namespace Repositories.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetAsync(Expression<Func<User, string>> expression, string value);

        public Task CreateAsync(User user);

        public Task UpdateByIdAsync(string id, User user);
    }
}
