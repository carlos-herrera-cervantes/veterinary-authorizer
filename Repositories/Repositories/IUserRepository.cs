using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Domain.Models;

namespace Repositories.Repositories;

public interface IUserRepository
{
    public Task<User> GetAsync(FilterDefinition<User> expression);

    public Task<IEnumerable<User>> GetAllAsync(FilterDefinition<User> filter);

    public Task<int> CountAsync(FilterDefinition<User> filter);

    public Task CreateAsync(User user);

    public Task UpdateByIdAsync(FilterDefinition<User> filter, User user);

    public Task UpdateManyAsync(FilterDefinition<User> filter, UpdateDefinition<User> updateDefinition);

    public Task DeleteManyAsync(FilterDefinition<User> filter);
}
