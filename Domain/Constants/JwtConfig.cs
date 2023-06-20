using System;

namespace Domain.Constants;

public static class JwtConfig
{
	public static readonly string SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
}
