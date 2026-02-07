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

namespace Hpp.Infrastructure;

/// <summary>
/// Service registration for HPP.
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddHppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HppDbContext>(options =>
        {
            var conn = configuration.GetConnectionString("HppDb") ?? "TODO:__DEV_DB_CONN__";
            options.UseSqlServer(conn);
        });

        services.AddSingleton<ILogger>(_ => SerilogConfigurator.Configure(configuration));
        services.AddAutoMapper(typeof(HppMappingProfile));

        services.AddScoped<IInventoryRepository, EfInventoryRepository>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISeedService, SeedService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IScenarioSimulator, ScenarioSimulatorService>();

        services.AddSingleton<IInvoiceParser, InvoiceParserMock>();
        services.AddSingleton<IInvoiceIngestor, InvoiceIngestor>();
        services.AddSingleton<IOpenAiClient, OpenAiNullClient>();

        services.AddSingleton<ICostingEngine, FifoCostingEngine>();
        services.AddSingleton<ICostingEngine, LifoCostingEngine>();
        services.AddSingleton<ICostingEngine, WeightedAverageCostingEngine>();
        services.AddSingleton<CostingService>();

        return services;
    }
}
