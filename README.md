# ProjetScolariteSOLID

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0.7-512BD4)](https://docs.microsoft.com/ef/)
[![MediatR](https://img.shields.io/badge/MediatR-12.x-green)](https://github.com/jbogard/MediatR)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-orange)](https://docs.microsoft.com/dotnet/architecture/)
[![Tests](https://img.shields.io/badge/Tests-75%20xUnit-success)](tests/)

Application web de gestion scolaire développée avec **ASP.NET Core MVC (.NET 10)**, conçue comme projet d'apprentissage et de démonstration des **principes SOLID**, du pattern **CQRS** via MediatR, et des bonnes pratiques d'architecture logicielle en couches.

---

## Table des matières

- [Aperçu](#aperçu)
- [Architecture](#architecture)
- [Modèle de données](#modèle-de-données)
- [Principes SOLID appliqués](#principes-solid-appliqués)
- [Pattern CQRS et MediatR](#pattern-cqrs-et-mediatr)
- [Technologies](#technologies)
- [Prérequis](#prérequis)
- [Installation et démarrage](#installation-et-démarrage)
- [Configuration](#configuration)
- [Déploiement IIS](#déploiement-iis)
- [Structure du projet](#structure-du-projet)
- [Tests unitaires](#tests-unitaires)
- [Fonctionnalités](#fonctionnalités)
- [Référentiels de données](#référentiels-de-données)

---

## Aperçu

**ProjetScolariteSOLID** est une application web complète de gestion de scolarité universitaire permettant d'administrer :

- Les **étudiants** avec génération automatique du numéro (`ETU0001`, `ETU0002`...)
- Les **enseignants** avec grades et spécialités
- Les **matières** avec coefficients et volumes horaires
- Les **classes** par filière, niveau et année académique
- Les **inscriptions** avec contrôle de capacité et gestion des statuts
- Les **notes** et génération automatique de **bulletins** avec calcul des moyennes pondérées

L'application met en œuvre une architecture en couches (Clean Architecture), le pattern **CQRS** via MediatR, des interactions **AJAX dynamiques** (modales jQuery sans rechargement de page), et des patterns centrés sur les principes SOLID.

Toutes les opérations de modification retournent un `OperationResult` — aucune exception métier n'est levée dans la couche Application

---

## Architecture

Le projet est organisé en **4 couches** distinctes suivant les principes de la **Clean Architecture** :

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         ProjetScolariteSOLID                            │
│                    (Couche Présentation - Web)                          │
│         Controllers MVC • ViewModels • Views Razor • Middleware         │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │ Dépend de ↓
┌────────────────────────────────▼────────────────────────────────────────┐
│                 ProjetScolariteSOLID.Application                        │
│                    (Couche Application/Use Cases)                       │
│    CQRS (Commands/Queries/Handlers) • Services • Validators • Behaviors │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │ Dépend de ↓
┌────────────────────────────────▼────────────────────────────────────────┐
│                ProjetScolariteSOLID.Infrastructure                      │
│                      (Couche Infrastructure)                            │
│    EF Core DbContext • Repositories EF • Configurations • Migrations    │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │ Dépend de ↓
┌────────────────────────────────▼────────────────────────────────────────┐
│                    ProjetScolariteSOLID.Domain                          │
│                    (Couche Domain - Cœur Métier)                        │
│        Entités • Interfaces Repositories • Value Objects                │
│                   ★ AUCUNE DÉPENDANCE EXTERNE ★                         │
└─────────────────────────────────────────────────────────────────────────┘
```

```
tests/
└── ProjetScolariteSOLID.Tests/             ← Tests unitaires xUnit + Moq
```

### Flux d'une requête (CQRS)

```
HTTP Request
     ↓
┌─────────────────┐
│   Controller    │ ← Reçoit la requête, prépare la Command/Query
└────────┬────────┘
         ↓
┌─────────────────┐
│    MediatR      │ ← Dispatche vers le pipeline
└────────┬────────┘
         ↓
┌─────────────────┐
│ LoggingBehavior │ ← Log la requête, mesure la durée (alerte si > 500ms)
└────────┬────────┘
         ↓
┌─────────────────┐
│ValidationBehavior│ ← Valide les Commands via IValidator<T>
└────────┬────────┘
         ↓
┌─────────────────┐
│    Handler      │ ← Exécute la logique métier
└────────┬────────┘
         ↓
┌─────────────────┐
│   Repository    │ ← Accède aux données via EF Core
└────────┬────────┘
         ↓
┌─────────────────┐
│   SQL Server    │ ← Persistance
└─────────────────┘
```

---

## Modèle de données

### Schéma relationnel

```
┌────────────────────┐         ┌────────────────────┐         ┌────────────────────┐
│     ETUDIANT       │         │       CLASSE       │         │    ENSEIGNANT      │
├────────────────────┤         ├────────────────────┤         ├────────────────────┤
│ Id (PK)            │         │ Id (PK)            │         │ Id (PK)            │
│ NumeroEtudiant     │         │ Nom                │         │ Matricule          │
│ Nom                │         │ CapaciteMax        │         │ Nom                │
│ Prenom             │         │ NiveauId (FK)      │──┐      │ Prenom             │
│ Email (unique)     │         │ FiliereId (FK)     │──┼──┐   │ Email              │
│ Telephone          │         │ AnneeAcademiqueId  │──┼──┼─┐ │ Telephone          │
│ Adresse            │         └─────────┬──────────┘  │  │ │ │ SpecialiteId (FK)  │──┐
│ DateNaissance      │                   │             │  │ │ │ GradeId (FK)       │──┼─┐
│ DateInscription    │                   │             │  │ │ │ DateEmbauche       │  │ │
└─────────┬──────────┘                   │             │  │ │ └─────────┬──────────┘  │ │
          │                              │             │  │ │           │             │ │
          │    ┌─────────────────────────┘             │  │ │           │             │ │
          │    │                                       │  │ │           │             │ │
          ▼    ▼                                       │  │ │           ▼             │ │
┌────────────────────┐                                 │  │ │ ┌────────────────────┐  │ │
│    INSCRIPTION     │                                 │  │ │ │      MATIERE       │  │ │
├────────────────────┤                                 │  │ │ ├────────────────────┤  │ │
│ Id (PK)            │                                 │  │ │ │ Id (PK)            │  │ │
│ EtudiantId (FK)    │                                 │  │ │ │ Code               │  │ │
│ ClasseId (FK)      │                                 │  │ │ │ Intitule           │  │ │
│ StatutId (FK)      │──────────────────────────────┐  │  │ │ │ Description        │  │ │
│ DateInscription    │                              │  │  │ │ │ Coefficient        │  │ │
└────────────────────┘                              │  │  │ │ │ VolumeHoraire      │  │ │
                                                    │  │  │ │ │ EnseignantId (FK)  │──┘ │
          ┌─────────────────────────────────────────┘  │  │ │ └─────────┬──────────┘    │
          │                                            │  │ │           │               │
          ▼                                            ▼  ▼ ▼           │               │
┌────────────────────┐                    ┌────────────────────┐        │               │
│ STATUT_INSCRIPTION │                    │   REFERENTIELS     │        │               │
├────────────────────┤                    ├────────────────────┤        │               │
│ Id (PK)            │                    │ • Niveau           │        │               │
│ Libelle            │                    │ • Filiere          │        │               │
│ (Active, Suspendue,│                    │ • AnneeAcademique  │        │               │
│  Annulée)          │                    │ • Specialite       │←───────┼───────────────┘
└────────────────────┘                    │ • Grade            │←───────┘
                                          │ • TypeEvaluation   │
                                          └─────────┬──────────┘
                                                    │
┌────────────────────┐                              │
│        NOTE        │                              │
├────────────────────┤                              │
│ Id (PK)            │                              │
│ EtudiantId (FK)    │ ←── Lien vers Etudiant       │
│ MatiereId (FK)     │ ←── Lien vers Matiere        │
│ Valeur (0-20)      │                              │
│ TypeEvaluationId   │ ←────────────────────────────┘
│ Date               │
│ Commentaire        │
└────────────────────┘
```

### Entités principales

| Entité | Description | Propriétés clés |
|--------|-------------|-----------------|
| **Etudiant** | Représente un étudiant inscrit | `NumeroEtudiant` (auto-généré ETU0001), `Nom`, `Prenom`, `Email`, `DateNaissance` |
| **Enseignant** | Représente un enseignant | `Matricule`, `Nom`, `Prenom`, `Specialite`, `Grade` |
| **Classe** | Une classe d'étudiants | `Nom`, `Niveau`, `Filiere`, `AnneeAcademique`, `CapaciteMax` |
| **Matiere** | Une matière enseignée | `Code`, `Intitule`, `Coefficient`, `VolumeHoraire`, `Enseignant` |
| **Inscription** | Lien étudiant-classe | `Etudiant`, `Classe`, `Statut`, `DateInscription` |
| **Note** | Note d'un étudiant | `Etudiant`, `Matiere`, `Valeur` (0-20), `TypeEvaluation`, `Date` |

### Value Objects

| Classe | Description |
|--------|-------------|
| **BulletinEtudiant** | Bulletin académique avec moyennes pondérées et mention |
| **LigneNote** | Ligne du bulletin : matière, coefficient, moyenne pondérée |
| **PagedResult\<T\>** | Résultat paginé avec `Items`, `TotalCount`, `Page`, `PageSize` |
| **OperationResult\<T\>** | Pattern Result : `IsSuccess`, `Value`, `ErrorMessage` |
| **ValidationResult** | Résultat de validation avec liste d'erreurs |

---

## Principes SOLID appliqués

| Principe | Exemple concret dans le projet |
|---|---|
| **S** — Single Responsibility | Chaque service (`EtudiantService`, `NoteService`…) gère un seul agrégat. `EtudiantValidator` ne fait que valider, `LoggingBehavior` ne fait que logger. |
| **O** — Open/Closed | `ScolariteDbContext` utilise `ApplyConfigurationsFromAssembly` : ajouter une entité ne modifie pas le DbContext. Les Behaviors MediatR sont extensibles sans modification. |
| **L** — Liskov Substitution | Les repositories EF (`EfEtudiantRepository`) sont substituables via leurs interfaces (`IEtudiantRepository`) sans rompre de contrat. |
| **I** — Interface Segregation | `IReadRepository<T>` (lecture) et `IWriteRepository<T>` (écriture) séparent les responsabilités. Les clients n'implémentent que ce dont ils ont besoin. |
| **D** — Dependency Inversion | Les couches supérieures dépendent uniquement des abstractions (interfaces), jamais des implémentations concrètes. Injection via DI. |

### Exemples de code SOLID

**SRP - Single Responsibility :**
```csharp
// Chaque validateur a UNE seule responsabilité
public sealed class EtudiantValidator : IValidator<Etudiant>
{
    public ValidationResult Validate(Etudiant etudiant)
    {
        var result = new ValidationResult();
        if (string.IsNullOrWhiteSpace(etudiant.Nom))
            result.AddError("Le nom est obligatoire.");
        // ...
        return result;
    }
}
```

**OCP - Open/Closed :**
```csharp
// Ajouter une entité = créer une IEntityTypeConfiguration, sans modifier DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(EtudiantConfiguration).Assembly);
}
```

**ISP - Interface Segregation :**
```csharp
// Interfaces séparées pour lecture et écriture
public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
}

public interface IWriteRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
```

**DIP - Dependency Inversion :**
```csharp
// Les services dépendent d'abstractions, pas d'implémentations
services.AddScoped<IEtudiantRepository, EfEtudiantRepository>();
services.AddScoped<IEtudiantService, EtudiantService>();
```

---

## Pattern CQRS et MediatR

### Commands (Écriture)

```csharp
// Création d'un étudiant
public sealed record CreateEtudiantCommand(Etudiant Etudiant) : IRequest<OperationResult<Etudiant>>;

// Mise à jour
public sealed record UpdateEtudiantCommand(Etudiant Etudiant) : IRequest<OperationResult>;

// Suppression
public sealed record DeleteEtudiantCommand(int Id) : IRequest<OperationResult>;
```

### Queries (Lecture)

```csharp
// Liste paginée
public sealed record GetEtudiantsQuery(int Page, int PageSize) : IRequest<PagedResult<Etudiant>>;

// Par ID
public sealed record GetEtudiantByIdQuery(int Id) : IRequest<Etudiant?>;

// Bulletin académique
public sealed record GetEtudiantBulletinQuery(int EtudiantId) : IRequest<BulletinEtudiant?>;
```

### Behaviors (Cross-Cutting Concerns)

| Behavior | Responsabilité |
|----------|---------------|
| **LoggingBehavior** | Log chaque requête MediatR, mesure la durée, alerte si > 500ms |
| **ValidationBehavior** | Valide automatiquement les Commands via `IValidator<T>` avant exécution |

```csharp
// Pipeline : Logging → Validation → Handler
cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
```

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

### Versions des packages

| Package | Version |
|---------|---------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.7 |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.7 |
| `MediatR` | 12.x |
| `Serilog.AspNetCore` | 10.0.0 |
| `Serilog.Sinks.MSSqlServer` | 9.0.3 |
| `Serilog.Enrichers.Environment` | 3.0.1 |
| `xUnit` | 2.9.3 |
| `Moq` | 4.20.72 |
| `Microsoft.NET.Test.Sdk` | 17.12.0 |
| `coverlet.collector` | 6.0.4 |

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/fr-fr/sql-server/sql-server-downloads) (version locale ou Express)
- Visual Studio 2022+ ou VS Code
- IIS avec le module **ASP.NET Core Module V2** (Hosting Bundle .NET 10) pour la production

---

## Installation et démarrage

### 1. Cloner le dépôt

```bash
git clone https://github.com/gloomandria/ProjetScolariteSOLID.git
cd ProjetScolariteSOLID
```

### 2. Configurer la connexion à la base de données

Modifier la chaîne de connexion dans `ProjetScolariteSOLID/appsettings.Development.json` :

```json
{
  "ConnectionStrings": {
    "ScolariteDb": "Server=.;Database=Scolarite_Dev;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Lancer l'application

```bash
dotnet run --project ProjetScolariteSOLID/ProjetScolariteSOLID.csproj
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
dotnet publish ProjetScolariteSOLID/ProjetScolariteSOLID.csproj -c Release -o ./publish
```

> Le `web.config` inclus configure automatiquement `ASPNETCORE_ENVIRONMENT=Release`.

---

## Structure du projet

```
ProjetScolariteSOLID/
├── Controllers/                           ← 7 controllers MVC (Etudiants, Enseignants, Matieres,
│                                            Classes, Inscriptions, Notes, Home)
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

src/ProjetScolariteSOLID.Domain/
├── Models/                                ← Entités métier (Etudiant, Enseignant, Matiere,
│                                            Classe, Inscription, Note, Referentiels…)
└── Repositories/                          ← Interfaces (IReadRepository<T>, IWriteRepository<T>…)

src/ProjetScolariteSOLID.Application/
├── Contracts/                             ← IValidator<T>, ValidationResult, interfaces de services
├── Services/                              ← EtudiantService, EnseignantService, MatiereService,
│                                            NoteService, InscriptionService
├── Validators/                            ← EtudiantValidator, EnseignantValidator,
│                                            MatiereValidator, NoteValidator
└── CQRS/
    ├── Behaviors/                         ← LoggingBehavior, ValidationBehavior
    └── Etudiants|Enseignants|Matieres|Classes|Inscriptions|Notes/
        └── Commands/ Queries/ Handlers/

src/ProjetScolariteSOLID.Infrastructure/
├── Data/
│   ├── ScolariteDbContext.cs              ← DbContext EF Core
│   ├── Configurations/                    ← IEntityTypeConfiguration<T> par entité
│   ├── Migrations/                        ← Migrations EF Core
│   └── DataSeeder.cs                      ← Données initiales (idempotent)
├── Repositories/                          ← Implémentations EF Core
└── Notifications/
    └── DatabaseNotificationService.cs     ← INotificationService (inscriptions + notes)

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

- ✅ Gestion complète des **étudiants** (liste paginée, création, édition, suppression, numéro auto-généré)
- ✅ Gestion complète des **enseignants** (liste paginée, création, édition, suppression, grades et spécialités)
- ✅ Gestion complète des **matières** (liste, création, édition, suppression, coefficients, volumes horaires)
- ✅ Gestion complète des **classes** (liste, création, édition, suppression, capacité max, niveaux, filières)
- ✅ Gestion des **inscriptions** (inscrire, contrôle de capacité, modification du statut, suppression)
- ✅ Saisie et gestion des **notes** avec génération de **bulletins** (moyennes pondérées, mentions)
- ✅ Interactions **AJAX dynamiques** (modales sans rechargement de page)
- ✅ Pipeline CQRS avec **logging** et **validation automatiques** via MediatR Behaviors
- ✅ **Migrations** et **seed** automatiques au démarrage
- ✅ Gestion centralisée des erreurs (`GlobalExceptionMiddleware` : JSON pour AJAX, redirect pour navigation)
- ✅ Logs structurés persistés en base de données (Serilog → table `Logs`)
- ✅ Déploiement **IIS** via Web Deploy avec `web.config` pré-configuré
- ✅ **75 tests unitaires** couvrant Services et Validators de la couche Application
- ✅ Pattern **Result** (`OperationResult<T>`) : pas d'exceptions métier
- ✅ Support mode **seed-only** : `dotnet run -- --seed-only`

---

## Référentiels de données

L'application utilise des **tables de référence** pour les données de configuration :

| Référentiel | Valeurs initiales (seed) |
|-------------|--------------------------|
| **Niveau** | L1, L2, L3, M1, M2, Doctorat |
| **Filiere** | Informatique, Mathématiques, Physique, Chimie, Biologie, Lettres |
| **AnneeAcademique** | 2023-2024, 2024-2025, 2025-2026 |
| **Specialite** | Informatique, Mathématiques, Physique, Chimie, Biologie |
| **Grade** | Professeur, Maître de conférences, Assistant, Vacataire |
| **StatutInscription** | Active, Suspendue, Annulée |
| **TypeEvaluation** | Examen, Contrôle continu, TP, Oral, Projet |

### Calcul du bulletin

```csharp
public sealed class BulletinEtudiant
{
    public Etudiant Etudiant { get; init; }
    public IReadOnlyList<LigneNote> Lignes { get; init; }
    public decimal MoyenneGenerale { get; init; }

    public string Mention => MoyenneGenerale switch
    {
        >= 16 => "Très bien",
        >= 14 => "Bien",
        >= 12 => "Assez bien",
        >= 10 => "Passable",
        _ => "Insuffisant"
    };
}

public sealed class LigneNote
{
    public string IntituleMatiere { get; init; }
    public int Coefficient { get; init; }
    public decimal MoyenneMatiere { get; init; }
    public decimal MoyennePonderee => MoyenneMatiere * Coefficient;
}
```

---

## Licence

Ce projet est distribué sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

---

**Développé avec ❤️ pour démontrer les principes SOLID et la Clean Architecture en .NET 10**
