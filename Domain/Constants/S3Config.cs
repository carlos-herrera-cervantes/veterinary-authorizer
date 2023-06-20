using System;

namespace Domain.Constants;

public static class S3Config
{
    public static readonly string Host = Environment.GetEnvironmentVariable("VETERINARY_STATICS");
}
