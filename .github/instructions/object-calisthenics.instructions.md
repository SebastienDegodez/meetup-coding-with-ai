---
description: "Object Calisthenics is a set of 9 rules to write cleaner, more maintainable code. Use this instruction to enforce these rules in your codebase."
applyTo: "**/*.Domain/*.cs"
---

1. **Un seul niveau d'indentation par méthode**
2. **Ne pas utiliser le mot-clé `else`**
3. **Encapsuler les primitives** - Pas d'int, string, bool nus. Toujours dans des Value Objects.
4. **Collections de première classe** - Une collection = une classe dédiée
5. **Un point par ligne** - Éviter les chaînes d'appels
6. **Ne pas abréger** - Noms explicites et complets
7. **Garder les entités petites** - Maximum 2 variables d'instance
8. **Pas de classes avec plus de 50 lignes**
9. **Pas de getters/setters** - Uniquement des méthodes métier