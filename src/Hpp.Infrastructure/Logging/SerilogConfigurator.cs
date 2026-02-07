using Microsoft.Extensions.Configuration;
using Serilog;

namespace Hpp.Infrastructure.Logging;

/// <summary>
/// Serilog configuration helper.
/// </summary>
public static class SerilogConfigurator
{
    public static ILogger Configure(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}
