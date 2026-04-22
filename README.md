# ProjetScolariteSOLID

Application web de gestion de scolarité développée avec **ASP.NET Core 10 Razor Pages**, conçue comme projet d'apprentissage et de démonstration des **principes SOLID** et des bonnes pratiques d'architecture logicielle.

## Table des matières

- [Aperçu](#aperçu)
- [Architecture](#architecture)
- [Principes SOLID appliqués](#principes-solid-appliqués)
- [Technologies](#technologies)
- [Prérequis](#prérequis)
- [Installation et démarrage](#installation-et-démarrage)
- [Configuration](#configuration)
- [Déploiement IIS](#déploiement-iis)
- [Structure du projet](#structure-du-projet)
- [Fonctionnalités](#fonctionnalités)

---

## Aperçu

**ProjetScolariteSOLID** est une application de gestion scolaire permettant d'administrer :

- Les **étudiants**
- Les **enseignants**
- Les **matières**
- Les **classes**
- Les **inscriptions**
- Les **notes** et bulletins

L'application met en œuvre une architecture en couches (Clean Architecture légère), le pattern **CQRS** via MediatR, et des patterns de conception centrés sur les principes SOLID.

---

## Architecture

Le projet est organisé en **4 couches** distinctes :

```
ProjetScolariteSOLID/          ← Couche Présentation (Razor Pages)
src/
├── WebApplicationForSOLID.Domain/          ← Modèles métier & interfaces de repositories
├── WebApplicationForSOLID.Application/     ← Services, CQRS (Commands/Queries/Handlers), Validators
└── WebApplicationForSOLID.Infrastructure/  ← EF Core, Repositories, Notifications, Migrations
```

### Flux d'une requête

```
Razor Page → MediatR (LoggingBehavior → ValidationBehavior) → Handler → Service → Repository → SQL Server
```

---

## Principes SOLID appliqués

| Principe | Exemple concret dans le projet |
|---|---|
| **S** — Single Responsibility | Chaque service (`EtudiantService`, `NoteService`…) gère un seul agrégat |
| **O** — Open/Closed | `ScolariteDbContext` utilise `ApplyConfigurationsFromAssembly` : ajouter une entité ne nécessite pas de modifier le DbContext |
| **L** — Liskov Substitution | Les repositories EF (`EfEtudiantRepository`) sont substituables via leurs interfaces (`IEtudiantRepository`) |
| **I** — Interface Segregation | `IReadRepository<T>` et `IWriteRepository<T>` séparent les responsabilités de lecture et d'écriture |
| **D** — Dependency Inversion | Les couches supérieures dépendent uniquement des abstractions (interfaces), jamais des implémentations concrètes |

---

## Technologies

| Technologie | Rôle |
|---|---|
| **ASP.NET Core 10** | Framework web |
| **Razor Pages** | Interface utilisateur |
| **Entity Framework Core 10** | ORM & migrations |
| **SQL Server** | Base de données |
| **MediatR** | Pattern CQRS (Commands, Queries, Behaviors) |
| **Serilog** | Logging structuré (Console + SQL Server) |
| **IIS + ASP.NET Core Module V2** | Hébergement en production |

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/fr-fr/sql-server/sql-server-downloads) (version locale ou Express — **pas LocalDB en production**)
- Visual Studio 2022+ ou VS Code
- IIS avec le module **ASP.NET Core Module V2** (Hosting Bundle .NET 10) pour la production

---

## Installation et démarrage

### 1. Cloner le dépôt

```bash
git clone https://github.com/gloomandria/WebApplicationForSOLID.git
cd WebApplicationForSOLID
```

### 2. Configurer la connexion à la base de données

Modifier la chaîne de connexion dans `WebApplicationForSOLID/appsettings.json` :

```json
{
  "ConnectionStrings": {
    "ScolariteDb": "Server=.;Database=Scolarite_Dev;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Lancer l'application

```bash
dotnet run --project WebApplicationForSOLID/ProjetScolariteSOLID.csproj
```

> Les migrations EF Core et le seed des données sont appliqués **automatiquement** au démarrage.

---

## Configuration

### Fichiers appsettings

| Fichier | Environnement | Usage |
|---|---|---|
| `appsettings.json` | Base (tous) | Valeurs par défaut |
| `appsettings.Development.json` | Développement | LocalDB, logs détaillés — **exclu du publish** |
| `appsettings.Release.json` | Production IIS | SQL Server réel, logs minimalistes |

### Connexion SQL Server

| Paramètre | Développement | Production |
|---|---|---|
| `Server` | `(localdb)\MSSQLLocalDB` | `localhost` (ou nom de l'instance) |
| `Database` | `Scolarite_Dev` | `Scolarite_Prod` |
| `Trusted_Connection` | `True` | `True` (compte du pool IIS) |

> En production, le compte du **pool d'application IIS** doit avoir les droits `db_owner` sur la base `Scolarite_Prod`.

### Logging (Serilog)

Les logs sont écrits vers :
- La **console** (développement uniquement)
- Une table **`dbo.Logs`** dans SQL Server (créée automatiquement)

Le niveau minimum est configurable dans `appsettings.json` sous la clé `Serilog`.

---

## Déploiement IIS

### Prérequis serveur

1. Installer le **[.NET 10 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/10.0)** (inclut ASP.NET Core Module V2)
2. Redémarrer IIS après l'installation : `iisreset`

### Configuration IIS

1. Créer un **site IIS** pointant vers le dossier publié
2. Créer un **pool d'application** en mode **No Managed Code**
3. Donner au compte du pool les droits sur la base SQL Server

### Publier depuis Visual Studio

Utiliser le profil `IISProfile` (Web Deploy) :

```
Serveur       : WMGTOJO
Site IIS      : ScolariteWeb
URL du site   : http://scolarite.gestion.loc/
```

Ou via CLI :

```bash
dotnet publish WebApplicationForSOLID/ProjetScolariteSOLID.csproj -c Release -o ./publish
```

### Chaîne de connexion en production

Mettre à jour `appsettings.Release.json` avec le nom réel du serveur SQL :

```json
{
  "ConnectionStrings": {
    "ScolariteDb": "Server=NOM_SERVEUR;Database=Scolarite_Prod;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> Le `web.config` inclus définit automatiquement `ASPNETCORE_ENVIRONMENT=Release`.

---

## Structure du projet

```
WebApplicationForSOLID/
├── Middleware/
│   └── GlobalExceptionMiddleware.cs       ← Gestion centralisée des exceptions
├── Pages/
│   ├── Etudiants/                         ← CRUD étudiants (Index, Create, Edit, Delete, Details)
│   ├── Enseignants/                       ← CRUD enseignants (Index, Create, Edit, Delete)
│   ├── Matieres/                          ← CRUD matières (Index, Create, Edit, Delete)
│   ├── Classes/                           ← CRUD classes (Index, Create, Edit, Delete)
│   ├── Inscriptions/                      ← Inscriptions (Index, Create, EditStatut, Delete)
│   └── Notes/                             ← CRUD notes (Index, Create, Edit, Delete)
├── Properties/PublishProfiles/
│   └── IISProfile.pubxml                  ← Profil Web Deploy vers IIS
├── web.config                             ← Configuration ASP.NET Core Module V2
├── appsettings.json                       ← Configuration de base
├── appsettings.Development.json           ← Configuration développement
├── appsettings.Release.json               ← Configuration production
└── Program.cs                             ← Point d'entrée, configuration DI

src/WebApplicationForSOLID.Domain/
├── Models/                                ← Entités métier (Etudiant, Enseignant, Matiere, Classe, Inscription, Note…)
└── Repositories/                          ← Interfaces de repositories (IReadRepository, IWriteRepository…)

src/WebApplicationForSOLID.Application/
├── Contracts/                             ← Interfaces de services métier & IValidator<T>
├── Services/                              ← Implémentations des services
├── Validators/                            ← Validateurs métier
└── CQRS/
    ├── Behaviors/                         ← LoggingBehavior, ValidationBehavior
    ├── Etudiants/Commands|Queries|Handlers
    ├── Enseignants/Commands|Queries|Handlers
    ├── Matieres/Commands|Queries|Handlers
    ├── Classes/Commands|Queries|Handlers
    ├── Inscriptions/Commands|Queries|Handlers
    └── Notes/Commands|Queries|Handlers

src/WebApplicationForSOLID.Infrastructure/
├── Data/
│   ├── ScolariteDbContext.cs              ← DbContext EF Core
│   ├── ScolariteDbContextFactory.cs       ← Factory pour dotnet ef CLI
│   ├── Configurations/                    ← IEntityTypeConfiguration<T> par entité
│   ├── Migrations/                        ← Migrations EF Core
│   └── DataSeeder.cs                      ← Données initiales (idempotent)
├── Repositories/                          ← Implémentations EF Core des repositories
└── Notifications/
    └── DatabaseNotificationService.cs     ← Notifications via Serilog
```

---

## Fonctionnalités

- ✅ Gestion complète des **étudiants** (liste paginée, création, édition, suppression, bulletin)
- ✅ Gestion complète des **enseignants** (liste paginée, création, édition, suppression)
- ✅ Gestion complète des **matières** (liste, création, édition, suppression)
- ✅ Gestion complète des **classes** (liste, création, édition, suppression)
- ✅ Gestion des **inscriptions** (inscrire un étudiant, modifier le statut, supprimer)
- ✅ Saisie et gestion des **notes** avec génération de **bulletins**
- ✅ Pipeline CQRS avec **logging automatique** et **validation automatique**
- ✅ **Migrations** et **seed** automatiques au démarrage
- ✅ Gestion centralisée des erreurs via middleware
- ✅ Logs structurés persistés en base de données (Serilog → SQL Server)
- ✅ Déploiement **IIS** via Web Deploy avec `web.config` pré-configuré


---

## Aperçu

**ProjetScolariteSOLID** est une application de gestion scolaire permettant d'administrer :

- Les **étudiants**
- Les **enseignants**
- Les **matières**
- Les **classes**
- Les **inscriptions**
- Les **notes** et bulletins

L'application met en œuvre une architecture en couches (Clean Architecture légère), le pattern **CQRS** via MediatR, et des patterns de conception centrés sur les principes SOLID.

---

## Architecture

Le projet est organisé en **4 couches** distinctes :

```
ProjetScolariteSOLID/          ← Couche Présentation (Razor Pages)
src/
├── WebApplicationForSOLID.Domain/          ← Modèles métier & interfaces de repositories
├── WebApplicationForSOLID.Application/     ← Services, CQRS (Commands/Queries/Handlers), Validators
└── WebApplicationForSOLID.Infrastructure/  ← EF Core, Repositories, Notifications, Migrations
```

### Flux d'une requête

```
Razor Page → MediatR (LoggingBehavior → ValidationBehavior) → Handler → Repository → SQL Server
```

---

## Principes SOLID appliqués

| Principe | Exemple concret dans le projet |
|---|---|
| **S** — Single Responsibility | Chaque service (`EtudiantService`, `NoteService`…) gère un seul agrégat |
| **O** — Open/Closed | `ScolariteDbContext` utilise `ApplyConfigurationsFromAssembly` : ajouter une entité ne nécessite pas de modifier le DbContext |
| **L** — Liskov Substitution | Les repositories EF (`EfEtudiantRepository`) sont substituables via leurs interfaces (`IEtudiantRepository`) |
| **I** — Interface Segregation | `IReadRepository<T>` et `IWriteRepository<T>` séparent les responsabilités de lecture et d'écriture |
| **D** — Dependency Inversion | Les couches supérieures dépendent uniquement des abstractions (interfaces), jamais des implémentations concrètes |

---

## Technologies

| Technologie | Rôle |
|---|---|
| **ASP.NET Core 10** | Framework web |
| **Razor Pages** | Interface utilisateur |
| **Entity Framework Core 10** | ORM & migrations |
| **SQL Server** | Base de données |
| **MediatR** | Pattern CQRS (Commands, Queries, Behaviors) |
| **Serilog** | Logging structuré (Console + SQL Server) |

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/fr-fr/sql-server/sql-server-downloads) (version locale ou Express)
- Visual Studio 2022+ ou VS Code

---

## Installation et démarrage

### 1. Cloner le dépôt

```bash
git clone https://github.com/gloomandria/WebApplicationForSOLID.git
cd WebApplicationForSOLID
```

### 2. Configurer la connexion à la base de données

Modifier la chaîne de connexion dans `WebApplicationForSOLID/appsettings.json` :

```json
{
  "ConnectionStrings": {
    "ScolariteDb": "Server=.;Database=ScolariteDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Lancer l'application

```bash
dotnet run --project WebApplicationForSOLID/ProjetScolariteSOLID.csproj
```

> Les migrations EF Core et le seed des données sont appliqués **automatiquement** au démarrage.

---

## Configuration

### Connexion SQL Server

| Paramètre | Description | Valeur par défaut |
|---|---|---|
| `Server` | Nom du serveur SQL | `.` (instance locale) |
| `Database` | Nom de la base de données | `ScolariteDb` |
| `Trusted_Connection` | Authentification Windows | `True` |

### Logging (Serilog)

Les logs sont écrits vers :
- La **console**
- Une table **`dbo.Logs`** dans SQL Server (créée automatiquement)

Le niveau minimum est configurable dans `appsettings.json` sous la clé `Serilog`.

---

## Structure du projet

```
WebApplicationForSOLID/
├── Middleware/
│   └── GlobalExceptionMiddleware.cs       ← Gestion centralisée des exceptions
├── Pages/
│   ├── Etudiants/                         ← CRUD étudiants
│   ├── Enseignants/                       ← CRUD enseignants
│   └── Matieres/                          ← CRUD matières
└── Program.cs                             ← Point d'entrée, configuration DI

src/WebApplicationForSOLID.Domain/
├── Models/                                ← Entités métier (Etudiant, Enseignant, Note…)
└── Repositories/                          ← Interfaces de repositories (IReadRepository, IWriteRepository…)

src/WebApplicationForSOLID.Application/
├── Contracts/                             ← Interfaces de services métier
├── Services/                              ← Implémentations des services
├── Validators/                            ← Validateurs métier
└── CQRS/
    ├── Behaviors/                         ← LoggingBehavior, ValidationBehavior
    ├── Etudiants/Commands|Queries|Handlers
    ├── Enseignants/Commands|Queries|Handlers
    ├── Matieres/Commands|Queries|Handlers
    ├── Classes/Commands|Queries|Handlers
    ├── Inscriptions/Commands|Queries|Handlers
    └── Notes/Commands|Queries|Handlers

src/WebApplicationForSOLID.Infrastructure/
├── Data/
│   ├── ScolariteDbContext.cs              ← DbContext EF Core
│   ├── Configurations/                    ← IEntityTypeConfiguration<T> par entité
│   ├── Migrations/                        ← Migrations EF Core
│   └── DataSeeder.cs                      ← Données initiales
├── Repositories/                          ← Implémentations EF Core des repositories
└── Notifications/                         ← DatabaseNotificationService, LogNotificationService
```

---

## Fonctionnalités

- ✅ Gestion complète des **étudiants** (liste, création, édition, suppression, détail)
- ✅ Gestion complète des **enseignants**
- ✅ Gestion des **matières**
- ✅ Gestion des **classes** et des **inscriptions**
- ✅ Saisie et consultation des **notes** avec génération de **bulletins**
- ✅ Pipeline CQRS avec **logging** et **validation** automatiques
- ✅ **Migrations** et **seed** automatiques au démarrage
- ✅ Gestion centralisée des erreurs via middleware
- ✅ Logs structurés persistés en base de données