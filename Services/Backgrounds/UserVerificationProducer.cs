using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using Newtonsoft.Json;
using Services.Types;
using Domain.Constants;
using Domain.Enums;

namespace Services.Backgrounds;

public class UserVerificationProducer : BackgroundService
{
    #region snippet_Properties

    private readonly IOperationHandler<UserVerificationEvent> _operationHandler;

    private readonly HttpClient _httpClient;

    private readonly ILogger _logger;

    #endregion

    #region snippet_Constructors

    public UserVerificationProducer(
        IOperationHandler<UserVerificationEvent> operationHandler,
        IHttpClientFactory clientFactory,
        ILogger<UserVerificationProducer> logger
    )
    {
        _operationHandler = operationHandler;
        _httpClient = clientFactory.CreateClient("veterinary");
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    private async Task<HttpResponseMessage> GetVerificationTemplateAsync(string welcomeTemplateUrl)
    {
        try
        {
            using var httpResponse = await _httpClient.GetAsync(welcomeTemplateUrl);
            return httpResponse;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while getting the welcome template", e.Message);
        }

        return null;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriberFn = async (UserVerificationEvent userVerificationEvent) =>
        {
            var welcomeTemplateUrl = userVerificationEvent.UserType == UserType.Organization
                ? "/veterinary-statics/welcome-employee.html"
                : "/veterinary-statics/welcome-customer.html";
            HttpResponseMessage httpResponse = await GetVerificationTemplateAsync(welcomeTemplateUrl);

            if (httpResponse is null) return;

            var config = new ProducerConfig
            {
                BootstrapServers = KafkaConfig.BootstrapServer,
                ClientId = KafkaConfig.ClientId
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

        _operationHandler.Subscribe(
            "UserVerificationProducer",
            async userVerificationEvent => await subscriberFn(userVerificationEvent)
        );

        return Task.CompletedTask;
    }

    #endregion
}
