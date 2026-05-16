using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MonAssurance.Domain.Eligibility;
using Xunit;

namespace MonAssurance.IntegrationTests.Eligibility;

public class EligibilityEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EligibilityEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostEligibility_WhenDriverIs25AndHasCar_Returns200AndIsEligibleTrue()
    {
        var payload = new
        {
            dateOfBirth = new DateOnly(2001, 1, 1).ToString("yyyy-MM-dd"),
            vehicleType = VehicleType.Car,
            power = (int?)null,
            licenseYears = 5
        };

        var response = await _client.PostAsJsonAsync("/eligibility", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<EligibilityResponse>();
        Assert.NotNull(result);
        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    private sealed record EligibilityResponse(bool IsEligible, string? RejectionReason);
}
