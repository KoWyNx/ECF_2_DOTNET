# Guide de Présentation du TP - Tests E2E et CI/CD (5 minutes)

## Introduction (30 secondes)
- Application web ASP.NET Core de gestion d'événements
- Objectif : Pipeline CI/CD avec tests E2E et analyse de qualité
- Technologies : Docker, Playwright, SonarQube, Trivy, GitHub Actions

## Points Clés à Présenter (4 minutes)

### 1. Architecture Conteneurisée (45 secondes)
- Application ASP.NET Core + SQL Server dans Docker
- Démonstration rapide : `docker-compose up -d` et accès à l'application

### 2. Tests E2E avec Playwright (45 secondes)
- Validation du comportement utilisateur (création d'événement)
- Montrer rapidement le code de test et exécuter `dotnet test`

### 3. Analyse de Sécurité avec Trivy (45 secondes)
- Analyse des dépendances et de l'image Docker
- Montrer les résultats et les vulnérabilités détectées

### 4. Analyse de Code avec SonarQube (45 secondes)
- Utilisation d'un script au lieu d'un conteneur pour plus de flexibilité
- Présenter le tableau de bord et les problèmes détectés (code smells, bugs)

### 5. Pipeline CI/CD avec GitHub Actions (45 secondes)
- Workflow automatisé : build, test, analyse
- Montrer le fichier de configuration et expliquer l'intégration des outils

## Conclusion (30 secondes)
- Bénéfices : détection précoce des problèmes, qualité de code, sécurité
- Démonstration que le pipeline fonctionne de bout en bout

## Commandes pour la Démonstration
```bash
# Démarrer l'environnement
docker-compose up -d

# Tests E2E
cd EcfDotnet.E2ETests.New && dotnet test

# Analyse Trivy
docker-compose up trivy

# Analyse SonarQube
export SONAR_TOKEN=squ_12746e19fbeaa144c66692b406fdd5075b7c5b30
export SONAR_HOST_URL=http://localhost:9000
./run-sonar-analysis.sh
```
