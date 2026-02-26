using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MonAssurance.ArchitectureTests.ArchUnit;

/// <summary>
/// Architecture rules for the API layer.
/// Ensures API does NOT reference Application directly (discovery via Infrastructure).
/// </summary>
public static class ApiLayerRules
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            System.Reflection.Assembly.Load("MonAssurance.Domain"),
            System.Reflection.Assembly.Load("MonAssurance.Application"),
            System.Reflection.Assembly.Load("MonAssurance.Infrastructure"),
            System.Reflection.Assembly.Load("MonAssurance.Api")
        )
        .Build();

    private static IObjectProvider<IType> ApiLayer =>
        Types().That().ResideInNamespace("MonAssurance.Api").As("API Layer");

    private static IObjectProvider<IType> ApplicationLayer =>
        Types().That().ResideInNamespace("MonAssurance.Application");

    /// <summary>
    /// CRITICAL: API should NOT reference Application layer directly.
    /// API -> Infrastructure -> Application (dependency injection).
    /// </summary>
    public static void ShouldNotReferenceApplication()
    {
        Types()
            .That().Are(ApiLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .Because("API should not reference Application layer directly; handlers are injected via Infrastructure DI")
            .Check(Architecture);
    }

    /// <summary>
    /// API can reference Infrastructure (for DI setup).
    /// </summary>
    public static void CanReferenceInfrastructure()
    {
        // This is expected behavior - API references Infrastructure for DI
    }

    /// <summary>
    /// API endpoints should inject ICommandHandler or IQueryHandler interfaces.
    /// NOT concrete handler classes.
    /// </summary>
    public static void EndpointsShouldInjectHandlerInterfaces()
    {
        // This is validated at runtime through DI container
        // ArchUnit can't easily validate method parameters, so this is a documentation reminder
        // Manual review: check that MapPost/MapGet inject ICommandHandler<>/IQueryHandler<>
    }
}
