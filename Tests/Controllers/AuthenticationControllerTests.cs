using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repositories.Repositories;
using Services;
using Services.Types;
using Web.Controllers;
using Web.Models;
using Xunit;

namespace Tests.Controllers;

[Collection("AuthenticationController")]
public class AuthenticationControllerTests
{
    #region snippet_Properties

    private readonly Mock<IUserRepository> _mockUserRepository;

    private readonly Mock<IMapper> _mockMapper;

    private readonly Mock<IPasswordHasher> _mockPasswordHasher;

    private readonly Mock<ITokenManager> _mockTokenManager;

    private readonly Mock<IUserSessionRepository> _mockUserSessionRepository;

    private readonly Mock<IOperationHandler<UserCreatedEvent>> _mockOperationHandler;

    private readonly Mock<IOperationHandler<UserVerificationEvent>> _mockUserVerificationEvent;

    #endregion

    #region snippet_Constructors

    public AuthenticationControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockTokenManager = new Mock<ITokenManager>();
        _mockUserSessionRepository = new Mock<IUserSessionRepository>();
        _mockOperationHandler = new Mock<IOperationHandler<UserCreatedEvent>>();
        _mockUserVerificationEvent = new Mock<IOperationHandler<UserVerificationEvent>>();
    }

    #endregion

    #region snippet_Tests

    [Fact(DisplayName = "Should return a 200 status code response for success sign in")]
    public async Task SigninShouldReturn200Response()
    {
        var user = new User
        {
            Email = "test@example.com",
            Verified = true,
        };

        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>()))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        _mockTokenManager
            .Setup(x => x.GetJwt(It.IsAny<User>()))
            .Returns("dummy-token");

        _mockUserSessionRepository
            .Setup(x => x.SetJwtAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(true));

        var authenticationController = new AuthenticationController
        (
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object,
            _mockTokenManager.Object,
            _mockUserSessionRepository.Object,
            _mockOperationHandler.Object,
            _mockUserVerificationEvent.Object
        );

        var credentials = new CreateUser
        {
            Email = "test@example.com",
            Password = "secret123"
        };
        var response = await authenticationController.SignInAsync(credentials);
        var okObjectResult = response as OkObjectResult;

        _mockUserRepository
            .Verify(x => x.GetAsync(It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>()), Times.Once);

        _mockPasswordHasher
            .Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _mockTokenManager
            .Verify(x => x.GetJwt(It.IsAny<User>()), Times.Once);

        _mockUserSessionRepository
            .Verify(x => x.SetJwtAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        Assert.IsType<OkObjectResult>(response);
        Assert.IsType<Web.Types.HttpResponseMessage>(okObjectResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
    }

    [Fact(DisplayName = "Should return a 404 status code response when user does not exist in sign in")]
    public async Task SigninShouldReturn404ResponseWhenUserNull()
    {
        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>()))
            .ReturnsAsync(() => null);

        var authenticationController = new AuthenticationController
        (
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object,
            _mockTokenManager.Object,
            _mockUserSessionRepository.Object,
            _mockOperationHandler.Object,
            _mockUserVerificationEvent.Object
        );

        var credentials = new CreateUser
        {
            Email = "test@example.com",
            Password = "secret123"
        };
        var response = await authenticationController.SignInAsync(credentials);
        var notFoundResult = response as NotFoundResult;

        _mockUserRepository
            .Verify(x => x.GetAsync(It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>()), Times.Once);

        _mockPasswordHasher
            .Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        Assert.IsType<NotFoundResult>(response);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact(DisplayName = "Should return a 404 status code response when password mismatch in sign in")]
    public async Task SigninShouldReturn404ResponseWhenPasswordMismatch()
    {
        var user = new User
        {
            Email = "test@example.com",
            Verified = true,
        };

        _mockUserRepository
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>()))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var authenticationController = new AuthenticationController
        (
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object,
            _mockTokenManager.Object,
            _mockUserSessionRepository.Object,
            _mockOperationHandler.Object,
            _mockUserVerificationEvent.Object
        );

        var credentials = new CreateUser
        {
            Email = "test@example.com",
            Password = "secret123"
        };
        var response = await authenticationController.SignInAsync(credentials);
        var notFoundResult = response as NotFoundResult;

        _mockUserRepository
            .Verify(x => x.GetAsync(It.IsAny<Expression<Func<User, string>>>(), It.IsAny<string>()), Times.Once);

        _mockPasswordHasher
            .Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        Assert.IsType<NotFoundResult>(response);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact(DisplayName = "Should return a 204 status code response in sign out")]
    public async Task SignoutShouldReturn204Response()
    {
        _mockUserSessionRepository
            .Setup(x => x.DropJwtAsync(It.IsAny<IEnumerable<string>>()))
            .Returns(Task.FromResult(true));

        var authenticationController = new AuthenticationController
        (
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object,
            _mockTokenManager.Object,
            _mockUserSessionRepository.Object,
            _mockOperationHandler.Object,
            _mockUserVerificationEvent.Object
        );
        var response = await authenticationController.SignOutAsync("test@example.com");
        var noContentResult = response as NoContentResult;

        _mockUserSessionRepository
            .Verify(x => x.DropJwtAsync(It.IsAny<IEnumerable<string>>()), Times.Once);

        Assert.IsType<NoContentResult>(response);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact(DisplayName = "Should return a 200 status code Response creating a customer")]
    public async Task SignupCustomerAsyncShouldReturn200Response()
    {
        var user = new User { Email = "test@example.com" };

        _mockMapper
            .Setup(x => x.Map<User>(It.IsAny<CreateUser>()))
            .Returns(user);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(user));

        _mockPasswordHasher
            .Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<int>()))
            .Returns("hashed-password");

        _mockOperationHandler
            .Setup(x => x.Publish(It.IsAny<UserCreatedEvent>()));

        var authenticationController = new AuthenticationController
        (
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object,
            _mockTokenManager.Object,
            _mockUserSessionRepository.Object,
            _mockOperationHandler.Object,
            _mockUserVerificationEvent.Object
        );

        var newUser = new CreateUser { Email = "test@example.com", Password = "secret123" };
        var response = await authenticationController.SignupCustomerAsync(newUser);
        var okObjectResult = response as OkObjectResult;

        _mockMapper
            .Verify(x => x.Map<User>(It.IsAny<CreateUser>()), Times.Once);

        _mockUserRepository
            .Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);

        _mockPasswordHasher
            .Verify(x => x.Hash(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

        _mockOperationHandler
            .Verify(x => x.Publish(It.IsAny<UserCreatedEvent>()), Times.Once);

        Assert.IsType<OkObjectResult>(response);
        Assert.IsType<Web.Types.HttpResponseMessage>(okObjectResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
    }

    [Fact(DisplayName = "Should return a 200 status code response creating an employee")]
    public async Task SignupEmployeeAsyncShouldReturn200Response()
    {
        var user = new User { Email = "test@example.com" };

        _mockMapper
            .Setup(x => x.Map<User>(It.IsAny<CreateUser>()))
            .Returns(user);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(user));

        _mockPasswordHasher
            .Setup(x => x.Hash(It.IsAny<string>(), It.IsAny<int>()))
            .Returns("hashed-password");

        _mockOperationHandler
            .Setup(x => x.Publish(It.IsAny<UserCreatedEvent>()));

        var authenticationController = new AuthenticationController
        (
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockPasswordHasher.Object,
            _mockTokenManager.Object,
            _mockUserSessionRepository.Object,
            _mockOperationHandler.Object,
            _mockUserVerificationEvent.Object
        );

        var newUser = new CreateUser { Email = "test@example.com", Password = "secret123" };
        var response = await authenticationController.SignupEmployeeAsync(newUser);
        var okObjectResult = response as OkObjectResult;

        _mockMapper
            .Verify(x => x.Map<User>(It.IsAny<CreateUser>()), Times.Once);

        _mockUserRepository
            .Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);

        _mockPasswordHasher
            .Verify(x => x.Hash(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

        _mockOperationHandler
            .Verify(x => x.Publish(It.IsAny<UserCreatedEvent>()), Times.Once);

        Assert.IsType<OkObjectResult>(response);
        Assert.IsType<Web.Types.HttpResponseMessage>(okObjectResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
    }

    #endregion
}
