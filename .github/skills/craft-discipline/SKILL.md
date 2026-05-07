---
name: craft-discipline
description: Use when reviewing pull requests or code changes to verify commit readiness. Enforces quality gates across architecture, tests, naming, and business terminology before any merge.
---

# Craft Discipline

## Vue d'ensemble

Vérifier la disponibilité d'un commit avant toute fusion.

Principe fondamental : un changement n'est prêt à être fusionné que lorsque tous les critères de qualité ci-dessous sont satisfaits simultanément. Chaque critère est une porte bloquante — aucune exception.

## Quand utiliser ce skill

Utiliser ce skill lors de :
- La revue d'une pull request avant approbation
- La validation d'un ensemble de changements avant fusion
- L'évaluation de la disponibilité d'un commit en fin de cycle TDD

Ne pas utiliser pour des changements purement documentaires sans impact sur le code ou les tests.

## Les cinq critères de disponibilité (commit-readiness gates)

### 1. Conformité architecturale

| Vérification | Critère de passage |
| --- | --- |
| Dépendances entre couches | Domain → zéro dépendance externe ; Application → Domain uniquement ; Infrastructure → Application + Domain ; Api → Infrastructure uniquement |
| Validation NetArchTest | Aucune violation détectée par les tests d'architecture |
| Logique métier dans Domain | Aucune règle métier dans Api, Infrastructure, ou les handlers |
| Namespaces file-scoped | Tous les fichiers `.cs` modifiés utilisent des namespaces file-scoped |

### 2. Discipline de test

| Vérification | Critère de passage |
| --- | --- |
| Tests verts | Tous les tests passent (`dotnet test`) |
| Couverture Application + Domain | 100 % de couverture sur le code modifié |
| Score de mutation | 0 mutant survivant non-équivalent sur la logique métier modifiée |
| Mutants équivalents | Chaque mutant équivalent documenté explicitement avec justification |
| Tests sociables | Infrastructure mockée uniquement ; objets Domain réels dans les tests Application |

### 3. Cohérence du nommage

| Vérification | Critère de passage |
| --- | --- |
| Handlers | `{UseCase}{Type}Handler` (ex. `CheckEligibilityQueryHandler`) |
| DTOs Command/Query | `{UseCase}{Type}` (ex. `CheckEligibilityQuery`) |
| Tests | `When{Condition}_Should{ExpectedOutcome}()` |
| Classes | `sealed` par défaut ; héritage explicitement justifié |
| Constantes | `ALL_CAPS` |
| Paramètres d'exception | `nameof(param)` systématiquement |

### 4. Lexique métier FR → EN

| Vérification | Critère de passage |
| --- | --- |
| Nouveau terme français | Ajouté dans `.github/instructions/business-lexicon.instructions.md` dans le même changement |
| Terme existant modifié | Ligne correspondante mise à jour dans le même changement |
| Terminologie US | Orthographe américaine utilisée dans tout le code et la documentation |
| Cohérence inter-fichiers | Le même terme canonique utilisé dans le code, les tests, les docs et les messages PR |

### 5. Cohérence des fichiers de documentation

| Vérification | Critère de passage |
| --- | --- |
| README.md modifié | `README.fr.md` mis à jour dans le même changement |
| Structure de rubriques | Les deux fichiers README ont une structure de rubriques identique |
| Blocs de code | Identifiants techniques conservés en anglais dans `README.fr.md` |

## Procédure d'évaluation

1. Lire l'ensemble des fichiers modifiés dans le changement.
2. Évaluer chacun des cinq critères indépendamment.
3. Pour chaque vérification échouée, produire un constat bloquant avec référence de fichier et numéro de ligne.
4. Si tous les critères sont satisfaits, émettre une approbation explicite.
5. Ne jamais approuver un changement avec un constat `HIGH` ou `MEDIUM` non résolu.

## Modèle de constats bloquants

- `HIGH` : Violation de dépendance entre couches architecturales.
- `HIGH` : Tests échouants ou couverture incomplète sur le code modifié.
- `HIGH` : Mutant survivant non-équivalent non traité avant fusion.
- `HIGH` : Terme métier français introduit sans mise à jour du lexique dans le même changement.
- `MEDIUM` : Nommage non conforme à la convention (handler, test, DTO).
- `MEDIUM` : Traduction non canonique en conflit avec une entrée existante dans le lexique.
- `MEDIUM` : README.fr.md non mis à jour alors que README.md a été modifié.
- `LOW` : Terminologie non américaine sans impact fonctionnel.
- `LOW` : Commentaire de code superflu ou code commenté non supprimé.

## Erreurs fréquentes

- Approuver un changement avec des tests verts mais sans vérifier le score de mutation.
- Traiter un changement de texte métier comme cosmétique sans mettre à jour le lexique.
- Valider la couverture de code sans vérifier que les tests capturent le comportement réel (non l'implémentation).
- Confondre un mutant équivalent avec un vrai gap de test pour éviter d'écrire un test supplémentaire.
- Négliger la mise à jour de `README.fr.md` lors d'ajouts de rubriques dans `README.md`.

## Signaux d'alerte

- « C'est un changement mineur, on peut passer. »
- « On mettra le lexique à jour plus tard. »
- « Les tests passent, ça suffit. »
- « La couverture est déjà à 95 %, c'est bien. »
- « C'est de la documentation, pas besoin de revue. »

Si l'un de ces signaux apparaît, bloquer la fusion et appliquer la procédure d'évaluation complète.

## Intégration avec les autres skills

| Étape | Skill requis |
| --- | --- |
| Définir le comportement observable | `gherkin-gate` — scénarios approuvés avant tout code |
| Cycle TDD | `red-synthesize-green` — RED validé → SYNTHESIZE GREEN |
| Stratégie de test | `outside-in-tdd` — tests sociables, émergence du domaine |
| Qualité des tests | `mutation-testing` — score 100 % sur la logique métier |
| Violation architecturale | `clean-architecture-dotnet` — règles de couches et patterns |
| Terminologie métier | `reviewing-business-terminology` — cohérence FR → EN |
| Mise à jour du lexique | `updating-business-lexicon` — table toujours à jour |
