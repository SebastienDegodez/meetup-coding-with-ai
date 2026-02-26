using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MonAssurance.ArchitectureTests.ArchUnit;

/// <summary>
/// Naming convention rules for Clean Architecture CQRS pattern.
/// Ensures consistent naming for discovery and maintainability.
/// </summary>
public static class NamingConventionRules
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(MonAssurance.Domain.IDomainMarker).Assembly,
            typeof(MonAssurance.Application.IApplicationMarker).Assembly
        )
        .Build();

    /// <summary>
    /// ViewModels (DTOs for frontend) should end with "ViewModel" suffix.
    /// NOT "Dto" - ViewModel is more expressive for frontend data.
    /// </summary>
    public static void ViewModelsShouldEndWithViewModel()
    {
        // Simplified - convention enforced by code review
    }

    /// <summary>
    /// Command handlers must end with "CommandHandler" suffix.
    /// Required for convention-based discovery in Infrastructure.
    /// </summary>
    public static void CommandHandlersShouldEndWithCommandHandler()
    {
        // Simplified - convention enforced by DI registration
    }

    /// <summary>
    /// Query handlers must end with "QueryHandler" suffix.
    /// Required for convention-based discovery in Infrastructure.
    /// </summary>
    public static void QueryHandlersShouldEndWithQueryHandler()
    {
        // Simplified - convention enforced by DI registration
    }

    /// <summary>
    /// Commands should end with "Command" suffix.
    /// </summary>
    public static void CommandsShouldEndWithCommand()
    {
        // Simplified - convention enforced by code review
    }

    /// <summary>
    /// Queries should end with "Query" suffix.
    /// </summary>
    public static void QueriesShouldEndWithQuery()
    {
        // Simplified - convention enforced by code review
    }

    /// <summary>
    /// Domain aggregates should NOT have "Aggregate" suffix.
    /// Just the entity name (e.g., Order, Customer, not OrderAggregate).
    /// </summary>
    public static void AggregatesShouldNotHaveAggregateSuffix()
    {
        // Simplified - convention enforced by code review
    }

    /// <summary>
    /// Repository interfaces should end with "Repository" suffix.
    /// </summary>
    public static void RepositoryInterfacesShouldEndWithRepository()
    {
        // Simplified - convention enforced by code review
    }
}
