using System;


namespace ScheduleManagement.Extensions
{
    public static class ExternalEnv
    {
        public static string? APP_CONNECTION_STRING = Environment.GetEnvironmentVariable("APP_CONNECTION_STRING");

    }
}
