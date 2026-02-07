using Hpp.Application.Interfaces;
using Hpp.Application.Mapping;
using Hpp.Application.Services;
using Hpp.Infrastructure.Data;
using Hpp.Infrastructure.Integrations;
using Hpp.Infrastructure.Logging;
using Hpp.Infrastructure.Repositories;
using Hpp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Hpp.Infrastructure;

/// <summary>
/// Service registration for HPP.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddHppServices(this IServiceCollection services, IConfiguration configuration)
    {
        var useMockIntegrationsRaw = configuration["Integrations:UseMock"];
        var useMockIntegrations = !bool.TryParse(useMockIntegrationsRaw, out var parsedUseMock) || parsedUseMock;
        var openAiApiKey = configuration["OPENAI_API_KEY"];

        services.AddDbContext<HppDbContext>(options =>
        {
            var conn = configuration.GetConnectionString("HppDb");
            if (string.IsNullOrWhiteSpace(conn) || conn.Contains("__DEV_DB_CONN__", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase("HppDev");
            }
            else
            {
                options.UseSqlServer(conn);
            }
        });

        services.AddSingleton<ILogger>(_ => SerilogConfigurator.Configure(configuration));
        services.AddAutoMapper(typeof(HppMappingProfile));

        services.AddScoped<IInventoryRepository, EfInventoryRepository>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISeedService, SeedService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IScenarioSimulator, ScenarioSimulatorService>();

        if (useMockIntegrations)
        {
            services.AddSingleton<IInvoiceParser, InvoiceParserMock>();
        }
        else
        {
            services.AddSingleton<IInvoiceParser, FormRecognizerAdapter>();
        }

        services.AddSingleton<IInvoiceIngestor, InvoiceIngestor>();
        services.AddSingleton<IOpenAiClient>(_ =>
            string.IsNullOrWhiteSpace(openAiApiKey)
                ? new OpenAiNullClient()
                : new OpenAiAdapter());

        services.AddSingleton<ICostingEngine, FifoCostingEngine>();
        services.AddSingleton<ICostingEngine, LifoCostingEngine>();
        services.AddSingleton<ICostingEngine, WeightedAverageCostingEngine>();
        services.AddSingleton<CostingService>();

        return services;
    }
}
