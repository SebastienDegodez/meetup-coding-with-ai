using Xunit;

namespace MonAssurance.ArchitectureTests;

public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        ArchUnit.DomainLayerRules.ShouldNotDependOnOtherLayers();
    }

    [Fact]
    public void Application_ShouldOnlyDependOnDomain()
    {
        ArchUnit.ApplicationLayerRules.ShouldOnlyDependOnDomain();
    }

    [Fact]
    public void Api_ShouldNotReferenceApplication()
    {
        ArchUnit.ApiLayerRules.ShouldNotReferenceApplication();
    }

    [Fact]
    public void Infrastructure_CanReferenceDomainAndApplication()
    {
        ArchUnit.InfrastructureLayerRules.CanReferenceDomainAndApplication();
    }

    [Fact]
    public void ViewModels_ShouldEndWithViewModel()
    {
        ArchUnit.NamingConventionRules.ViewModelsShouldEndWithViewModel();
    }

    [Fact]
    public void CommandHandlers_ShouldEndWithCommandHandler()
    {
        ArchUnit.NamingConventionRules.CommandHandlersShouldEndWithCommandHandler();
    }

    [Fact]
    public void QueryHandlers_ShouldEndWithQueryHandler()
    {
        ArchUnit.NamingConventionRules.QueryHandlersShouldEndWithQueryHandler();
    }
}
