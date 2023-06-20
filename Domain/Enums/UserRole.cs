using System;

namespace Domain.Enums;

public static class UserRole
{
    public const string Customer = "Customer";

    public const string Admin = "Admin";

    public const string Employee = "Employee";
}

public static class BootstrapUser
{
    public static readonly string Super = Environment.GetEnvironmentVariable("SUPER_USER");

    public static readonly string SuperPassword = Environment.GetEnvironmentVariable("SUPER_USER_PASSWORD");
}
