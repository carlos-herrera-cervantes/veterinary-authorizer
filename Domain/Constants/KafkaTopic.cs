using System;

namespace Domain.Constants;

public static class KafkaTopic
{
    public const string UserCreated = "veterinary-user-created";

    public const string UserVerification = "send-email";
}

public static class KafkaConfig
{
    public static readonly string BootstrapServer = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");

    public static readonly string ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
}
