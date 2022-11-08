using System.Threading.Tasks;
using Domain.Models;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Repositories.Repositories;

public class UserRepository : IUserRepository
{
    #region snippet_Properties

    private readonly IMongoCollection<User> _collection;

    #endregion

    #region snippet_Constructors

    public UserRepository(IMongoClient mongoClient)
    {
        string database = Environment.GetEnvironmentVariable("DEFAULT_DB");
        _collection = mongoClient.GetDatabase(database).GetCollection<User>("users");
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<User> GetAsync(Expression<Func<User, string>> expression, string value)
    {
        var filter = Builders<User>.Filter.Eq(expression, value);
        return await _collection.FindAsync(filter).Result.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, string>> expression, List<string> values)
    {
        var filter = Builders<User>.Filter.In(expression, values);
        return await _collection.FindAsync(filter).Result.ToListAsync();
    }

    public async Task<int> CountAsync(FilterDefinition<User> filter) => (int)await _collection.CountDocumentsAsync(filter);

    public async Task CreateAsync(User user) => await _collection.InsertOneAsync(user);

    public async Task UpdateByIdAsync(string id, User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        await _collection.ReplaceOneAsync(filter, user);
    }

    public async Task UpdateManyAsync<T, K>(Expression<Func<User, T>> filterExpression, List<T> values, Expression<Func<User, K>> updateExpression, K value)
        where T : class
    {
        var listWrites = new List<WriteModel<User>>();
        var filterDefinition = Builders<User>.Filter.In(filterExpression, values);
        var updateDefinition = Builders<User>.Update.Set(updateExpression, value);

        listWrites.Add(new UpdateManyModel<User>(filterDefinition, updateDefinition));

        await _collection.BulkWriteAsync(listWrites);
    }

    public async Task DeleteManyAsync(FilterDefinition<User> filter) => await _collection.DeleteManyAsync(filter);

    #endregion
}
