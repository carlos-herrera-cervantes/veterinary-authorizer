using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using MongoDB.Driver;
using Domain.Models;
using Domain.Constants;

namespace Repositories.Repositories;

public class UserRepository : IUserRepository
{
    #region snippet_Properties

    private readonly IMongoCollection<User> _collection;

    #endregion

    #region snippet_Constructors

    public UserRepository(IMongoClient mongoClient)
        => _collection = mongoClient.GetDatabase(MongoConfig.DefaultDatabase).GetCollection<User>("users");

    #endregion

    #region snippet_ActionMethods

    public async Task<User> GetAsync(FilterDefinition<User> filter) => await _collection.FindAsync(filter).Result.FirstOrDefaultAsync();

    public async Task<IEnumerable<User>> GetAllAsync(FilterDefinition<User> filter) => await _collection.FindAsync(filter).Result.ToListAsync();

    public async Task<int> CountAsync(FilterDefinition<User> filter) => (int)await _collection.CountDocumentsAsync(filter);

    public async Task CreateAsync(User user) => await _collection.InsertOneAsync(user);

    public async Task UpdateByIdAsync(FilterDefinition<User> filter, User user) => await _collection.ReplaceOneAsync(filter, user);

    public async Task UpdateManyAsync(FilterDefinition<User> filter, UpdateDefinition<User> updateDefinition)
    {
        var listWrites = new List<WriteModel<User>>();
        listWrites.Add(new UpdateManyModel<User>(filter, updateDefinition));

        await _collection.BulkWriteAsync(listWrites);
    }

    public async Task DeleteManyAsync(FilterDefinition<User> filter) => await _collection.DeleteManyAsync(filter);

    #endregion
}
