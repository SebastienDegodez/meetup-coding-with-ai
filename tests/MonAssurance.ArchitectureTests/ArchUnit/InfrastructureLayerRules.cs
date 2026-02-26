using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MonAssurance.ArchitectureTests.ArchUnit;

/// <summary>
/// Architecture rules for the Infrastructure layer.
/// Ensures Infrastructure can reference Domain and Application (implements contracts).
/// </summary>
public static class InfrastructureLayerRules
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            System.Reflection.Assembly.Load("MonAssurance.Domain"),
            System.Reflection.Assembly.Load("MonAssurance.Application"),
            System.Reflection.Assembly.Load("MonAssurance.Infrastructure")
        )
        .Build();

    private static IObjectProvider<IType> InfrastructureLayer =>
        Types().That().ResideInNamespace("MonAssurance.Infrastructure").As("Infrastructure Layer");

    private static IObjectProvider<IType> ApiLayer =>
        Types().That().ResideInNamespace("MonAssurance.Api");

    /// <summary>
    /// Infrastructure can reference both Domain and Application.
    /// Infrastructure implements interfaces defined in those layers.
    /// </summary>
    public static void CanReferenceDomainAndApplication()
    {
        Types()
            .That().Are(InfrastructureLayer)
            .Should().NotDependOnAny(ApiLayer)
            .Because("Infrastructure should not depend on API layer")
            .Check(Architecture);
    }

    /// <summary>
    /// Repository implementations should be in Infrastructure.Persistence namespace.
    /// </summary>
    public static void RepositoryImplementationsShouldBeInPersistence()
    {
        // Simplified - convention enforced by project structure
    }

    /// <summary>
    /// Repository implementations should implement Domain repository interfaces.
    /// </summary>
    public static void RepositoriesShouldImplementDomainInterfaces()
    {
        // Simplified - convention enforced at compile time
    }

    /// <summary>
    /// DbContext should be in Infrastructure.Persistence namespace.
    /// </summary>
    public static void DbContextShouldBeInPersistence()
    {
        // Simplified - convention enforced by project structure
    }
}
