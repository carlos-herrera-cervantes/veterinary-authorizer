using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;
using MongoDB.Driver;

namespace Repositories.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetAsync(Expression<Func<User, string>> expression, string value);

        public Task<IEnumerable<User>> GetAllAsync
        (
            Expression<Func<User, string>> expression,
            List<string> values
        );

        public Task CreateAsync(User user);

        public Task UpdateByIdAsync(string id, User user);

        public Task UpdateManyAsync<T, K>
        (
            Expression<Func<User, T>> filterExpression,
            List<T> values,
            Expression<Func<User, K>> updateExpression,
            K value
        ) where T : class;
    }
}
