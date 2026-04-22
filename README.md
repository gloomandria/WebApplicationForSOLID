# ProjetScolariteSOLID

Application web de gestion scolaire développée avec **ASP.NET Core MVC (.NET 10)**, conçue comme projet d'apprentissage et de démonstration des **principes SOLID**, du pattern **CQRS** via MediatR, et des bonnes pratiques d'architecture logicielle en couches.

---

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
- [Tests unitaires](#tests-unitaires)
- [Fonctionnalités](#fonctionnalités)

---

## Aperçu

**ProjetScolariteSOLID** est une application web de gestion scolaire permettant d'administrer :

- Les **étudiants**
- Les **enseignants**
- Les **matières**
- Les **classes**
- Les **inscriptions** (avec contrôle de capacité et gestion des statuts)
- Les **notes** et bulletins

L'application met en œuvre une architecture en couches (Clean Architecture légère), le pattern **CQRS** via MediatR, des interactions **AJAX dynamiques** (modales jQuery sans rechargement de page), et des patterns centrés sur les principes SOLID.

Toutes les opérations de modification retournent un `OperationResult` — aucune exception métier n'est levée dans la couche Application.

---

## Architecture

Le projet est organisé en **5 couches** distinctes :

```
WebApplicationForSOLID/        ← Couche Présentation (ASP.NET Core MVC)
src/
├── WebApplicationForSOLID.Domain/          ← Modèles métier & interfaces de repositories
├── WebApplicationForSOLID.Application/     ← Services, CQRS (Commands/Queries/Handlers), Validators
└── WebApplicationForSOLID.Infrastructure/  ← EF Core, Repositories, Notifications, Migrations
tests/
└── ProjetScolariteSOLID.Tests/             ← Tests unitaires xUnit + Moq
```

### Flux d'une requête

```
Controller → MediatR (LoggingBehavior → ValidationBehavior) → Handler → Service → Repository → SQL Server
```

---

## Principes SOLID appliqués

| Principe | Exemple concret dans le projet |
|---|---|
| **S** — Single Responsibility | Chaque service (`EtudiantService`, `NoteService`…) gère un seul agrégat |
| **O** — Open/Closed | `ScolariteDbContext` utilise `ApplyConfigurationsFromAssembly` : ajouter une entité ne modifie pas le DbContext |
| **L** — Liskov Substitution | Les repositories EF (`EfEtudiantRepository`) sont substituables via leurs interfaces (`IEtudiantRepository`) |
| **I** — Interface Segregation | `IReadRepository<T>` et `IWriteRepository<T>` séparent les responsabilités de lecture et d'écriture |
| **D** — Dependency Inversion | Les couches supérieures dépendent uniquement des abstractions (interfaces), jamais des implémentations concrètes |

---

## Technologies

| Technologie | Rôle |
|---|---|
| **ASP.NET Core MVC 10** | Framework web — Controllers, Views Razor, ViewModels |
| **jQuery AJAX** | Interactions dynamiques (modales, chargement partiel sans rechargement de page) |
| **Bootstrap** | UI responsive |
| **Entity Framework Core 10** | ORM & migrations |
| **SQL Server** | Base de données |
| **MediatR** | Pattern CQRS (Commands, Queries, Behaviors) |
| **Serilog** | Logging structuré (Console + SQL Server) |
| **xUnit + Moq** | Tests unitaires de la couche Application |
| **coverlet** | Couverture de code |

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/fr-fr/sql-server/sql-server-downloads) (version locale ou Express)
- Visual Studio 2022+ (ou VS Code)
- IIS avec le module **ASP.NET Core Module V2** (Hosting Bundle .NET 10) pour la production

---

## Installation et démarrage

### 1. Cloner le dépôt

```bash
git clone https://github.com/gloomandria/WebApplicationForSOLID.git
cd WebApplicationForSOLID
```

### 2. Configurer la connexion à la base de données

Modifier la chaîne de connexion dans `WebApplicationForSOLID/appsettings.Development.json` :

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
| `appsettings.Development.json` | Développement | SQL Server local, logs détaillés — **exclu du publish** |
| `appsettings.Release.json` | Production IIS | SQL Server réel, logs minimalistes |

### Connexion SQL Server

| Paramètre | Développement | Production |
|---|---|---|
| `Server` | `.` ou `(localdb)\MSSQLLocalDB` | Nom du serveur SQL |
| `Database` | `Scolarite_Dev` | `Scolarite_Prod` |
| `Trusted_Connection` | `True` | `True` (compte du pool IIS) |

> En production, le compte du **pool d'application IIS** doit avoir les droits `db_owner` sur la base.

### Authentification JWT

Configurer les paramètres JWT dans `appsettings.json` :

```json
"Jwt": {
  "Key": "<clé secrète de 32+ caractères>",
  "Issuer": "ScolariteApp",
  "Audience": "ScolariteApp",
  "ExpiresInMinutes": 480
}
```

### Administrateur par défaut

Un compte administrateur est créé automatiquement au premier démarrage :

```json
"AdminDefault": {
  "Email": "admin@scolarite.local",
  "Password": "<mot de passe>"
}
```

### SMTP (envoi d'e-mails)

Configurer les paramètres SMTP dans `appsettings.json` :

```json
"Smtp": {
  "Host": "smtp.example.com",
  "Port": "587",
  "User": "<utilisateur>",
  "Password": "<mot de passe>",
  "From": "noreply@scolarite.local",
  "FromName": "Gestion Scolarité"
}
```

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
3. Donner au compte du pool les droits `db_owner` sur la base SQL Server

### Publier via CLI

```bash
dotnet publish WebApplicationForSOLID/ProjetScolariteSOLID.csproj -c Release -o ./publish
```

> Le `web.config` inclus configure automatiquement `ASPNETCORE_ENVIRONMENT=Release`.

---

## Structure du projet

```
WebApplicationForSOLID/
├── Controllers/                           ← 10 controllers MVC (Etudiants, Enseignants, Matieres,
│                                            Classes, Inscriptions, Notes, Home, Account, Admin, Audit)
│   └── Api/
│       └── AuthApiController.cs           ← API REST d'authentification JWT
├── Middleware/
│   └── GlobalExceptionMiddleware.cs       ← Gestion centralisée des exceptions
│                                            (JSON pour AJAX, redirect pour navigation classique)
├── ViewModels/                            ← ViewModels par section
├── Views/                                 ← Vues Razor par section (modales AJAX incluses)
├── wwwroot/js/
│   ├── ajax-helpers.js                    ← Helpers AJAX mutualisés (modal, soumission, suppression)
│   └── etudiants.js / enseignants.js / …  ← JS spécifique par section
├── appsettings.json / Development / Release
├── web.config                             ← Configuration ASP.NET Core Module V2
└── Program.cs                             ← Point d'entrée, configuration DI

src/WebApplicationForSOLID.Domain/
├── Models/                                ← Entités métier (Etudiant, Enseignant, Matiere,
│                                            Classe, Inscription, Note, Referentiels…)
│   └── Auth/                              ← ApplicationUser, ApplicationRole, EmailQueue, RolePermission
├── Repositories/                          ← Interfaces (IReadRepository<T>, IWriteRepository<T>,
│                                            IAuditLogRepository…)

src/WebApplicationForSOLID.Application/
├── Contracts/                             ← IValidator<T>, ValidationResult, interfaces de services,
│                                            IEmailQueueService, ISmtpEmailSender, IPermissionService
├── Services/                              ← EtudiantService, EnseignantService, MatiereService,
│                                            NoteService, InscriptionService
├── Validators/                            ← EtudiantValidator, EnseignantValidator,
│                                            MatiereValidator, NoteValidator
└── CQRS/
    ├── Behaviors/                         ← LoggingBehavior, ValidationBehavior
    └── Etudiants|Enseignants|Matieres|Classes|Inscriptions|Notes/
        └── Commands/ Queries/ Handlers/

src/WebApplicationForSOLID.Infrastructure/
├── Data/
│   ├── ScolariteDbContext.cs              ← DbContext EF Core
│   ├── Configurations/                    ← IEntityTypeConfiguration<T> par entité
│   ├── Migrations/                        ← Migrations EF Core
│   └── DataSeeder.cs                      ← Données initiales (idempotent)
├── Repositories/                          ← Implémentations EF Core
├── Notifications/
│   └── DatabaseNotificationService.cs     ← INotificationService (inscriptions + notes)
├── Email/
│   ├── EfEmailQueueService.cs             ← File d'attente d'e-mails en base
│   ├── SmtpEmailSender.cs                 ← Envoi SMTP
│   └── EmailQueueBackgroundService.cs     ← Service d'arrière-plan pour l'envoi
└── Auth/
    └── PermissionService.cs               ← Gestion des permissions par rôle

tests/ProjetScolariteSOLID.Tests/
├── Fixtures/
│   └── Builders.cs                        ← EtudiantBuilder, NoteBuilder, EnseignantBuilder,
│                                            MatiereBuilder
├── Validators/                            ← Tests sans mock (règles de validation)
└── Services/                              ← Tests avec Moq (logique métier)
```

---

## Tests unitaires

Le projet `tests/ProjetScolariteSOLID.Tests` couvre la couche **Application** (Services + Validators) sans dépendance à la base de données.

```bash
# Lancer tous les tests
dotnet test tests/ProjetScolariteSOLID.Tests

# Avec couverture de code
dotnet test tests/ProjetScolariteSOLID.Tests --collect:"XPlat Code Coverage"
```

| Classe testée | Tests | Stratégie |
|---|---|---|
| `EtudiantValidator` | 9 | Instanciation directe |
| `NoteValidator` | 8 | Instanciation directe (Theory sur les bornes 0/10/20) |
| `EnseignantValidator` | 8 | Instanciation directe |
| `MatiereValidator` | 7 | Instanciation directe |
| `EtudiantService` | 8 | `Mock<IEtudiantRepository>` + `Mock<IValidator<Etudiant>>` |
| `EnseignantService` | 7 | `Mock<IEnseignantRepository>` |
| `MatiereService` | 8 | `Mock<IMatiereRepository>` |
| `NoteService` | 8 | `Mock<INoteRepository>` + `Mock<INotificationService>` |
| `InscriptionService` | 10 | 6 mocks — cas capacité max + statut absent (`InvalidOperationException`) |
| **Total** | **75** | |

---

## Fonctionnalités

- ✅ Gestion complète des **étudiants** (liste paginée, création, édition, suppression)
- ✅ Gestion complète des **enseignants** (liste paginée, création, édition, suppression)
- ✅ Gestion complète des **matières** (liste, création, édition, suppression)
- ✅ Gestion complète des **classes** (liste, création, édition, suppression)
- ✅ Gestion des **inscriptions** (inscrire, contrôle de capacité, modification du statut, suppression)
- ✅ Saisie et gestion des **notes** avec génération de **bulletins**
- ✅ Interactions **AJAX dynamiques** (modales sans rechargement de page)
- ✅ Pipeline CQRS avec **logging** et **validation automatiques** via MediatR Behaviors
- ✅ **Authentification** ASP.NET Identity (inscription, connexion, mot de passe oublié/réinitialisation)
- ✅ **API REST JWT** pour l'authentification (`/api/auth`)
- ✅ **Rôles et permissions** — administration des utilisateurs, rôles et permissions par rôle
- ✅ **File d'attente d'e-mails** SMTP avec service d'arrière-plan (`EmailQueueBackgroundService`)
- ✅ **Journal d'audit** — traçabilité des opérations (AuditLog)
- ✅ **Migrations** et **seed** automatiques au démarrage (dont admin par défaut)
- ✅ Gestion centralisée des erreurs (JSON pour AJAX, redirect pour navigation classique)
- ✅ Logs structurés persistés en base de données (Serilog)
- ✅ Déploiement **IIS** via Web Deploy avec `web.config` pré-configuré
- ✅ **75 tests unitaires** couvrant Services et Validators de la couche Application
