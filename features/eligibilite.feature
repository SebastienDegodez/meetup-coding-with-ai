# language: fr
Fonctionnalité: Éligibilité à la souscription d'assurance
  En tant qu'assureur
  Je veux vérifier si un prospect est éligible
  Afin de ne pas assurer de conducteurs non autorisés

  Contexte:
    Étant donné nous sommes le "01/01/2026"

  # Règle : Il faut avoir au moins 21 ans pour assurer une voiture ou une moto
  Scénario: Refus de souscription voiture pour un conducteur trop jeune
    Étant donné un conducteur né le "10/10/2010"
    Et le véhicule est une "Voiture"
    Quand je demande une éligibilité
    Alors la demande est refusée avec le motif "Conducteur trop jeune pour ce véhicule"

  Plan du Scénario: Vérification de l'âge minimum selon le type de véhicule
    Étant donné un conducteur âgé de <age> ans
    Et le véhicule est une "<vehicule>"
    Quand je demande une éligibilité
    Alors la demande est <resultat>

    Exemples: Conducteurs trop jeunes refusés
      | age | vehicule | resultat                                                       |
      | 20  | Voiture  | refusée avec le motif "Conducteur trop jeune pour ce véhicule" |
      | 18  | Voiture  | refusée avec le motif "Conducteur trop jeune pour ce véhicule" |
      | 20  | Moto     | refusée avec le motif "Conducteur trop jeune pour ce véhicule" |

    Exemples: Conducteurs éligibles acceptés
      | age | vehicule | resultat |
      | 21  | Voiture  | acceptée |

  # Règle : On peut assurer des petits véhicules électriques dès 16 ans
  Plan du Scénario: Vérification de l'âge minimum pour les véhicules électriques
    Étant donné un conducteur âgé de <age> ans
    Et le véhicule est une "<vehicule>"
    Quand je demande une éligibilité
    Alors la demande est <resultat>

    Exemples: Véhicules électriques légers
      | age | vehicule              | resultat                                                       |
      | 16  | Trottinette électrique | acceptée                                                       |
      | 15  | Trottinette électrique | refusée avec le motif "Conducteur trop jeune pour ce véhicule" |

  # Règle : Les motos extrêmement puissantes (>100ch) nécessitent 5 ans de permis
  Plan du Scénario: Vérification de l'expérience pour les motos puissantes
    Étant donné un conducteur avec <experience> ans de permis
    Et le véhicule est une "Moto" de <puissance> chevaux
    Quand je demande une éligibilité
    Alors la demande est <resultat>

    Exemples: Expérience insuffisante pour moto puissante
      | experience | puissance | resultat                                                         |
      | 2          | 120       | refusée avec le motif "Expérience insuffisante pour la puissance" |

    Exemples: Expérience suffisante pour moto puissante
      | experience | puissance | resultat |
      | 5          | 120       | acceptée |

    Exemples: Limite de puissance (boundary 100/101ch)
      | experience | puissance | resultat                                                         |
      | 1          | 100       | acceptée                                                         |
      | 2          | 101       | refusée avec le motif "Expérience insuffisante pour la puissance" |
