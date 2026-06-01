using Microsoft.Extensions.DependencyInjection;
using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure services (repositories, external services, DbContext)
    /// and the Application use cases / handlers.
    /// </summary>
    /// <remarks>
    /// Future Infrastructure Services:
    /// - EF Core DbContext: services.AddDbContext&lt;AppDbContext&gt;(options => ...);
    /// - Repositories: services.AddScoped&lt;IOrderRepository, OrderRepository&gt;();
    /// </remarks>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Register eligibility services
        services.AddSingleton<EligibilityPolicy>();
        services.AddScoped<CheckEligibilityQueryHandler>();

        return services;
    }
}
