using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using MongoDB.Driver;
using Moq;
using Repositories.Repositories;
using Xunit;

namespace Tests.Repositories;

[Collection("UserRepository")]
public class UserRepositoryTests
{
    #region snippet_Properties

    private readonly Mock<IMongoClient> _mockMongoClient;

    private readonly Mock<IMongoCollection<User>> _mockMongoCollection;

    private readonly Mock<IMongoDatabase> _mockMongoDatabase;

    private readonly Mock<IAsyncCursor<User>> _mockMongoCursor;

    private readonly List<User> _users;

    #endregion

    #region snippet_Constructors

    public UserRepositoryTests()
    {
        _mockMongoClient = new Mock<IMongoClient>();
        _mockMongoCollection = new Mock<IMongoCollection<User>>();
        _mockMongoDatabase = new Mock<IMongoDatabase>();
        _mockMongoCursor = new Mock<IAsyncCursor<User>>();

        var user = new User { Email = "test@example.com" };
        var users = new List<User>();
        users.Add(user);

        _users = users;
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return null when user does not exist")]
    public async Task GetOneAsyncShouldReturnNull()
    {
        _mockMongoClient
            .Setup(x => x.GetDatabase(It.IsAny<string>(), default))
            .Returns(_mockMongoDatabase.Object);
        _mockMongoDatabase
            .Setup(x => x.GetCollection<User>("users", default))
            .Returns(_mockMongoCollection.Object);
        _mockMongoCursor.Setup(_ => _.Current).Returns(_users);
        _mockMongoCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(false));
        _mockMongoCollection
            .Setup(x => x.FindAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(_mockMongoCursor.Object);

        var userRepository = new UserRepository(_mockMongoClient.Object);
        var result = await userRepository.GetAsync(u => u.Email, "bad@example.com");

        _mockMongoCollection
            .Verify(x => x.FindAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

        Assert.Null(result);
    }

    [Fact(DisplayName = "Should return user")]
    public async Task GetOneAsyncShouldReturnUser()
    {
        _mockMongoClient
            .Setup(x => x.GetDatabase(It.IsAny<string>(), default))
            .Returns(_mockMongoDatabase.Object);
        _mockMongoDatabase
            .Setup(x => x.GetCollection<User>("users", default))
            .Returns(_mockMongoCollection.Object);
        _mockMongoCursor.Setup(_ => _.Current).Returns(_users);
        _mockMongoCursor
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        _mockMongoCollection
            .Setup(x => x.FindAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(_mockMongoCursor.Object);

        var userRepository = new UserRepository(_mockMongoClient.Object);
        var result = await userRepository.GetAsync(u => u.Email, "test@example.com");

        _mockMongoCollection
            .Verify(x => x.FindAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact(DisplayName = "Should return a list of users")]
    public async Task GetAllAsyncShouldReturnList()
    {
        _mockMongoClient
            .Setup(x => x.GetDatabase(It.IsAny<string>(), default))
            .Returns(_mockMongoDatabase.Object);
        _mockMongoDatabase
            .Setup(x => x.GetCollection<User>("users", default))
            .Returns(_mockMongoCollection.Object);
        _mockMongoCursor
            .Setup(x => x.Current)
            .Returns(_users);
        _mockMongoCollection
            .Setup(x => x.FindAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(_mockMongoCursor.Object);

        var userRepository = new UserRepository(_mockMongoClient.Object);
        var result = await userRepository
            .GetAllAsync(u => u.Email, new List<string> { "test@example.com" });

        _mockMongoCollection
            .Verify(x => x.FindAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

        Assert.Empty(result);
    }

    [Fact(DisplayName = "Should call the insert one async method")]
    public async Task CreateAsyncShouldCreateUser()
    {
        _mockMongoClient
            .Setup(x => x.GetDatabase(It.IsAny<string>(), default))
            .Returns(_mockMongoDatabase.Object);
        _mockMongoDatabase
            .Setup(x => x.GetCollection<User>("users", default))
            .Returns(_mockMongoCollection.Object);
        _mockMongoCollection
            .Setup(x => x.InsertOneAsync
                (
                    It.IsAny<User>(),
                    It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()
                ))
            .Returns(Task.FromResult((object)null));

        var userRepository = new UserRepository(_mockMongoClient.Object);
        var newUser = new User { Email = "new.user@example.com" };
        await userRepository.CreateAsync(newUser);

        _mockMongoCollection
            .Verify(x => x.InsertOneAsync
                (
                    It.IsAny<User>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
    }

    [Fact(DisplayName = "Should call the replace one async method")]
    public async Task UpdateByIdAsyncShouldUpdateUser()
    {
        _mockMongoClient
            .Setup(x => x.GetDatabase(It.IsAny<string>(), default))
            .Returns(_mockMongoDatabase.Object);
        _mockMongoDatabase
            .Setup(x => x.GetCollection<User>("users", default))
            .Returns(_mockMongoCollection.Object);

        var mockReplaceOneResult = new Mock<ReplaceOneResult>();
        mockReplaceOneResult.Setup(_ => _.IsAcknowledged).Returns(true);
        mockReplaceOneResult.Setup(_ => _.ModifiedCount).Returns(1);

        _mockMongoCollection
            .Setup(x => x.ReplaceOneAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<User>(), It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()
                ))
            .Returns(Task.FromResult(mockReplaceOneResult.Object));

        var userRepository = new UserRepository(_mockMongoClient.Object);
        var updatedUser = new User { Email = "new.user@example.com", Verified = true };
        await userRepository.UpdateByIdAsync("dummy-id", updatedUser);

        _mockMongoCollection
            .Verify(x => x.ReplaceOneAsync
                (
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<User>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
    }

    [Fact(DisplayName = "Should call the bulk write async method")]
    public async Task UpdateManyAsyncShouldUpdateUsers()
    {
        _mockMongoClient
            .Setup(x => x.GetDatabase(It.IsAny<string>(), default))
            .Returns(_mockMongoDatabase.Object);
        _mockMongoDatabase
            .Setup(x => x.GetCollection<User>("users", default))
            .Returns(_mockMongoCollection.Object);

        var mockBulkWriteResult = (BulkWriteResult<User>)new BulkWriteResult<User>.Acknowledged
        (
            200, 0, 0, 0,
            0,
            new List<WriteModel<User>>(),
            new List<BulkWriteUpsert>()
        );
        _mockMongoCollection
            .Setup(x => x.BulkWriteAsync
                (
                    It.IsAny<IEnumerable<WriteModel<User>>>(),
                    It.IsAny<BulkWriteOptions>(),
                    It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(mockBulkWriteResult);

        var userRepository = new UserRepository(_mockMongoClient.Object);
        var emails = new List<string> { "test@example.com" };
        await userRepository
            .UpdateManyAsync<string, bool>(u => u.Email, emails, u => u.Block, true);

        _mockMongoCollection
            .Verify(x => x.BulkWriteAsync
                (
                    It.IsAny<IEnumerable<WriteModel<User>>>(),
                    It.IsAny<BulkWriteOptions>(),
                    It.IsAny<CancellationToken>()
                ), Times.Once);
    }

    #endregion
}
