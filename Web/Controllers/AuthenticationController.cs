using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories;
using Services;
using Services.Types;
using Web.Models;
using Web.Types;

namespace Web.Controllers
{
    [Route("api/v1/authentication")]
    public class AuthenticationController : ControllerBase
    {
        #region snippet_Properties

        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly IPasswordHasher _passwordHasher;

        private readonly ITokenManager _tokenManager;

        private readonly IUserSessionRepository _userSessionRepository;

        private readonly IOperationHandler<UserCreatedEvent> _userCreatedEvent;

        #endregion

        #region snippet_Constructors

        public AuthenticationController
        (
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            ITokenManager tokenManager,
            IUserSessionRepository userSessionRepository,
            IOperationHandler<UserCreatedEvent> userCreatedEvent
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _tokenManager = tokenManager;
            _userSessionRepository = userSessionRepository;
            _userCreatedEvent = userCreatedEvent;
        }

        #endregion

        #region snippet_ActionMethods

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignInAsync([FromBody] CreateUser credentials)
        {
            var user = await _userRepository.GetByStringFieldAsync("email", credentials.Email);

            if (user is null || !_passwordHasher.Verify(credentials.Password, user.Password))
            {
                return NotFound();
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
            await _userSessionRepository.DropJwtAsync($"jwt:{userEmail}");
            return NoContent();
        }

        [HttpPost("sign-up/customers")]
        public async Task<IActionResult> SignupCustomerAsync([FromBody] CreateUser createUser)
        {
            var user = _mapper.Map<User>(createUser);
            user.Password = _passwordHasher.Hash(createUser.Password, 10);

            await _userRepository.CreateAsync(user);
            EmitUserCreatedMessage(user, userType: "Customer");

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
            user.Type = "Organization";
            user.Password = _passwordHasher.Hash(createUser.Password, 10);

            await _userRepository.CreateAsync(user);
            EmitUserCreatedMessage(user, userType: "Organization");

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

        #endregion
    }
}
