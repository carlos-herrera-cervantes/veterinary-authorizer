using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain.Constants;
using Domain.Enums;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Services.Types;

namespace Services.Backgrounds;

public class UserVerificationProducer : BackgroundService
{
    #region snippet_Properties

    private readonly IOperationHandler<UserVerificationEvent> _operationHandler;

    private readonly HttpClient _httpClient;

    #endregion

    #region snippet_Constructors

    public UserVerificationProducer
    (
        IOperationHandler<UserVerificationEvent> operationHandler,
        IHttpClientFactory clientFactory
    )
    {
        _operationHandler = operationHandler;
        _httpClient = clientFactory.CreateClient("veterinary");
    }

    #endregion

    #region snippet_ActionMethods

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriberFn = async (UserVerificationEvent userVerificationEvent) =>
        {
            var welcomeTemplate = userVerificationEvent.UserType == UserType.Organization
                ? "/veterinary-statics/welcome-employee.html"
                : "/veterinary-statics/welcome-customer.html";
            using var httpResponse = await _httpClient.GetAsync(welcomeTemplate);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return;
            }

            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS"),
                ClientId = Environment.GetEnvironmentVariable("CLIENT_ID")
            };
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var appHost = Environment.GetEnvironmentVariable("APP_HOST");
            var verificationEndpoint = userVerificationEvent.UserType == UserType.Organization
                ? appHost + $"/api/v1/authentication/employee/verification/{userVerificationEvent.Jwt}"
                : appHost + $"/api/v1/authentication/customer/verification/{userVerificationEvent.Jwt}";

            var stringContent = await httpResponse.Content.ReadAsStringAsync();
            var finalWelcomeTemplate = stringContent.Replace("{{placeholder}}", verificationEndpoint);

            userVerificationEvent.Body = finalWelcomeTemplate;

            var messageString = JsonConvert.SerializeObject(userVerificationEvent);
            var message = new Message<Null, string> { Value = messageString };

            await producer.ProduceAsync(KafkaTopic.UserVerification, message);
        };

        _operationHandler.Subscribe
        (
            "UserVerificationProducer",
            async userVerificationEvent => await subscriberFn(userVerificationEvent)
        );

        return Task.CompletedTask;
    }

    #endregion
}
