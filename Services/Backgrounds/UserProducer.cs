using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Confluent.Kafka;
using Services.Types;
using Domain.Constants;

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
                BootstrapServers = KafkaConfig.BootstrapServer,
                ClientId = KafkaConfig.ClientId
            };
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var userCreatedEventStr = JsonConvert.SerializeObject(userCreatedEvent);
            var message = new Message<Null, string> { Value = userCreatedEventStr };

            await producer.ProduceAsync(KafkaTopic.UserCreated, message);
        };

        _operationHandler.Subscribe(
            "UserProducer",
            async userCreatedEvent => await subscriberFn(userCreatedEvent)
        );

        return Task.CompletedTask;
    }

    #endregion
}
