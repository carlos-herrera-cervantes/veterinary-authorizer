using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Driver;
using Repositories.Repositories;
using Xunit;
using Domain.Models;

namespace Tests.Repositories;

[Collection("UserRepositoryIntegration")]
public class UserRepositoryIntegrationTests
{
    #region snippet_Properties

    private readonly IMongoClient _mongoClient;

    #endregion

    #region snippet_Constructors

    public UserRepositoryIntegrationTests()
    {
        string uri = Environment.GetEnvironmentVariable("MONGODB_URI");
        _mongoClient = new MongoClient(uri);
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return an empty document")]
    public async Task GetAsyncShouldReturnEmptyDocument()
    {
        var userRepository = new UserRepository(_mongoClient);
        var document = await userRepository.GetAsync(u => u.Email, "dummy-email@example.com");
        Assert.Null(document);
    }

    [Fact(DisplayName = "Should return an empty list")]
    public async Task GetAllAsyncShouldReturnEmptyList()
    {
        var userRepository = new UserRepository(_mongoClient);
        var list = await userRepository.GetAllAsync(u => u.Email, new List<string>());
        Assert.Empty(list);
    }

    [Fact(DisplayName = "Should return 0 documents")]
    public async Task CountAsyncShouldReturn0()
    {
        var userRepository = new UserRepository(_mongoClient);
        var counter = await userRepository.CountAsync(Builders<User>.Filter.Empty);
        Assert.Equal(0, counter);
    }

    [Fact(DisplayName = "Should create, update and delete a user")]
    public async Task CreateUpdateAndDeleteAsync()
    {
        var userRepository = new UserRepository(_mongoClient);
        var user = new User
        {
            Email = "user@example.com",
            Password = "secret",
        };
        await userRepository.CreateAsync(user);
        var counterBeforeDelete = await userRepository.CountAsync(Builders<User>.Filter.Empty);

        Assert.NotNull(user.Id);
        Assert.Equal(1, counterBeforeDelete);

        var insertResult = await userRepository.GetAsync(u => u.Id, user.Id);
        Assert.NotNull(insertResult);

        insertResult.Block = true;
        await userRepository.UpdateByIdAsync(insertResult.Id, insertResult);

        var updateResult = await userRepository.GetAsync(u => u.Id, user.Id);
        Assert.True(updateResult.Block);

        await userRepository.DeleteManyAsync(Builders<User>.Filter.Empty);
        var counterAfterDelete = await userRepository.CountAsync(Builders<User>.Filter.Empty);
        
        Assert.Equal(0, counterAfterDelete);
    }

    #endregion
}
