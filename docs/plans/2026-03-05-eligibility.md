# Insurance Eligibility Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Implement insurance subscription eligibility check — refuse drivers who are too young or lack experience for the requested vehicle type.

**Architecture:** Query-only feature (no state mutation). `EligibilityPolicy` domain service encodes all rules. `CheckEligibilityQueryHandler` in Application delegates to the domain, no Infrastructure port needed. API exposes `POST /eligibility`.

**Tech Stack:** .NET 10, xUnit, FakeItEasy, custom CQRS bus (`IQueryBus`), Clean Architecture 4-layer.

---

## Business Rules (from Gherkin)

| # | Rule | Rejection reason |
|---|------|-----------------|
| 1 | Car or Motorcycle requires age ≥ 18 | "Driver too young for this vehicle" |
| 2 | Electric scooter allowed from age 16 (no 18 minimum) | — |
| 3 | Motorcycle > 100 hp requires license ≥ 5 years | "Insufficient experience for the power" |

---

## Files

**Create:**
- `src/MonAssurance.Domain/Eligibility/VehicleType.cs`
- `src/MonAssurance.Domain/Eligibility/DriverInfo.cs`
- `src/MonAssurance.Domain/Eligibility/VehicleInfo.cs`
- `src/MonAssurance.Domain/Eligibility/EligibilityResult.cs`
- `src/MonAssurance.Domain/Eligibility/EligibilityRejectionReasons.cs`
- `src/MonAssurance.Domain/Eligibility/EligibilityPolicy.cs`
- `src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQuery.cs`
- `src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQueryResult.cs`
- `src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQueryHandler.cs`
- `tests/MonAssurance.UnitTests/Application/Eligibility/Queries/CheckEligibilityQueryHandlerTests.cs`
- `tests/MonAssurance.UnitTests/Domain/Eligibility/EligibilityPolicyTests.cs`

**Modify:**
- `src/MonAssurance.Api/Program.cs` — add `POST /eligibility` endpoint + `AddApplicationHandlers()`
- `.github/instructions/business-lexicon.instructions.md` — add new terms

---

## Task 1: [RED] Application handler tests — all 3 scenarios

> Write the 3 failing tests for `CheckEligibilityQueryHandler`. Stub to compile, then confirm behavior failures.

**Files:**
- Create: `tests/MonAssurance.UnitTests/Application/Eligibility/Queries/CheckEligibilityQueryHandlerTests.cs`

**Step 1: Create the test file**

```csharp
using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Application.Eligibility.Queries;

public sealed class CheckEligibilityQueryHandlerTests
{
    // Scenario 1: Gherkin — born 10/10/2010, today 01/01/2026 = age 15; vehicle = Car
    // Age computed: 2026-01-01 - 2010-10-10 = 15 years (birthday not yet reached in 2026)
    [Fact]
    public async Task WhenDriverIsMinorAndVehicleIsCar_ShouldRefuseWithDriverTooYoungReason()
    {
        var handler = new CheckEligibilityQueryHandler();
        var query = new CheckEligibilityQuery
        {
            DriverAge = 15,
            DriverLicenseYears = 0,
            VehicleType = VehicleType.Car
        };

        var result = await handler.HandleAsync(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Driver too young for this vehicle", result.RejectionReason);
    }

    // Scenario 2: driver age 16, vehicle = ElectricScooter → accepted
    [Fact]
    public async Task WhenDriverIs16AndVehicleIsElectricScooter_ShouldAccept()
    {
        var handler = new CheckEligibilityQueryHandler();
        var query = new CheckEligibilityQuery
        {
            DriverAge = 16,
            DriverLicenseYears = 0,
            VehicleType = VehicleType.ElectricScooter
        };

        var result = await handler.HandleAsync(query);

        Assert.True(result.IsEligible);
        Assert.Null(result.RejectionReason);
    }

    // Scenario 3: driver with 2 years license, motorcycle 120 hp → refused
    [Fact]
    public async Task WhenDriverHas2YearsLicenseAndMotorcycleIsOver100Hp_ShouldRefuseWithInsufficientExperienceReason()
    {
        var handler = new CheckEligibilityQueryHandler();
        var query = new CheckEligibilityQuery
        {
            DriverAge = 25,
            DriverLicenseYears = 2,
            VehicleType = VehicleType.Motorcycle,
            VehicleHorsepower = 120
        };

        var result = await handler.HandleAsync(query);

        Assert.False(result.IsEligible);
        Assert.Equal("Insufficient experience for the power", result.RejectionReason);
    }
}
```

**Step 2: Add stubs to make it compile (wishful thinking phase)**

Types used in the test that don't exist yet — create minimal stubs:

```csharp
// src/MonAssurance.Domain/Eligibility/VehicleType.cs
namespace MonAssurance.Domain.Eligibility;
public enum VehicleType { Car, ElectricScooter, Motorcycle }
```

```csharp
// src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQuery.cs
using MonAssurance.Domain.Eligibility;
namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;
public sealed class CheckEligibilityQuery
{
    public int DriverAge { get; init; }
    public int DriverLicenseYears { get; init; }
    public VehicleType VehicleType { get; init; }
    public int? VehicleHorsepower { get; init; }
}
```

```csharp
// src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQueryResult.cs
namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;
public sealed class CheckEligibilityQueryResult
{
    public bool IsEligible { get; init; }
    public string? RejectionReason { get; init; }
}
```

```csharp
// src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQueryHandler.cs
using MonAssurance.Application.Shared;
namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;
public sealed class CheckEligibilityQueryHandler : IQueryHandler<CheckEligibilityQuery, CheckEligibilityQueryResult>
{
    public Task<CheckEligibilityQueryResult> HandleAsync(CheckEligibilityQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(new CheckEligibilityQueryResult());  // stub — returns default, all tests fail on behavior
}
```

**Step 3: Run — confirm compilation succeeds and tests fail on behavior (not compilation)**

```bash
cd /path/to/workspace
dotnet test tests/MonAssurance.UnitTests/ --filter "CheckEligibilityQueryHandlerTests" -v minimal
```

Expected output: **3 FAILED** — `Assert.False(True)` / `Assert.Equal("Driver too young…", null)` — behavior failures, not compilation errors. This is **RED** ✓

---

> **⚠️ MANDATORY PAUSE — Developer validates the 3 tests before proceeding.**
> Confirm: Do the 3 tests correctly capture the intended business behavior?
> Only proceed to Task 2 after explicit confirmation.

---

## Task 2: [SYNTHESIZE GREEN] Domain model + Application handler

> Implement all domain types and the handler. Tests must turn green.

**Files:**
- Create: `src/MonAssurance.Domain/Eligibility/DriverInfo.cs`
- Create: `src/MonAssurance.Domain/Eligibility/VehicleInfo.cs`
- Create: `src/MonAssurance.Domain/Eligibility/EligibilityResult.cs`
- Create: `src/MonAssurance.Domain/Eligibility/EligibilityRejectionReasons.cs`
- Create: `src/MonAssurance.Domain/Eligibility/EligibilityPolicy.cs`
- Replace stubs: `CheckEligibilityQueryHandler.cs`

**Step 1: Create Domain value objects**

```csharp
// src/MonAssurance.Domain/Eligibility/DriverInfo.cs
namespace MonAssurance.Domain.Eligibility;

public sealed class DriverInfo
{
    public int Age { get; init; }
    public int LicenseYears { get; init; }
}
```

```csharp
// src/MonAssurance.Domain/Eligibility/VehicleInfo.cs
namespace MonAssurance.Domain.Eligibility;

public sealed class VehicleInfo
{
    public VehicleType Type { get; init; }
    public int? Horsepower { get; init; }
}
```

```csharp
// src/MonAssurance.Domain/Eligibility/EligibilityResult.cs
namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityResult
{
    public bool IsEligible { get; init; }
    public string? RejectionReason { get; init; }

    public static EligibilityResult Accepted() => new() { IsEligible = true };
    public static EligibilityResult Rejected(string reason) => new() { IsEligible = false, RejectionReason = reason };
}
```

```csharp
// src/MonAssurance.Domain/Eligibility/EligibilityRejectionReasons.cs
namespace MonAssurance.Domain.Eligibility;

public static class EligibilityRejectionReasons
{
    public const string DriverTooYoungForVehicle = "Driver too young for this vehicle";
    public const string InsufficientExperienceForPower = "Insufficient experience for the power";
}
```

**Step 2: Create EligibilityPolicy domain service**

```csharp
// src/MonAssurance.Domain/Eligibility/EligibilityPolicy.cs
namespace MonAssurance.Domain.Eligibility;

public sealed class EligibilityPolicy
{
    private const int MinimumAgeForCarOrMotorcycle = 18;
    private const int MinimumAgeForElectricScooter = 16;
    private const int MinimumLicenseYearsForPowerfulMotorcycle = 5;
    private const int PowerfulMotorcycleHorsepowerThreshold = 100;

    public EligibilityResult Evaluate(DriverInfo driver, VehicleInfo vehicle)
    {
        if (vehicle.Type is VehicleType.Car or VehicleType.Motorcycle
            && driver.Age < MinimumAgeForCarOrMotorcycle)
            return EligibilityResult.Rejected(EligibilityRejectionReasons.DriverTooYoungForVehicle);

        if (vehicle.Type == VehicleType.ElectricScooter
            && driver.Age < MinimumAgeForElectricScooter)
            return EligibilityResult.Rejected(EligibilityRejectionReasons.DriverTooYoungForVehicle);

        if (vehicle.Type == VehicleType.Motorcycle
            && vehicle.Horsepower > PowerfulMotorcycleHorsepowerThreshold
            && driver.LicenseYears < MinimumLicenseYearsForPowerfulMotorcycle)
            return EligibilityResult.Rejected(EligibilityRejectionReasons.InsufficientExperienceForPower);

        return EligibilityResult.Accepted();
    }
}
```

**Step 3: Replace the stub handler with the real implementation**

```csharp
// src/MonAssurance.Application/Eligibility/Queries/CheckEligibility/CheckEligibilityQueryHandler.cs
using MonAssurance.Application.Shared;
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.Application.Eligibility.Queries.CheckEligibility;

public sealed class CheckEligibilityQueryHandler : IQueryHandler<CheckEligibilityQuery, CheckEligibilityQueryResult>
{
    private readonly EligibilityPolicy _policy = new();

    public Task<CheckEligibilityQueryResult> HandleAsync(
        CheckEligibilityQuery query,
        CancellationToken cancellationToken = default)
    {
        var driver = new DriverInfo { Age = query.DriverAge, LicenseYears = query.DriverLicenseYears };
        var vehicle = new VehicleInfo { Type = query.VehicleType, Horsepower = query.VehicleHorsepower };

        var result = _policy.Evaluate(driver, vehicle);

        return Task.FromResult(new CheckEligibilityQueryResult
        {
            IsEligible = result.IsEligible,
            RejectionReason = result.RejectionReason
        });
    }
}
```

**Step 4: Run tests — confirm GREEN**

```bash
dotnet test tests/MonAssurance.UnitTests/ --filter "CheckEligibilityQueryHandlerTests" -v minimal
```

Expected: **3 PASSED**

**Step 5: Commit**

```bash
git add src/MonAssurance.Domain/Eligibility/ \
        src/MonAssurance.Application/Eligibility/ \
        tests/MonAssurance.UnitTests/Application/Eligibility/
git commit -m "feat(eligibility): add CheckEligibilityQueryHandler with domain policy"
```

---

## Task 3: [RED → GREEN] Domain EligibilityPolicy unit tests

> Drive direct domain rule coverage: edge cases the handler tests don't cover (boundary values, 16yo motorcycle, etc.).

**Files:**
- Create: `tests/MonAssurance.UnitTests/Domain/Eligibility/EligibilityPolicyTests.cs`

**Step 1: Write the domain tests**

```csharp
using MonAssurance.Domain.Eligibility;

namespace MonAssurance.UnitTests.Domain.Eligibility;

public sealed class EligibilityPolicyTests
{
    private readonly EligibilityPolicy _policy = new();

    // Rule 1 boundary — age 17 (just below minimum)
    [Fact]
    public void WhenDriverIs17AndVehicleIsCar_ShouldReject()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 17, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.Car });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.DriverTooYoungForVehicle, result.RejectionReason);
    }

    // Rule 1 boundary — age 18 (meets minimum)
    [Fact]
    public void WhenDriverIs18AndVehicleIsCar_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 18, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.Car });

        Assert.True(result.IsEligible);
    }

    // Rule 1 — motorcycle also requires 18+
    [Fact]
    public void WhenDriverIs17AndVehicleIsMotorcycle_ShouldReject()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 17, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 50 });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.DriverTooYoungForVehicle, result.RejectionReason);
    }

    // Rule 2 — electric scooter allowed at 16
    [Fact]
    public void WhenDriverIs16AndVehicleIsElectricScooter_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 16, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.ElectricScooter });

        Assert.True(result.IsEligible);
    }

    // Rule 2 boundary — electric scooter refused at 15
    [Fact]
    public void WhenDriverIs15AndVehicleIsElectricScooter_ShouldReject()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 15, LicenseYears = 0 },
            new VehicleInfo { Type = VehicleType.ElectricScooter });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.DriverTooYoungForVehicle, result.RejectionReason);
    }

    // Rule 3 boundary — exactly 100 hp (not over threshold) → no experience requirement
    [Fact]
    public void WhenMotorcycleIsExactly100HpAndDriverHas2YearsLicense_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 25, LicenseYears = 2 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 100 });

        Assert.True(result.IsEligible);
    }

    // Rule 3 boundary — 101 hp with 4 years → refused
    [Fact]
    public void WhenMotorcycleIs101HpAndDriverHas4YearsLicense_ShouldReject()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 25, LicenseYears = 4 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 101 });

        Assert.False(result.IsEligible);
        Assert.Equal(EligibilityRejectionReasons.InsufficientExperienceForPower, result.RejectionReason);
    }

    // Rule 3 boundary — 101 hp with exactly 5 years → accepted
    [Fact]
    public void WhenMotorcycleIs101HpAndDriverHas5YearsLicense_ShouldAccept()
    {
        var result = _policy.Evaluate(
            new DriverInfo { Age = 25, LicenseYears = 5 },
            new VehicleInfo { Type = VehicleType.Motorcycle, Horsepower = 101 });

        Assert.True(result.IsEligible);
    }
}
```

**Step 2: Run — confirm RED first (before implementing policy was already green from Task 2)**

```bash
dotnet test tests/MonAssurance.UnitTests/ --filter "EligibilityPolicyTests" -v minimal
```

Expected: all tests pass immediately (domain policy was already fully implemented in Task 2). If any fail, fix `EligibilityPolicy.cs`.

**Step 3: Commit**

```bash
git add tests/MonAssurance.UnitTests/Domain/Eligibility/
git commit -m "test(eligibility): add EligibilityPolicy boundary unit tests"
```

---

## Task 4: [Implement] API endpoint + DI wiring

> Expose `POST /eligibility` via the CQRS query bus. Wire all handlers with `AddApplicationHandlers()`.

**Files:**
- Modify: `src/MonAssurance.Api/Program.cs`

**Step 1: Add a request record for the endpoint (inline in Program.cs)**

No separate file needed — use a local `record` in Program.cs.

**Step 2: Update Program.cs**

Add `AddApplicationHandlers()` in the service registration section, and add the endpoint.

Before `var app = builder.Build();`, add:
```csharp
builder.Services.AddApplicationHandlers();
```

After `app.UseHttpsRedirection();`, add:
```csharp
app.MapPost("/eligibility", async (
    CheckEligibilityRequest request,
    IQueryBus queryBus,
    CancellationToken cancellationToken) =>
{
    var query = new CheckEligibilityQuery
    {
        DriverAge = request.DriverAge,
        DriverLicenseYears = request.DriverLicenseYears,
        VehicleType = request.VehicleType,
        VehicleHorsepower = request.VehicleHorsepower
    };

    var result = await queryBus.SendAsync<CheckEligibilityQuery, CheckEligibilityQueryResult>(
        query, cancellationToken);

    return result.IsEligible
        ? Results.Ok(result)
        : Results.UnprocessableEntity(result);
})
.WithName("CheckEligibility")
.Produces<CheckEligibilityQueryResult>(StatusCodes.Status200OK)
.Produces<CheckEligibilityQueryResult>(StatusCodes.Status422UnprocessableEntity);
```

Add the required using/record at the top of Program.cs:
```csharp
using MonAssurance.Application.Eligibility.Queries.CheckEligibility;
using MonAssurance.Application.Shared;
using MonAssurance.Domain.Eligibility;
```

And define the request record (file-scoped, before `app.Run();`):
```csharp
public record CheckEligibilityRequest(
    int DriverAge,
    int DriverLicenseYears,
    VehicleType VehicleType,
    int? VehicleHorsepower);
```

**Step 3: Build and run to verify no errors**

```bash
dotnet build src/MonAssurance.Api/
```

Expected: **Build succeeded**

**Step 4: Smoke test with curl**

```bash
# Start the API
dotnet run --project src/MonAssurance.Api/ &

# Scenario 1 — minor with car → 422
curl -s -X POST http://localhost:5000/eligibility \
  -H "Content-Type: application/json" \
  -d '{"driverAge":15,"driverLicenseYears":0,"vehicleType":"Car","vehicleHorsepower":null}' | jq .

# Scenario 2 — 16yo + electric scooter → 200
curl -s -X POST http://localhost:5000/eligibility \
  -H "Content-Type: application/json" \
  -d '{"driverAge":16,"driverLicenseYears":0,"vehicleType":"ElectricScooter","vehicleHorsepower":null}' | jq .

# Scenario 3 — 2yr license + 120hp moto → 422
curl -s -X POST http://localhost:5000/eligibility \
  -H "Content-Type: application/json" \
  -d '{"driverAge":25,"driverLicenseYears":2,"vehicleType":"Motorcycle","vehicleHorsepower":120}' | jq .
```

**Step 5: Run all tests**

```bash
dotnet test --verbosity minimal
```

Expected: all existing tests (architecture, CQRS integration, unit) pass.

**Step 6: Commit**

```bash
git add src/MonAssurance.Api/Program.cs
git commit -m "feat(eligibility): expose POST /eligibility endpoint via CQRS query bus"
```

---

## Task 5: [Update] Business Lexicon

> Add all new French→English domain terms introduced by this feature.

**Files:**
- Modify: `.github/instructions/business-lexicon.instructions.md`

**Step 1: Add new terms to the correspondence table**

| Français | English | Notes |
|---|---|---|
| souscription | subscription | Insurance contract entry |
| voiture | car | Vehicle type |
| trottinette électrique | electric scooter | Vehicle type |
| moto | motorcycle | Vehicle type |
| conducteur trop jeune pour ce véhicule | driver too young for this vehicle | Refusal reason |
| expérience insuffisante pour la puissance | insufficient experience for the power | Refusal reason |
| motif de refus | rejection reason | Business term |

**Step 2: Commit**

```bash
git add .github/instructions/business-lexicon.instructions.md
git commit -m "docs(lexicon): add eligibility feature FR->EN business terms"
```

---

## Final Verification

Run the full test suite:

```bash
dotnet test --verbosity minimal
```

Expected output: all tests pass — unit (handler + domain), integration (clean architecture + CQRS wiring).
