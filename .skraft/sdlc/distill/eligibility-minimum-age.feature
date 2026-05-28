Feature: Eligibility — Minimum Age Enforcement

  The law requires drivers to be at least 21 years old to insure a car or motorcycle.
  Drivers applying for an electric scooter are subject to the existing minimum age of 16.
  A driver who does not meet the minimum age is refused with the reason "Conducteur trop jeune pour ce véhicule".

  Background:
    Given the eligibility service is operational

  # ─── Happy path ────────────────────────────────────────────────────────────────

  @eligibility @happy-path @smoke
  Scenario: Driver aged 21 applying for a car is accepted
    Given a driver aged 21 with a valid B licence
    And a vehicle of type Car
    When the driver requests an eligibility check
    Then the eligibility is accepted

  @eligibility @happy-path @smoke
  Scenario: Driver aged 16 applying for an electric scooter is accepted
    Given a driver aged 16 with a valid AM licence
    And a vehicle of type ElectricScooter
    When the driver requests an eligibility check
    Then the eligibility is accepted

  # ─── Edge cases — boundary values ──────────────────────────────────────────────

  @eligibility @edge-case @smoke
  Scenario: Driver aged 20 applying for a car is refused
    Given a driver aged 20 with a valid B licence
    And a vehicle of type Car
    When the driver requests an eligibility check
    Then the eligibility is refused
    And the rejection reason is "Conducteur trop jeune pour ce véhicule"

  @eligibility @edge-case
  Scenario: Driver aged 20 applying for a motorcycle is refused
    Given a driver aged 20 with a valid A licence and 1 year of experience
    And a vehicle of type Motorcycle with power not exceeding 100 hp
    When the driver requests an eligibility check
    Then the eligibility is refused
    And the rejection reason is "Conducteur trop jeune pour ce véhicule"

  @eligibility @edge-case
  Scenario: Driver aged 18 applying for a car is refused
    Given a driver aged 18 with a valid B licence
    And a vehicle of type Car
    When the driver requests an eligibility check
    Then the eligibility is refused
    And the rejection reason is "Conducteur trop jeune pour ce véhicule"

  # ─── Parametrised boundary — minimum age per vehicle type ───────────────────────

  @eligibility @edge-case
  Scenario Outline: Driver eligibility varies by age and vehicle type
    Given a driver aged <age> with a valid <licence> licence
    And a vehicle of type <vehicleType>
    When the driver requests an eligibility check
    Then the eligibility is <result>

    Examples:
      | age | licence | vehicleType    | result   |
      | 20  | B       | Car            | refused  |
      | 21  | B       | Car            | accepted |
      | 20  | A       | Motorcycle     | refused  |
      | 21  | A       | Motorcycle     | accepted |
      | 15  | AM      | ElectricScooter| refused  |
      | 16  | AM      | ElectricScooter| accepted |
