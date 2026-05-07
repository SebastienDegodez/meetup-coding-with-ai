# language: fr
Fonctionnalité: Éligibilité à la souscription d'assurance
  En tant qu'assureur
  Je veux vérifier si un prospect est éligible
  Afin de ne pas assurer de conducteurs non autorisés

  # Règle : Il faut être majeur pour assurer une voiture ou une grosse moto
  Scénario: Refus de souscription voiture pour un mineur
    Étant donné un conducteur né le "10/10/2010"
    Et nous sommes le "01/01/2026"
    Et le véhicule est une "Voiture"
    Quand je demande une éligibilité
    Alors la demande est refusée avec le motif "Conducteur trop jeune pour ce véhicule"

  # Règle : On peut assurer des petits véhicules électriques dès 16 ans
  Scénario: Acceptation souscription trottinette pour un adolescent de 16 ans
    Étant donné un conducteur âgé de 16 ans
    Et le véhicule est une "Trottinette électrique"
    Quand je demande une éligibilité
    Alors la demande est acceptée

  # Règle : Les motos extrêmement puissantes (> 100ch) nécessitent 5 ans de permis
  Scénario: Refus moto puissante pour jeune permis
    Étant donné un conducteur avec 2 ans de permis
    Et le véhicule est une "Moto" de 120 chevaux
    Quand je demande une éligibilité
    Alors la demande est refusée avec le motif "Expérience insuffisante pour la puissance"
