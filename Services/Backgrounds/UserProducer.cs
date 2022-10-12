using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Domain.Constants;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Services.Types;

namespace Services.Backgrounds;

public class UserProducer : BackgroundService
{
    #region snippet_Properties

    private readonly IOperationHandler<UserCreatedEvent> _operationHandler;

    #endregion

    #region snippet_Constructors

    public UserProducer(IOperationHandler<UserCreatedEvent> operationHandler)
        => _operationHandler = operationHandler;

    #endregion

    #region snippet_ActionMethods

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriberFn = async (UserCreatedEvent userCreatedEvent) =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS"),
                ClientId = Environment.GetEnvironmentVariable("CLIENT_ID")
            };
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var userCreatedEventStr = JsonConvert.SerializeObject(userCreatedEvent);
            var message = new Message<Null, string> { Value = userCreatedEventStr };

            await producer.ProduceAsync(KafkaTopic.UserCreated, message);
        };

        _operationHandler.Subscribe
        (
            "UserProducer",
            async userCreatedEvent => await subscriberFn(userCreatedEvent)
        );

        return Task.CompletedTask;
    }

    #endregion
}
