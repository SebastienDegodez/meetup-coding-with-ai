Feature: Vérification d'éligibilité — Âge légal minimum de conduite

  Le système d'éligibilité applique le nouvel âge légal minimum de 21 ans pour les véhicules
  non-trottinette. La règle de 16 ans pour la trottinette électrique reste inchangée.

  Background:
    Given le service d'éligibilité est opérationnel

  @eligibility @happy-path @smoke
  Scenario: Conducteur de 21 ans avec voiture est déclaré éligible
    Given un conducteur âgé de 21 ans avec un permis B valide
    And un véhicule de type voiture
    When le conducteur soumet une demande de vérification d'éligibilité
    Then le conducteur est déclaré éligible

  @eligibility @edge-case @smoke
  Scenario: Conducteur de 21 ans exactement est accepté (borne inférieure inclusive)
    Given un conducteur dont la date de naissance tombe exactement aujourd'hui il y a 21 ans
    And un véhicule de type voiture
    When le conducteur soumet une demande de vérification d'éligibilité
    Then le conducteur est déclaré éligible

  @eligibility @error-case @smoke
  Scenario: Conducteur de 20 ans avec voiture est refusé
    Given un conducteur âgé de 20 ans avec un permis B valide
    And un véhicule de type voiture
    When le conducteur soumet une demande de vérification d'éligibilité
    Then le conducteur est déclaré non éligible
    And le motif de refus est "Conducteur trop jeune pour ce véhicule"

  @eligibility @edge-case
  Scenario: Conducteur de 18 ans avec voiture est refusé — ancienne limite devenue invalide
    Given un conducteur âgé de 18 ans avec un permis B valide
    And un véhicule de type voiture
    When le conducteur soumet une demande de vérification d'éligibilité
    Then le conducteur est déclaré non éligible
    And le motif de refus est "Conducteur trop jeune pour ce véhicule"

  @eligibility @happy-path
  Scenario: Conducteur de 16 ans avec trottinette électrique est déclaré éligible
    Given un conducteur âgé de 16 ans
    And un véhicule de type trottinette électrique
    When le conducteur soumet une demande de vérification d'éligibilité
    Then le conducteur est déclaré éligible
