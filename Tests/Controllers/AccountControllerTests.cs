using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using MongoDB.Driver;
using Moq;
using Moq.Protected;
using Repositories.Repositories;
using Web.Controllers;
using Web.Models;
using Domain.Models;

namespace Tests.Controllers;

[Collection("AccountController")]
public class AccountControllerTests
{
    #region snippet_Properties

    private readonly Mock<IUserRepository> _mockUserRepository;

    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    private readonly Mock<IUserSessionRepository> _mockUserSessionRepository;

    #endregion

    #region snippet_Constructors

    public AccountControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockUserSessionRepository = new Mock<IUserSessionRepository>();
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return the fallback html whe request fails")]
    public async Task VerifyAccountAsyncShouldReturnFallbackContentResult()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:4566");

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
            .Verifiable();
        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()))
            .ReturnsAsync(new User { Type = "Organization" });

        var accountController = new AccountController
        (
            _mockUserRepository.Object,
            _mockHttpClientFactory.Object,
            _mockUserSessionRepository.Object
        );
        var response = await accountController.VerifyAccountAsync(jwt: "dummy-token");
        var expectedContent = "<html><body><h1>Something went wrong. Comunicate with support.</h1></body></html>";

        _mockUserRepository
            .Verify(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()), Times.Once);

        Assert.IsType<ContentResult>(response);
        Assert.Equal("text/html", response.ContentType);
        Assert.Equal(expectedContent, response.Content);
    }

    [Fact(DisplayName = "Should return success verification template")]
    public async Task VerifyAccountAsyncShouldReturnSuccessTemplate()
    {
        var mockDelegatingHandler = new Mock<DelegatingHandler>();
        var httpClient = new HttpClient(mockDelegatingHandler.Object);
        httpClient.BaseAddress = new Uri("http://localhost:4566");

        var expectedContent = "<html><body><h1>S3 Content</h1></body></html>";

        _mockHttpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient)
            .Verifiable();
        mockDelegatingHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedContent)
            })
            .Verifiable();
        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()))
            .ReturnsAsync(new User { Id = "630954b1c339766f61ec8d0c", Type = "Organization" });
        _mockUserRepository
            .Setup(x => x.UpdateByIdAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<User>()))
            .Returns(Task.FromResult(true));

        var accountController = new AccountController(
            _mockUserRepository.Object,
            _mockHttpClientFactory.Object,
            _mockUserSessionRepository.Object
        );
        var response = await accountController.VerifyAccountAsync(jwt: "dummy-token");

        _mockUserRepository
            .Verify(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()), Times.Once);
        _mockUserRepository
            .Verify(x => x.UpdateByIdAsync(It.IsAny<FilterDefinition<User>>(), It.IsAny<User>()), Times.Once);

        Assert.IsType<ContentResult>(response);
        Assert.Equal("text/html", response.ContentType);
        Assert.Equal(expectedContent, response.Content);
    }

    [Fact(DisplayName = "Should return bad request when list is empty")]
    public async Task SwitchLockedAsyncShouldReturnBadRequest()
    {
        var accountController = new AccountController
        (
            _mockUserRepository.Object,
            _mockHttpClientFactory.Object,
            _mockUserSessionRepository.Object
        );
        var lockUser = new LockUser
        {
            Emails = new List<string>()
        };
        var response = await accountController.SwitchLockedAsync(lockUser);

        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact(DisplayName = "Should return not found when some element in the list does not exist")]
    public async Task SwitchLockedAsyncShouldReturnNotFound()
    {
        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()))
            .ReturnsAsync(() => null);

        var accountController = new AccountController
        (
            _mockUserRepository.Object,
            _mockHttpClientFactory.Object,
            _mockUserSessionRepository.Object
        );
        var lockUser = new LockUser
        {
            Emails = new List<string> { "test@example.com" }
        };
        var response = await accountController.SwitchLockedAsync(lockUser);

        _mockUserRepository
            .Verify(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()), Times.Once);

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact(DisplayName = "Should return no content when process success")]
    public async Task SwitchLockedAsyncShouldReturnNoContent()
    {
        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()))
            .ReturnsAsync(new User { Id = "630954b1c339766f61ec8d0c", Type = "Organization", Block = false });
        _mockUserRepository
            .Setup(x => x.UpdateManyAsync(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<UpdateDefinition<User>>()
            ))
            .Returns(Task.FromResult(true));

        var accountController = new AccountController(
            _mockUserRepository.Object,
            _mockHttpClientFactory.Object,
            _mockUserSessionRepository.Object
        );
        var lockUser = new LockUser
        {
            Emails = new List<string> { "test@example.com" }
        };
        var response = await accountController.SwitchLockedAsync(lockUser);

        _mockUserRepository.Verify(x => x.GetAsync(It.IsAny<FilterDefinition<User>>()), Times.Once);

        Assert.IsType<NoContentResult>(response);
    }

    #endregion
}
