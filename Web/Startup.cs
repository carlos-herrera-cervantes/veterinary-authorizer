using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories.Repositories;
using Services;
using Services.Backgrounds;
using Web.Extensions;
using Domain.Constants;

namespace Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson();
        services.AddMongoDbClient();
        services.AddAutoMapperConfig();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenManager, TokenManager>();
        services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
        services.AddSingleton(typeof(IOperationHandler<>), typeof(OperationHandler<>));
        services.AddJwtAuthentication();
        services.AddRedisClient();
        services.AddHttpClient("veterinary", c =>
        {
            c.BaseAddress = new Uri(S3Config.Host);
        });
        services.AddHostedService<UserProducer>();
        services.AddHostedService<UserVerificationProducer>();
        services.AddHostedService<UserMigrator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
