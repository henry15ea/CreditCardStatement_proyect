using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CreditCardStatement.API.HealthChecks;

public class ApplicationHealthCheck : IHealthCheck
{
    private static readonly DateTime _startTime = DateTime.UtcNow;
    private readonly IHostEnvironment _environment;

    public ApplicationHealthCheck(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyName = assembly.GetName().Name ?? "Unknown";
        var version = assembly.GetName().Version?.ToString() ?? "Unknown";
        var uptime = DateTime.UtcNow - _startTime;

        var data = new Dictionary<string, object>
        {
            { "version", version },
            { "uptime", uptime.ToString(@"dd\.hh\:mm\:ss") },
            { "environment", _environment.EnvironmentName },
            { "assemblyName", assemblyName }
        };

        return Task.FromResult(HealthCheckResult.Healthy("Application is running", data));
    }
}
