
# Projet MVC de Gestion d'Événements

## Description

Ce projet est une application web de gestion d'événements utilisant le modèle **MVC (Model-View-Controller)**. Elle permet de gérer les événements, les participants et les inscriptions, et d'afficher des statistiques sur ces événements. Cette application est conçue pour centraliser les informations et faciliter le processus de gestion pour les organisateurs d'événements.

## Fonctionnalités

- **Gestion des événements** : Créer, afficher, modifier et supprimer des événements.
- **Gestion des participants** : Ajouter des participants à des événements et afficher la liste des participants.
- **Statistiques** : Affichage des statistiques d'inscription pour chaque événement.
- **Vue responsive** : Interface utilisateur construite avec **Bootstrap** pour une utilisation sur différents appareils.

## Prérequis

Avant de commencer, assurez-vous d'avoir installé les outils suivants :

- **.NET 6.0 ou supérieur**.
- **Visual Studio 2022 ou un autre IDE compatible avec .NET**.
- **SQL Server** ou toute autre base de données relationnelle compatible avec **Entity Framework Core**.
- **MongoDB** (facultatif pour les statistiques).

## Installation

### Cloner le projet

Clonez le repository sur votre machine locale en utilisant Git :

```bash
git clone <URL_DU_PROJET>
cd <DOSSIER_DU_PROJET>
```

### Restaurer les dépendances

Pour restaurer les dépendances du projet, exécutez la commande suivante :

```bash
dotnet restore
```

### Configurer la base de données

Si vous utilisez **SQL Server**, appliquez les migrations **Entity Framework Core** pour créer la base de données :

```bash
dotnet ef database update
```

### Lancer l'application

Lancez l'application avec la commande suivante :

```bash
dotnet run
```

Cela démarrera l'application sur `http://localhost:5000` par défaut.


### Composants principaux

- **Controllers** : Contient la logique pour gérer les événements et les participants.
- **Models** : Représente les entités du projet comme `Événement`, `Participant`, etc.
- **Views** : Fichiers Razor pour afficher les événements et participants.
- **Data** : Contient la configuration pour Entity Framework Core et les migrations de base de données.

## API REST

L'application expose des APIs REST pour gérer les événements et les participants. Par exemple :

- **GET /api/evenements** : Récupère la liste de tous les événements.
- **POST /api/evenements** : Crée un nouvel événement.
- **PUT /api/evenements/{id}** : Modifie un événement existant.
- **DELETE /api/evenements/{id}** : Supprime un événement.
- **GET /api/participants/{evenementId}** : Récupère la liste des participants inscrits à un événement.

## Test

Pour exécuter les tests unitaires, vous pouvez utiliser **xUnit**. Les tests vérifient les fonctionnalités clés de l'application, y compris la gestion des événements et des participants.

```bash
dotnet test
```

## Technologies utilisées

- **ASP.NET Core MVC** pour la gestion des requêtes HTTP et le rendu des vues.
- **Entity Framework Core** pour l'accès aux données et la gestion des migrations.
- **MongoDB** pour le stockage des statistiques (facultatif).
- **Bootstrap** pour la création d'une interface responsive.
- **jQuery** pour les interactions dynamiques côté client.

## Contribuer

Si vous souhaitez contribuer au projet, suivez ces étapes :

1. Forkez le repository.
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/ma-fonctionnalite`).
3. Commitez vos modifications (`git commit -m 'Ajout de ma fonctionnalité'`).
4. Poussez vos changements (`git push origin feature/ma-fonctionnalite`).
5. Soumettez une pull request.

## Licence

Ce projet est sous la **Licence MIT**. Consultez le fichier **LICENSE** pour plus de détails.
