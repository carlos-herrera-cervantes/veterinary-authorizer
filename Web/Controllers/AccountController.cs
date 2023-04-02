using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories;
using Domain.Enums;
using Web.Models;
using Web.Config;

namespace Web.Controllers;

[Route($"{ApiConfig.BasePath}/v1/account")]
public class AccountController : ControllerBase
{
    #region snippet_Properties

    private readonly IUserRepository _userRepository;

    private readonly HttpClient _httpClient;

    private readonly IUserSessionRepository _userSessionRepository;

    #endregion

    #region snippet_Constructors

    public AccountController
    (
        IUserRepository userRepository,
        IHttpClientFactory clientFactory,
        IUserSessionRepository userSessionRepository
    )
    {
        _userRepository = userRepository;
        _httpClient = clientFactory.CreateClient("veterinary");
        _userSessionRepository = userSessionRepository;
    }

    #endregion

    #region snippet_ActionMethods

    [HttpGet("verification/{jwt}")]
    public async Task<ContentResult> VerifyAccountAsync([FromRoute] string jwt)
    {
        var user = await _userRepository.GetAsync(u => u.VerificationToken, jwt);

        var successVerificationTemplate = user.Type == UserType.Organization
            ? "/veterinary-statics/success-employee-verification.html"
            : "/veterinary-statics/success-customer-verification.html";
        using var httpResponse = await _httpClient.GetAsync(successVerificationTemplate);

        if (!httpResponse.IsSuccessStatusCode || user is null)
        {
            return new ContentResult
            {
                Content = "<html><body><h1>Something went wrong. Comunicate with support.</h1></body></html>",
                ContentType = "text/html"
            };
        }

        user.Verified = true;
        await _userRepository.UpdateByIdAsync(user.Id, user);

        var stringContent = await httpResponse.Content.ReadAsStringAsync();
        return new ContentResult
        {
            Content = stringContent,
            ContentType = "text/html"
        };
    }

    [HttpPatch("lock")]
    public async Task<IActionResult> SwitchLockedAsync([FromBody] LockUser lockUser)
    {
        if (lockUser.Emails.Count == 0)
        {
            return new BadRequestObjectResult(new Types.HttpResponseMessage
            {
                Message = "empty list"
            });
        }

        var user = await _userRepository.GetAsync(u => u.Email, lockUser.Emails[0]);

        if (user is null)
        {
            return new NotFoundObjectResult(new Types.HttpResponseMessage
            {
                Message = "some elements in the list does not exist"
            });
        }

        var locked = !user.Block;
        await _userRepository.
            UpdateManyAsync<string, bool>(u => u.Email, lockUser.Emails, u => u.Block, locked);

        var keys = lockUser.Emails.Select(email => $"jwt:{email}");
        await _userSessionRepository.DropJwtAsync(keys);

        return NoContent();
    }

    #endregion
}
