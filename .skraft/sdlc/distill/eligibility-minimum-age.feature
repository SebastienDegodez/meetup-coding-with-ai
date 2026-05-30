Feature: Eligibility Minimum Age Check

  Background:
    Given the eligibility service is operational

  @eligibility @happy-path @smoke
  Scenario: Driver aged exactly 21 with a Car is declared eligible
    Given a driver aged 21 with a valid B licence with 3 years of experience
    When the driver requests an eligibility check for a Car
    Then the driver is declared eligible

  @eligibility @error-case @smoke
  Scenario: Driver aged 20 with a Car is refused for being too young
    Given a driver aged 20 with a valid B licence with 2 years of experience
    When the driver requests an eligibility check for a Car
    Then the driver is declared not eligible
    And the rejection reason is "Conducteur trop jeune pour ce véhicule"

  @eligibility @edge-case
  Scenario: Driver aged 20 with a Motorcycle is refused for being too young
    Given a driver aged 20 with a valid A licence with 1 year of experience
    When the driver requests an eligibility check for a Motorcycle with 50hp
    Then the driver is declared not eligible
    And the rejection reason is "Conducteur trop jeune pour ce véhicule"

  @eligibility @edge-case
  Scenario: Driver aged 18 previously eligible under old law is now refused
    Given a driver aged 18 with a valid B licence with 0 years of experience
    When the driver requests an eligibility check for a Car
    Then the driver is declared not eligible
    And the rejection reason is "Conducteur trop jeune pour ce véhicule"

  @eligibility @edge-case @smoke
  Scenario: Driver aged 16 with an Electric Scooter is declared eligible
    Given a driver aged 16 with 0 years of experience
    When the driver requests an eligibility check for an Electric Scooter
    Then the driver is declared eligible

  @eligibility @edge-case
  Scenario Outline: Minimum age boundary for standard vehicles
    Given a driver aged <age> with a valid licence with 3 years of experience
    When the driver requests an eligibility check for a <vehicle>
    Then the driver is <result>

    Examples:
      | age | vehicle    | result           |
      | 20  | Car        | declared not eligible |
      | 21  | Car        | declared eligible    |
      | 20  | Motorcycle | declared not eligible |
      | 21  | Motorcycle | declared eligible    |
