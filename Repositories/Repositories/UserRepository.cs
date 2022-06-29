using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region snippet_Properties

        private readonly IMongoCollection<User> _collection;

        #endregion

        #region snippet_Constructors

        public UserRepository(MongoClient mongoClient, IConfiguration configuration)
            => _collection = mongoClient
            .GetDatabase(configuration["MongoDb:Default"])
            .GetCollection<User>("users");

        #endregion

        #region snippet_ActionMethods

        public async Task<User> GetOneAsync(Expression<Func<User, bool>> expression)
            => await _collection.Find(expression).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) => await _collection.InsertOneAsync(user);

        public async Task UpdateByIdAsync(string id, User user)
            => await _collection.ReplaceOneAsync(u => u.Id == id, user);

        #endregion
    }
}
