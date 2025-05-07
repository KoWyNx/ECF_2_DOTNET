# Guide de Présentation - Tests E2E et CI/CD

## Plan de Présentation 

### 1. Contexte du Projet 
- Application web ASP.NET Core de gestion d'événements
- Architecture : Application + SQL Server + SonarQube + Trivy

### 2. Scénarios Testés en E2E 
- Tests avec Playwright pour valider le comportement utilisateur
- Démonstration du test de création d'événement
- Explication de l'approche de test de bout en bout

### 3. Analyse de Sécurité avec Trivy 
- Résultats de l'analyse des dépendances et de l'image Docker
- Points critiques identifiés et leur impact
- Stratégies de correction des vulnérabilités

### 4. Rapport SonarQube 
- Dette technique identifiée
- Bugs et code smells détectés
- Qualité du code et couverture des tests

### 5. Pipeline CI/CD 
- Choix de GitHub Actions (vs GitLab CI)
- Étapes clés du workflow
- Intégration des outils d'analyse et de test

## Commandes pour la Démonstration

```bash
# Démarrer l'environnement complet
docker-compose up -d

# Exécuter les tests E2E avec Playwright
cd EcfDotnet.E2ETests.New && dotnet test

# Analyse de sécurité avec Trivy (dépendances)
trivy fs --format json --output rapports/trivy-deps.json .
trivy fs --format table .

# Analyse de sécurité avec Trivy (image Docker)
trivy image ecf_2_dotnet-app:latest --format json --output rapports/trivy-image.json
trivy image ecf_2_dotnet-app:latest --format table

# Analyse de qualité avec SonarQube
export SONAR_TOKEN=squ_12746e19fbeaa144c66692b406fdd5075b7c5b30
export SONAR_HOST_URL=http://localhost:9000

# Installer SonarScanner si nécessaire
dotnet tool install --global dotnet-sonarscanner

# Démarrer l'analyse
dotnet sonarscanner begin /k:"ECF_2_DOTNET" /n:"ECF_2_DOTNET" /d:sonar.host.url="$SONAR_HOST_URL" /d:sonar.login="$SONAR_TOKEN"

# Compiler le projet
dotnet build EcfDotnet.csproj

# Finaliser l'analyse
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"

# Accès aux rapports
echo "Rapport SonarQube: http://localhost:9000/dashboard?id=ECF_2_DOTNET"
echo "Rapports Trivy: voir le dossier ./rapports/"
```

## Points Clés à Souligner

- **Tests E2E**: Validation du comportement utilisateur réel avec Playwright
- **Sécurité**: Identification proactive des vulnérabilités avec Trivy
- **Qualité**: Mesure et amélioration continue avec SonarQube
- **Automatisation**: Pipeline complet pour garantir la qualité à chaque modification
