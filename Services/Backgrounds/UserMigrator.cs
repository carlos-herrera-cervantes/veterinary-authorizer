using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Repositories.Repositories;
using Domain.Models;
using Domain.Enums;

namespace Services.Backgrounds;

public class UserMigrator : IHostedService
{
    #region snippet_Properties

    private readonly IUserRepository _userRepository;

    private readonly IPasswordHasher _passwordHasher;

    #endregion

    #region snippet_Constructors

    public UserMigrator(IUserRepository userRepository, IPasswordHasher passwordHasher)
        => (_userRepository, _passwordHasher) = (userRepository, passwordHasher);

    #endregion

    #region snippet_ActionMethods

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, BootstrapUser.Super);
        User user = await _userRepository.GetAsync(filter);

        if (user is not null) return;

        await _userRepository.CreateAsync(new User
        {
            Email = BootstrapUser.Super,
            Password = _passwordHasher.Hash(BootstrapUser.SuperPassword, 10),
            Roles = new List<string> { UserRole.Admin },
            Type = UserType.Organization,
            Verified = true
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #endregion
}
