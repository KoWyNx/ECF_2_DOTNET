# Guide de Présentation - Tests E2E et CI/CD

## Plan

1. **Contexte du Projet** : Application ASP.NET Core de gestion d'événements
2. **Tests E2E** : Playwright
3. **Sécurité** : Trivy
4. **Qualité** : SonarQube
5. **CI/CD** : GitHub Actions

## Commandes

```bash
# Environnement
docker-compose up -d

# Tests E2E
# Démarrer l'application dans un terminal
dotnet run --project EcfDotnet.csproj --urls=http://localhost:5001

# Puis dans un autre terminal
cd EcfDotnet.E2ETests.New && dotnet test

# Analyse Trivy
trivy fs --format table .
trivy image ecf_2_dotnet-app:latest --format table

# SonarQube
export SONAR_TOKEN=squ_12746e19fbeaa144c66692b406fdd5075b7c5b30
dotnet sonarscanner begin /k:"ECF_2_DOTNET" /n:"ECF_2_DOTNET" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="$SONAR_TOKEN"
dotnet build EcfDotnet.csproj
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
# Résultats: http://localhost:9000
```

## Points Clés

- **Tests E2E** : Validation du comportement utilisateur réel
- **Sécurité** : Identification proactive des vulnérabilités
- **Qualité** : Mesure et amélioration continue
- **CI/CD** : Automatisation des tests et analyses
