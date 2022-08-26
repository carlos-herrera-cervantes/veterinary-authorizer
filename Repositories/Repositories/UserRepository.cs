using System.Threading.Tasks;
using Domain.Models;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Repositories.Repositories
{
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

        public async Task CreateAsync(User user) => await _collection.InsertOneAsync(user);

        public async Task UpdateByIdAsync(string id, User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _collection.ReplaceOneAsync(filter, user);
        }

        #endregion
    }
}
