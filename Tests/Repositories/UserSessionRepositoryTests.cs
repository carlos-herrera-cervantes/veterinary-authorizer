using System.Threading;
using System.Threading.Tasks;
using Moq;
using Repositories.Repositories;
using ServiceStack.Redis;
using Xunit;

namespace Tests.Repositories
{
    [Collection("UserSessionRepository")]
    public class UserSessionRepositoryTests
    {
        #region snippet_Properties

        private readonly Mock<IRedisClientsManagerAsync> _mockRedisClientsManager;

        #endregion

        #region snippet_Constructors

        public UserSessionRepositoryTests()
        {
            _mockRedisClientsManager = new Mock<IRedisClientsManagerAsync>();
        }

        #endregion

        #region snippet_Tests

        [Fact(DisplayName = "Should call the remove async method")]
        public async Task DropJwt()
        {
            var mockRedisClient = new Mock<IRedisClientAsync>();

            _mockRedisClientsManager
                .Setup(x => x.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockRedisClient.Object);

            mockRedisClient
                .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var userSessionRepository = new UserSessionRepository(_mockRedisClientsManager.Object);
            await userSessionRepository.DropJwtAsync("dummy-key");

            mockRedisClient
                .Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should call the set async method")]
        public async Task SetJwt()
        {
            var mockRedisClient = new Mock<IRedisClientAsync>();

            _mockRedisClientsManager
                .Setup(x => x.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockRedisClient.Object);

            mockRedisClient
                .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var userSessionRepository = new UserSessionRepository(_mockRedisClientsManager.Object);
            await userSessionRepository.SetJwtAsync("dummy-key", "dummy-token");

            mockRedisClient
                .Verify(x => x.SetAsync
                    (
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    ),
                    Times.Once
                );
        }

        #endregion
    }
}
