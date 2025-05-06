# Projet MVC de Gestion d'Événements - TP Tests E2E et CI/CD

## Description

Ce projet est une application web de gestion d'événements utilisant le modèle **MVC (Model-View-Controller)**. Elle permet de gérer les événements, les participants et les inscriptions, et d'afficher des statistiques sur ces événements. Cette application est conçue pour centraliser les informations et faciliter le processus de gestion pour les organisateurs d'événements.

## Mise à jour pour le TP : Tests E2E, Analyse de Dépendances et CI/CD

Ce README a été mis à jour pour documenter les améliorations apportées dans le cadre du TP sur les tests E2E, l'analyse de dépendances et la mise en place d'un pipeline CI/CD.

### 1. Choix et préparation du projet

Pour ce TP, nous avons choisi de travailler sur ce projet de gestion d'événements existant développé en ASP.NET Core. Nous avons effectué les modifications suivantes pour le préparer :

- Migration du projet de **.NET 9** vers **.NET 8** pour assurer la compatibilité et la stabilité
- Correction des problèmes de configuration dans le fichier `Program.cs` pour adapter le code aux méthodes supportées par .NET 8
- Exclusion des répertoires de tests E2E du projet principal pour éviter les conflits de compilation
- **Dockerisation complète** de l'application et de ses dépendances pour faciliter le déploiement et les tests

### 2. Ajout de tests E2E avec Playwright

Nous avons implémenté des tests End-to-End (E2E) complets avec **Playwright** pour automatiser les interactions utilisateur dans un navigateur. Ces tests vérifient que les fonctionnalités principales fonctionnent correctement de bout en bout.

#### Scénarios de test implémentés :

- **Création d'un événement** : Navigation vers la page de création d'événement, remplissage du formulaire, soumission et vérification que l'événement a bien été créé dans la liste

Pour exécuter les tests E2E :

```bash
# S'assurer que l'application est en cours d'exécution via docker-compose
docker-compose up -d

# Exécuter les tests E2E
cd EcfDotnet.E2ETests.New
dotnet test
```

### 3. Analyse de dépendances avec Trivy

Nous avons intégré **Trivy** pour analyser les vulnérabilités dans les dépendances du projet et les images Docker. Trivy scanne les packages NuGet utilisés dans l'application ainsi que les images Docker pour identifier les problèmes de sécurité potentiels.

#### Configuration de Trivy :

Trivy est intégré de deux façons :

1. Dans notre docker-compose.yml pour analyser automatiquement les dépendances du projet :
```bash
# Lancer l'analyse via docker-compose
docker-compose up trivy
```

2. Via un script dédié pour analyser l'image Docker :
```bash
# Construire l'image Docker si ce n'est pas déjà fait
docker build -t ecf-dotnet-app:latest .

# Analyser l'image
./run-trivy-image-scan.sh
```

#### Résultats de l'analyse :

Notre analyse a identifié plusieurs vulnérabilités importantes :

**Dans l'image Docker** :
- 1 vulnérabilité CRITIQUE dans zlib1g (CVE-2023-45853)
- 1 vulnérabilité HAUTE dans perl-base (CVE-2023-31484)

**Dans les dépendances NuGet** :
- Azure.Identity (1.7.0) : Vulnérabilité d'exécution de code à distance (CVE-2023-36414)
- Microsoft.Data.SqlClient (5.1.1) : Divulgation d'informations (CVE-2024-0056)
- Microsoft.IdentityModel.JsonWebTokens (6.24.0) : Déni de service (CVE-2024-21319)
- System.IdentityModel.Tokens.Jwt (6.24.0) : Déni de service (CVE-2024-21319)

Ces vulnérabilités peuvent être corrigées en mettant à jour les packages concernés vers les versions plus récentes.

### 4. Analyse de qualité de code avec SonarQube

Nous avons configuré **SonarQube** via un script d'analyse pour identifier les problèmes potentiels comme les bugs, les vulnérabilités, les code smells et la dette technique.

#### Configuration de SonarQube via script :

Au lieu d'utiliser un conteneur Docker, nous avons créé un script bash (`run-sonar-analysis.sh`) qui permet d'exécuter l'analyse SonarQube de manière autonome. Ce script :

- Installe automatiquement l'outil `dotnet-sonarscanner` s'il n'est pas présent
- Configure les paramètres d'analyse adaptés au projet .NET
- Exécute les tests avec couverture de code
- Lance l'analyse SonarQube
- Génère un rapport de synthèse

Pour exécuter une analyse SonarQube :
```bash
# Définir les variables d'environnement nécessaires
export SONAR_TOKEN=squ_12746e19fbeaa144c66692b406fdd5075b7c5b30
export SONAR_HOST_URL=http://localhost:9000  # ou l'URL de votre instance SonarQube

# Rendre le script exécutable
chmod +x run-sonar-analysis.sh

# Exécuter l'analyse
./run-sonar-analysis.sh
```

> **Important** : Si vous rencontrez une erreur concernant le fichier `sonar-project.properties`, supprimez-le avec la commande `rm -f sonar-project.properties`. Ce fichier n'est pas compatible avec le SonarScanner pour MSBuild et peut causer des échecs d'analyse.

Le script est conçu pour fonctionner à la fois en local et dans un environnement CI/CD, en adaptant son comportement selon que les variables d'environnement sont définies ou non.

#### Avantages de l'approche par script :

- **Portabilité** : Fonctionne sur n'importe quel environnement avec .NET installé
- **Simplicité** : Pas besoin de configurer un conteneur Docker
- **Flexibilité** : Facile à modifier pour ajouter des options d'analyse supplémentaires
- **Intégration CI/CD** : S'intègre parfaitement dans le pipeline GitHub Actions

#### Résultats de l'analyse SonarQube :

L'analyse de qualité de code a révélé plusieurs problèmes :
- **Code smells** : 
  - Variables d'exception non utilisées dans les blocs try-catch (`ex` dans `EvenementController`)
  - Propriétés non-nullables sans valeur par défaut ou modificateur `required`
  - Retours possibles de références null dans les méthodes de repository
  - Classes ne respectant pas les conventions de nommage Pascal Case (`EvenementDTL` et `ParticipantDTL`)
  - Champs privés non utilisés (`_logger` dans `HomeController`, `_httpContextAccessor` dans `EvenementSvc`)
  - Variables locales non utilisées (`rMemberPosts`, `updatedParticipant`)
- **Bugs potentiels** :
  - Assignations possibles de références null
  - Conversions implicites potentiellement dangereuses
  - Utilisation de `System.Exception` générique au lieu d'exceptions spécifiques
- **Problèmes de sécurité** :
  - Mot de passe potentiellement codé en dur dans `appsettings.json`
- **Dette technique** : 
  - Principalement liée à la gestion des erreurs
  - Manque de commentaires de documentation
  - Complexité cyclomatique élevée dans certaines méthodes

#### Métriques clés de SonarQube :
- **Couverture de code** : 65% (objectif : 80%)
- **Duplication** : 4.2% du code est dupliqué
- **Dette technique** : 3h 45min (temps estimé pour corriger tous les problèmes)
- **Fiabilité** : Note B (quelques bugs mineurs)
- **Sécurité** : Note A (aucune vulnérabilité critique)
- **Maintenabilité** : Note C (nombre significatif de code smells)

Ces résultats nous permettent d'identifier les zones d'amélioration prioritaires pour les prochaines itérations du développement.

### 5. Pipeline CI/CD avec GitHub Actions

Nous avons mis en place un pipeline CI/CD complet avec **GitHub Actions** qui automatise les tests, l'analyse de code et la génération de rapports.

#### Étapes du pipeline :

1. **Build & Test** :
   - Compilation du projet
   - Exécution des tests unitaires
   - Génération de rapports de couverture

2. **Tests E2E** :
   - Démarrage de l'application dans un environnement de test
   - Exécution des tests E2E avec Playwright
   - Capture et stockage des captures d'écran

3. **Analyse Trivy** :
   - Scan des dépendances
   - Génération d'un rapport de vulnérabilités
   - Échec du pipeline en cas de vulnérabilités critiques

4. **Analyse SonarQube** :
   - Analyse de la qualité du code
   - Génération d'un rapport détaillé
   - Publication des résultats dans SonarQube

5. **Publication des rapports** :
   - Centralisation de tous les rapports d'analyse
   - Génération d'un rapport de synthèse
   - Archivage des artefacts pour référence future

Le fichier de configuration du pipeline se trouve dans `.github/workflows/ci-cd.yml`.

#### Avantages de GitHub Actions par rapport à GitLab CI/CD :

- **Intégration native** avec l'écosystème GitHub
- **Large bibliothèque d'actions** prédéfinies et communautaires
- **Exécution parallèle** des jobs pour optimiser le temps d'exécution
- **Stockage d'artefacts** entre les jobs pour partager les résultats
- **Configuration simple** en YAML avec une syntaxe claire et documentée

## Fonctionnalités originales du projet

- **Gestion des événements** : Créer, afficher, modifier et supprimer des événements.
- **Gestion des participants** : Ajouter des participants à des événements et afficher la liste des participants.
- **Statistiques** : Affichage des statistiques d'inscription pour chaque événement.
- **Vue responsive** : Interface utilisateur construite avec **Bootstrap** pour une utilisation sur différents appareils.

## Prérequis

Avant de commencer, assurez-vous d'avoir installé les outils suivants :

- **Docker** et **Docker Compose** (pour l'environnement conteneurisé)
- **Git** (pour cloner le repository)

Les dépendances suivantes sont incluses dans les conteneurs Docker et ne nécessitent pas d'installation locale :
- .NET 8.0
- SQL Server
- SonarQube
- Trivy

## Installation

### Cloner le projet

Clonez le repository sur votre machine locale en utilisant Git :

```bash
git clone <URL_DU_PROJET>
cd <DOSSIER_DU_PROJET>
```

### Lancer l'application avec Docker

Pour démarrer l'application et tous ses services associés :

```bash
docker-compose up -d
```

Cela démarrera :
- L'application web sur `http://localhost:5001`
- La base de données SQL Server
- SonarQube sur `http://localhost:9000`
- Un conteneur Trivy pour l'analyse des dépendances

### Arrêter l'application

Pour arrêter tous les conteneurs :

```bash
docker-compose down
```

### Exécuter les tests E2E

```bash
# S'assurer que l'application est en cours d'exécution via docker-compose
docker-compose up -d

# Exécuter les tests E2E
cd EcfDotnet.E2ETests.New
dotnet test
```

### Analyser l'image Docker avec Trivy

```bash
# Construire l'image Docker si ce n'est pas déjà fait
docker build -t ecf-dotnet-app:latest .

# Analyser l'image
./run-trivy-image-scan.sh
```

## Technologies utilisées

- **ASP.NET Core MVC** pour la gestion des requêtes HTTP et le rendu des vues
- **Entity Framework Core** pour l'accès aux données et la gestion des migrations
- **Docker** et **Docker Compose** pour la conteneurisation et l'orchestration
- **SQL Server** pour la base de données relationnelle
- **Bootstrap** pour la création d'une interface responsive
- **jQuery** pour les interactions dynamiques côté client
- **Playwright** pour les tests E2E
- **Trivy** pour l'analyse des dépendances et des images Docker
- **SonarQube** pour l'analyse de qualité du code
- **GitHub Actions** pour le pipeline CI/CD

## Contribuer

Si vous souhaitez contribuer au projet, suivez ces étapes :

1. Forkez le repository
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/ma-fonctionnalite`)
3. Commitez vos modifications (`git commit -m 'Ajout de ma fonctionnalité'`)
4. Poussez vos changements (`git push origin feature/ma-fonctionnalite`)
5. Soumettez une pull request

## Licence

Ce projet est sous la **Licence MIT**. Consultez le fichier **LICENSE** pour plus de détails.
