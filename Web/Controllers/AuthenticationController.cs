using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Repositories.Repositories;
using Services;
using Services.Types;
using Web.Models;
using Web.Types;
using Web.Config;


namespace Web.Controllers;

[Route($"{ApiConfig.BasePath}/v1")]
public class AuthenticationController : ControllerBase
{
    #region snippet_Properties

    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly IPasswordHasher _passwordHasher;

    private readonly ITokenManager _tokenManager;

    private readonly IUserSessionRepository _userSessionRepository;

    private readonly IOperationHandler<UserCreatedEvent> _userCreatedEvent;

    private readonly IOperationHandler<UserVerificationEvent> _userVerificationEvent;

    #endregion

    #region snippet_Constructors

    public AuthenticationController
    (
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ITokenManager tokenManager,
        IUserSessionRepository userSessionRepository,
        IOperationHandler<UserCreatedEvent> userCreatedEvent,
        IOperationHandler<UserVerificationEvent> userVerificationEvent
    )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenManager = tokenManager;
        _userSessionRepository = userSessionRepository;
        _userCreatedEvent = userCreatedEvent;
        _userVerificationEvent = userVerificationEvent;
    }

    #endregion

    #region snippet_ActionMethods

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] CreateUser credentials)
    {
        var user = await _userRepository.GetAsync(u => u.Email, credentials.Email);

        if (user is null || !_passwordHasher.Verify(credentials.Password, user.Password))
        {
            return NotFound();
        }

        if (!user.Verified || user.Block)
        {
            return new UnauthorizedObjectResult(new HttpResponseMessage
            {
                Message = "Your account is not verified or is locked."
            });
        }

        var token = _tokenManager.GetJwt(user);
        await _userSessionRepository.SetJwtAsync(token, $"jwt:{user.Email}");

        return Ok(new HttpResponseMessage
        {
            Message = token
        });
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync([FromHeader(Name = "user-email")] string userEmail)
    {
        await _userSessionRepository.DropJwtAsync(new List<string> { $"jwt:{userEmail}" });
        return NoContent();
    }

    [HttpPost("sign-up/customers")]
    public async Task<IActionResult> SignupCustomerAsync([FromBody] CreateUser createUser)
    {
        var user = _mapper.Map<User>(createUser);
        user.Password = _passwordHasher.Hash(createUser.Password, 10);

        var token = _tokenManager.GetJwt(user);
        user.VerificationToken = token;

        await _userRepository.CreateAsync(user);

        EmitUserCreatedMessage(user, userType: "Customer");
        EmitUserVerificationMessage(user);

        return Ok(new HttpResponseMessage
        {
            Message = "A verification email was send to you"
        });
    }

    [HttpPost("sign-up/employees")]
    public async Task<IActionResult> SignupEmployeeAsync([FromBody] CreateUser createUser)
    {
        var user = _mapper.Map<User>(createUser);
        user.Roles = new List<string> { "Employee" };
        user.Type = UserType.Organization;
        user.Password = _passwordHasher.Hash(createUser.Password, 10);

        var token = _tokenManager.GetJwt(user);
        user.VerificationToken = token;

        await _userRepository.CreateAsync(user);

        EmitUserCreatedMessage(user, userType: "Organization");
        EmitUserVerificationMessage(user);

        return Ok(new HttpResponseMessage
        {
            Message = "A verification email was send to the employee"
        });
    }

    #endregion

    #region snippet_Helpers

    private void EmitUserCreatedMessage(User user, string userType)
    {
        var message = new UserCreatedEvent
        {
            UserId = user.Id,
            Email = user.Email,
            Type = userType,
            Roles = user.Roles
        };
        _userCreatedEvent.Publish(message);
    }

    private void EmitUserVerificationMessage(User user)
    {
        var message = new UserVerificationEvent
        {
            To = user.Email,
            Subject = "¡Welcome!",
            UserType = user.Type,
            Jwt = user.VerificationToken
        };
        _userVerificationEvent.Publish(message);
    }

    #endregion
}
