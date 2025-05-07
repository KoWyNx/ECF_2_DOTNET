# Guide de Présentation - Tests E2E et CI/CD

## Plan de Présentation (5 minutes)

### 1. Contexte du Projet (30 secondes)
- Application web ASP.NET Core de gestion d'événements
- Architecture : Application + SQL Server + SonarQube + Trivy

### 2. Scénarios Testés en E2E (1 minute)
- Tests avec Playwright pour valider le comportement utilisateur
- Démonstration du test de création d'événement
- Explication de l'approche de test de bout en bout

### 3. Analyse de Sécurité avec Trivy (1 minute)
- Résultats de l'analyse des dépendances et de l'image Docker
- Points critiques identifiés et leur impact
- Stratégies de correction des vulnérabilités

### 4. Rapport SonarQube (1 minute)
- Dette technique identifiée
- Bugs et code smells détectés
- Qualité du code et couverture des tests

### 5. Pipeline CI/CD (1 minute 30)
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
./run-trivy-deps-scan.sh

# Analyse de sécurité avec Trivy (image Docker)
./run-trivy-image-scan.sh

# Analyse de qualité avec SonarQube
export SONAR_TOKEN=squ_12746e19fbeaa144c66692b406fdd5075b7c5b30
export SONAR_HOST_URL=http://localhost:9000
./run-sonar-analysis.sh

# Accès aux rapports
echo "Rapport SonarQube: http://localhost:9000/dashboard?id=ECF_2_DOTNET"
echo "Rapports Trivy: voir le dossier ./rapports/"
```

## Points Clés à Souligner

- **Tests E2E**: Validation du comportement utilisateur réel avec Playwright
- **Sécurité**: Identification proactive des vulnérabilités avec Trivy
- **Qualité**: Mesure et amélioration continue avec SonarQube
- **Automatisation**: Pipeline complet pour garantir la qualité à chaque modification
