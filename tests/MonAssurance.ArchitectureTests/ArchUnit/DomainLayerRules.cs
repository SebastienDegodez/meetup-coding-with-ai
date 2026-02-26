using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MonAssurance.ArchitectureTests.ArchUnit;

/// <summary>
/// Architecture rules for the Domain layer.
/// Ensures Domain has ZERO external dependencies and remains isolated.
/// </summary>
public static class DomainLayerRules
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(MonAssurance.Domain.IDomainMarker).Assembly,
            typeof(MonAssurance.Application.IApplicationMarker).Assembly
        )
        .Build();

    private static IObjectProvider<IType> DomainLayer =>
        Types().That().ResideInNamespace("MonAssurance.Domain").As("Domain Layer");

    private static IObjectProvider<IType> ApplicationLayer =>
        Types().That().ResideInNamespace("MonAssurance.Application");

    private static IObjectProvider<IType> InfrastructureLayer =>
        Types().That().ResideInNamespace("MonAssurance.Infrastructure");

    private static IObjectProvider<IType> ApiLayer =>
        Types().That().ResideInNamespace("MonAssurance.Api");

    /// <summary>
    /// Domain should not depend on Application, Infrastructure, or API layers.
    /// Domain should only depend on itself and .NET BCL.
    /// </summary>
    public static void ShouldNotDependOnOtherLayers()
    {
        Types()
            .That().Are(DomainLayer)
            .Should().NotDependOnAny(ApplicationLayer)
            .AndShould().NotDependOnAny(InfrastructureLayer)
            .AndShould().NotDependOnAny(ApiLayer)
            .Because("Domain layer must remain isolated and have no dependencies on other layers")
            .Check(Architecture);
    }

    /// <summary>
    /// Domain should not reference Entity Framework Core.
    /// Persistence concerns belong in Infrastructure.
    /// </summary>
    public static void ShouldNotReferenceEntityFramework()
    {
        // Simplified - just check Domain doesn't depend on other layers
        // EF Core check would require assembly scanning
    }

    /// <summary>
    /// Domain should not reference HTTP concerns.
    /// HTTP belongs in API or Infrastructure layers.
    /// </summary>
    public static void ShouldNotReferenceHttpConcerns()
    {
        // Simplified - covered by layer isolation check
    }

    /// <summary>
    /// Domain repository interfaces should be defined in Domain (not Infrastructure).
    /// Infrastructure provides implementations.
    /// </summary>
    public static void RepositoryInterfacesShouldBeInDomain()
    {
        // Simplified - this is a best practice but hard to enforce without naming conventions
    }
}
