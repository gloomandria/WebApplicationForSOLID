# 🎓 Gestion Scolarité — ProjetScolariteSOLID

Application web ASP.NET Core MVC de gestion de scolarité, conçue selon les **principes SOLID** et l'**Architecture en couches (Clean Architecture)**. Elle couvre la gestion complète des étudiants, enseignants, classes, matières, inscriptions et notes, avec un back-office d'administration, un journal d'audit, un système d'e-mails et une API JWT.

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0.7-0078D4?logo=nuget)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoftsqlserver)
![MediatR](https://img.shields.io/badge/MediatR-14.1.0-orange?logo=nuget)
![xUnit](https://img.shields.io/badge/xUnit-2.9.3-blue?logo=xunit)
![Tests](https://img.shields.io/badge/Tests-64%20passed-brightgreen)

---

## 📑 Table des matières

- [Fonctionnalités](#fonctionnalités)
- [Architecture](#architecture)
- [Principes SOLID appliqués](#principes-solid-appliqués)
- [Technologies](#technologies)
- [Structure du projet](#structure-du-projet)
- [Prérequis](#prérequis)
- [Installation et démarrage](#installation-et-démarrage)
- [Configuration](#configuration)
- [Rôles et permissions](#rôles-et-permissions)
- [Journal d'audit](#journal-daudit)
- [Système d'e-mails](#système-de-mails)
- [API REST (JWT)](#api-rest-jwt)
- [Déploiement IIS](#déploiement-iis)
- [Tests unitaires](#tests-unitaires)

---

## Fonctionnalités

| Module | Description |
|---|---|
| **Étudiants** | CRUD complet, création de compte Identity lié, bulletin de notes avec moyennes pondérées et mention |
| **Enseignants** | CRUD complet, spécialité, grade, compte Identity lié |
| **Classes** | Gestion des promotions/classes avec niveau, filière et année académique |
| **Matières** | Catalogue avec coefficient, volume horaire et enseignant assigné |
| **Inscriptions** | Inscription étudiant → classe → année scolaire, contrôle de capacité |
| **Notes** | Saisie et consultation, type d'évaluation, génération de bulletins avec calcul automatique des moyennes |
| **Référentiels** | Spécialités, grades, filières, niveaux, années académiques, types d'évaluation, statuts d'inscription |
| **Tableau de bord** | Vue d'ensemble avec compteurs et moyennes générales par classe |
| **Administration** | Gestion des utilisateurs, rôles, matrice de permissions, file d'e-mails |
| **Journal d'audit** | Traçabilité de toutes les opérations INSERT/UPDATE/DELETE en base |
| **E-mails** | Templates WYSIWYG (Quill), file d'envoi asynchrone (SMTP) |
| **Compte** | Inscription, connexion, confirmation e-mail, mot de passe oublié / réinitialisation |
| **API JWT** | Authentification programmatique via `/api/auth` |

Toutes les listes (Étudiants, Enseignants, Classes, Matières, Inscriptions, Notes) utilisent **DataTables 2.1.8** avec pagination, tri et recherche côté serveur.

### 📊 Bulletin étudiant

Le système génère automatiquement un bulletin pour chaque étudiant avec :
- **Moyenne par matière** : Moyenne arithmétique des notes par matière
- **Moyenne pondérée** : Moyenne matière × Coefficient
- **Moyenne générale** : Somme des moyennes pondérées / Somme des coefficients
- **Mention automatique** :
  - ≥ 16 : Très bien
  - ≥ 14 : Bien
  - ≥ 12 : Assez bien
  - ≥ 10 : Passable
  - < 10 : Insuffisant

---

## Architecture

Le projet est organisé en **5 couches** distinctes :

```
ProjetScolariteSOLID/                     → Couche Présentation (ASP.NET Core MVC)
src/
  ProjetScolariteSOLID.Domain/              → Modèles métier & interfaces de repositories
  ProjetScolariteSOLID.Application/         → CQRS (Commands/Queries/Handlers), Services, Validators
  ProjetScolariteSOLID.Infrastructure/      → EF Core, Repositories, SMTP, Migrations
tests/
  ProjetScolariteSOLID.Tests/               → Tests unitaires (xUnit + Moq)
```

### Flux d'une requête

```
Razor View → Controller → IMediator (LoggingBehavior → ValidationBehavior) → Handler → Service → Repository → SQL Server
```

- Les **lectures** passent par des `Query` MediatR renvoyant des modèles de domaine.
- Les **écritures** passent par des `Command` MediatR renvoyant un `OperationResult<T>`.
- Les **contrôleurs** ne touchent jamais EF Core directement.

---

## Principes SOLID appliqués

| Principe | Exemple concret dans le projet |
|---|---|
| **S** — Single Responsibility | Chaque handler CQRS a une seule responsabilité. Les contrôleurs délèguent à MediatR. Les validators sont séparés des services. |
| **O** — Open/Closed | `ScolariteDbContext` utilise `ApplyConfigurationsFromAssembly` : ajouter une entité ne modifie pas le DbContext. Les référentiels utilisent `EfReferentielRepository<T>` générique. |
| **L** — Liskov Substitution | Les repositories EF (`EfEtudiantRepository`) sont substituables via leurs interfaces (`IEtudiantRepository`). |
| **I** — Interface Segregation | Les contrats applicatifs (`IEtudiantService`, `IEmailQueueService`…) sont séparés par domaine fonctionnel. `IReadRepository<T>` et `IWriteRepository<T>` sont distincts. |
| **D** — Dependency Inversion | Les couches hautes (Application, Présentation) dépendent d'abstractions (interfaces), jamais des implémentations concrètes. L'injection est centralisée dans `AddApplicationServices()` et `AddInfrastructureServices()`. |

---

## Technologies

| Catégorie | Technologie | Version | Rôle |
|---|---|---|---|
| **Framework** | ASP.NET Core MVC | .NET 10.0 | Framework web avec Razor Views |
| **ORM** | Entity Framework Core | 10.0.7 | Mapping objet-relationnel |
| **Base de données** | SQL Server | 2019+ | Stockage persistant |
| **Authentification** | ASP.NET Core Identity | 10.0.7 | Gestion des utilisateurs et rôles |
| **CQRS/Mediator** | MediatR | 14.1.0 | Pattern Commands/Queries séparées |
| **Logging** | Serilog | 10.0.0 | Logs structurés (Console + SQL Server) |
| **Logging SQL** | Serilog.Sinks.MSSqlServer | 9.0.3 | Persistance des logs en base |
| **E-mail** | MailKit | 4.16.0 | Envoi SMTP |
| **JWT** | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.7 | Authentification API REST |
| **UI Framework** | Bootstrap | 5.3 | Framework CSS responsive |
| **Tables interactives** | DataTables | 2.1.8 | Pagination/tri/recherche serveur |
| **Éditeur HTML** | Quill.js | 2.x | Édition WYSIWYG des templates |
| **Notifications** | SweetAlert2 | — | Alertes et confirmations |
| **Tests** | xUnit | 2.9.3 | Framework de tests unitaires |
| **Mocking** | Moq | 4.20.72 | Simulation de dépendances |
| **Code Coverage** | Coverlet | 10.0.0 | Analyse de couverture de code |

---

## Structure du projet

```
ProjetScolariteSOLID/
├── Controllers/
│   ├── AccountController.cs        # Connexion, inscription, activation, reset
│   ├── AdminController.cs          # Back-office : users, rôles, permissions, e-mails
│   ├── AuditController.cs          # Journal d'audit paginé + détail
│   ├── ClassesController.cs        # CRUD Classes (DataTables server-side)
│   ├── EmailTemplatesController.cs # Gestion des templates e-mail (Quill)
│   ├── EnseignantsController.cs    # CRUD Enseignants (DataTables server-side)
│   ├── EtudiantsController.cs      # CRUD Étudiants + bulletin (DataTables server-side)
│   ├── HomeController.cs           # Dashboard avec statistiques
│   ├── InscriptionsController.cs   # CRUD Inscriptions (DataTables server-side)
│   ├── MatieresController.cs       # CRUD Matières (DataTables server-side)
│   ├── NotesController.cs          # CRUD Notes (DataTables server-side)
│   ├── ReferentielsController.cs   # Spécialités, grades et autres référentiels
│   └── Api/
│       └── AuthApiController.cs    # POST /api/auth/login → JWT
├── Middleware/
│   └── GlobalExceptionMiddleware.cs# Gestion centralisée des exceptions
├── ViewModels/                     # ViewModels spécifiques par feature
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml          # Layout principal (DataTables CDN, Bootstrap 5)
│   │   └── _AuthLayout.cshtml      # Layout pages authentification
│   ├── Etudiants / Enseignants / …   # Vues CRUD avec modals AJAX
│   ├── Admin/                      # Users, Rôles, Permissions, EmailQueue
│   └── Audit/                      # Liste avec filtres + pagination ellipsis
├── wwwroot/js/
│   ├── ajax-helpers.js             # Helpers AJAX réutilisables
│   ├── etudiants.js                # DataTables init + AJAX CRUD
│   ├── enseignants.js
│   ├── classes.js / matieres.js / inscriptions.js / notes.js
│   └── emailtemplates.js           # DataTables client-side + Quill
├── appsettings.json / Development / Release
├── web.config                      # Configuration ASP.NET Core Module V2 (IIS)
└── Program.cs                      # Point d'entrée, configuration DI

src/ProjetScolariteSOLID.Domain/
├── Models/                         # Etudiant, Enseignant, Classe, Matiere,
│                                   # Inscription, Note, AuditLog, EmailTemplate…
│   └── Auth/                       # ApplicationUser, ApplicationRole, RolePermission, EmailQueue
└── Repositories/                   # IEtudiantRepository, IAuditLogRepository, IXxxRepository…

src/ProjetScolariteSOLID.Application/
├── CQRS/
│   ├── Behaviors/                  # LoggingBehavior, ValidationBehavior
│   └── Etudiants|Enseignants|…/
│       └── Commands/ Queries/ Handlers/
├── Contracts/                      # IValidator<T>, IEmailQueueService, IPermissionService…
├── Services/                       # EtudiantService, NoteService, InscriptionService…
├── Validators/                     # EtudiantValidator, NoteValidator, MatiereValidator…
└── Extensions/                     # AddApplicationServices() (DI registration)

src/ProjetScolariteSOLID.Infrastructure/
├── Data/
│   ├── ScolariteDbContext.cs       # DbContext EF Core + Audit interceptor
│   ├── DataSeeder.cs               # Seed admin, rôles, templates, données de démo (idempotent)
│   ├── Configurations/             # IEntityTypeConfiguration<T> par entité
│   └── Migrations/                 # Migrations EF Core
├── Repositories/                   # EfEtudiantRepository, EfAuditLogRepository…
├── Email/
│   ├── EfEmailQueueService.cs      # File d'attente d'e-mails en base
│   ├── EfEmailTemplateService.cs   # Gestion des templates e-mail
│   ├── SmtpEmailSender.cs          # Envoi SMTP via MailKit
│   └── EmailQueueBackgroundService.cs # Service d'arrière-plan
├── Notifications/
│   └── DatabaseNotificationService.cs # Notifications via Serilog
├── Auth/
│   └── PermissionService.cs        # Permissions par rôle
└── Extensions/                     # AddInfrastructureServices() (DI registration)

tests/ProjetScolariteSOLID.Tests/
├── Fixtures/Builders.cs            # EtudiantBuilder, NoteBuilder…
├── Validators/                     # Tests des règles de validation
└── Services/                       # Tests avec Moq (logique métier)
```

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server 2019+ (ou LocalDB pour le développement)
- (Optionnel) Serveur SMTP ou [Mailhog](https://github.com/mailhog/MailHog) pour les e-mails en local

---

## Installation et démarrage

### 1. Cloner le dépôt

```bash
git clone https://github.com/gloomandria/ProjetScolariteSOLID.git
cd ProjetScolariteSOLID
```

### 2. Configurer `appsettings.json`

Renseigner la chaîne de connexion et les autres paramètres (voir [Configuration](#configuration)).

### 3. Appliquer les migrations et le seed uniquement

```bash
dotnet run --project ProjetScolariteSOLID/ProjetScolariteSOLID.csproj -- --seed-only
```

Cette commande applique toutes les migrations EF Core et insère les données initiales (compte admin, rôles, templates e-mail, données de démonstration) puis s'arrête sans démarrer le serveur web.

### 4. Démarrer l'application

```bash
dotnet run --project ProjetScolariteSOLID/ProjetScolariteSOLID.csproj
```

> Les migrations et le seed sont aussi appliqués automatiquement au démarrage normal.

L'application est accessible sur `https://localhost:5001` (ou le port configuré).

### 5. Connexion initiale

| Champ | Valeur |
|---|---|
| Email | `admin@scolarite.local` (configurable) |
| Mot de passe | valeur de `AdminDefault:Password` dans `appsettings.json` |

> ⚠️ Changez le mot de passe admin immédiatement après la première connexion.

---

## Configuration

Toutes les clés se trouvent dans `ProjetScolariteSOLID/appsettings.json` :

```json
{
  "ConnectionStrings": {
    "ScolariteDb": "Server=.;Database=ScolariteDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "AdminDefault": {
    "Email": "admin@scolarite.local",
    "Password": "VotreMotDePasseAdmin!"
  },
  "Identity": {
    "DefaultPassword": "Changeme@1234!"
  },
  "Jwt": {
    "Key": "une_clé_secrète_d_au_moins_32_caractères!",
    "Issuer": "ScolariteApp",
    "Audience": "ScolariteApp",
    "ExpiresInMinutes": 480
  },
  "Smtp": {
    "Host": "localhost",
    "Port": "1025",
    "User": "",
    "Password": "",
    "From": "noreply@scolarite.local",
    "FromName": "Gestion Scolarité"
  }
}
```

| Clé | Description |
|---|---|
| `ConnectionStrings:ScolariteDb` | Chaîne de connexion SQL Server |
| `AdminDefault:Password` | Mot de passe du compte administrateur créé au seed |
| `Identity:DefaultPassword` | Mot de passe provisoire attribué aux nouveaux comptes (étudiant/enseignant) |
| `Jwt:Key` | Clé secrète HS256 (min. 32 caractères) pour la signature des tokens JWT |
| `Smtp:*` | Paramètres SMTP pour l'envoi d'e-mails |

---

## Rôles et permissions

L'application définit quatre rôles fixes :
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

| Rôle | Accès |
|---|---|
| `Administrateur` | Back-office complet, gestion des utilisateurs, audit, permissions |
| `Enseignant` | Consultation et édition des étudiants, matières, notes, inscriptions, classes |
| `Etudiant` | Consultation de ses informations et de son bulletin de notes |
| `Visiteur` | Accès en lecture seule au dashboard |

En plus des rôles, une **matrice de permissions** par ressource (Étudiants, Enseignants, Matières, Classes, Inscriptions, Notes, Référentiels) est configurable par l'administrateur via le back-office (`/Admin/Permissions`). Chaque rôle peut se voir accorder les droits `Lire`, `Créer`, `Modifier`, `Supprimer` par ressource.

---

## Journal d'audit

Toutes les opérations `INSERT`, `UPDATE` et `DELETE` en base de données sont automatiquement tracées dans la table `AuditLogs` via un **intercepteur EF Core** (`SaveChangesInterceptor`).

Chaque entrée contient :
- La table concernée, l'action et l'horodatage UTC
- Les clés primaires de l'entité
- Les anciennes et nouvelles valeurs (JSON)
- L'identifiant de l'utilisateur connecté

Le journal est consultable via `/Audit/Index` avec :
- **Filtrage** par table ou par identifiant d'utilisateur
- **Pagination** avec ellipsis (premières pages, pages proches de la courante, dernières pages)
- **Détail** complet d'une entrée via `/Audit/Details/{id}`

---

## Système d'e-mails

Les e-mails sont gérés de façon **asynchrone** via une file d'envoi (`EmailQueue`) :

1. Un e-mail est enregistré en base (`EmailQueue`) avec son template résolu.
2. Un **BackgroundService** (`EmailQueueBackgroundService`) tente l'envoi périodiquement (toutes les 30 secondes) via SMTP (MailKit).
3. En cas d'échec, le nombre de tentatives est incrémenté (max 5) et l'envoi est retenté.

Les **templates** sont éditables par l'administrateur via un éditeur WYSIWYG Quill.js (`/EmailTemplates`). Les variables sont substituées dynamiquement (ex. `{{NomComplet}}`, `{{Email}}`, `{{Lien}}`).

Templates livrés par défaut :
- `ConfirmationEmail` — confirmation d'adresse e-mail à l'inscription
- `NouvelleInscriptionAdmin` — notification à l'admin lors d'une nouvelle inscription
- `ResetMotDePasse` — lien de réinitialisation du mot de passe
- `CompteActive` — notification d'activation de compte
- `CompteDesactive` — notification de désactivation

---

## API REST (JWT)

Une API REST est disponible pour l'intégration programmatique.

### Authentification

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@scolarite.local",
  "password": "VotreMotDePasseAdmin!"
}
```

**Réponse (200 OK) :**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-01-01T16:00:00Z"
}
```

Transmettre le token dans le header `Authorization: Bearer <token>` pour les appels API sécurisés.

---

## Déploiement IIS

### Prérequis serveur

1. Installer le **[.NET 10 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/10.0)** (inclut ASP.NET Core Module V2)
2. Redémarrer IIS : `iisreset`

### Publication

```bash
dotnet publish ProjetScolariteSOLID/ProjetScolariteSOLID.csproj -c Release -o ./publish
```

### Configuration IIS

1. Créer un **site IIS** pointant vers le dossier publié
2. Créer un **pool d'application** en mode **No Managed Code**
3. Donner au compte du pool les droits `db_owner` sur la base SQL Server

> Le `web.config` inclus configure automatiquement `ASPNETCORE_ENVIRONMENT=Release`.

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
| `EtudiantValidator` | ✔ | Instanciation directe, règles de validation |
| `NoteValidator` | ✔ | Theory sur les bornes 0 / 20 |
| `EnseignantValidator` | ✔ | Instanciation directe |
| `MatiereValidator` | ✔ | Instanciation directe |
| `EtudiantService` | ✔ | `Mock<IEtudiantRepository>` + `Mock<IValidator<Etudiant>>` |
| `EnseignantService` | ✔ | `Mock<IEnseignantRepository>` |
| `MatiereService` | ✔ | `Mock<IMatiereRepository>` |
| `NoteService` | ✔ | `Mock<INoteRepository>` + `Mock<INotificationService>` |
| `InscriptionService` | ✔ | 6 mocks — cas capacité max + statut |
| **Total** | **64 tests** | **100% réussis** |

### Exemples de tests

```csharp
[Fact]
public async Task CreateAsync_etudiant_valide_retourne_succes()
{
    var etudiant = EtudiantBuilder.Valide(0);
    var cree = EtudiantBuilder.Valide(42);

    _validatorMock.Setup(v => v.Validate(etudiant)).Returns(new ValidationResult());
    _repoMock.Setup(r => r.AddAsync(etudiant, default)).ReturnsAsync(cree);

    var result = await _sut.CreateAsync(etudiant);

    Assert.True(result.IsSuccess);
    Assert.Equal(42, result.Value!.Id);
}

[Theory]
[InlineData(-1)]
[InlineData(21)]
public void Valeur_hors_bornes_retourne_erreur(decimal valeur)
{
    var note = NoteBuilder.Valide().WithValeur(valeur);
    var result = _validator.Validate(note);
    Assert.False(result.IsValid);
    Assert.Contains("entre 0 et 20", result.Errors.First());
}
```

---

## Licence

Ce projet est à usage éducatif pour démontrer l'application des principes SOLID et de l'architecture Clean Architecture dans un contexte ASP.NET Core.

---

## 📊 Schéma de la base de données

### Entités principales

```
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│    Etudiant     │──────│   Inscription   │──────│     Classe      │
│─────────────────│ 1..n │─────────────────│ n..1 │─────────────────│
│ Id              │      │ Id              │      │ Id              │
│ UserId (FK)     │      │ EtudiantId (FK) │      │ Nom             │
│ NumeroEtudiant  │      │ ClasseId (FK)   │      │ NiveauId (FK)   │
│ DateNaissance   │      │ StatutId (FK)   │      │ FiliereId (FK)  │
│ Adresse         │      │ DateInscription │      │ AnneeAcadId(FK) │
│ DateInscription │      └─────────────────┘      │ CapaciteMax     │
└─────────────────┘                               └─────────────────┘
        │ 1                                              
        │                                                
        │ n                                              
┌─────────────────┐      ┌─────────────────┐
│      Note       │──────│    Matiere      │
│─────────────────│ n..1 │─────────────────│
│ Id              │      │ Id              │
│ EtudiantId (FK) │      │ Code            │
│ MatiereId (FK)  │      │ Intitule        │
│ Valeur (0-20)   │      │ Coefficient     │
│ TypeEvalId (FK) │      │ VolumeHoraire   │
│ Date            │      └─────────────────┘
│ Commentaire     │
└─────────────────┘
```

### Référentiels (tables de paramétrage)

| Table | Description | Exemples de valeurs |
|-------|-------------|---------------------|
| `Filieres` | Filières d'études | Informatique, Mathématiques, Physique |
| `AnneesAcademiques` | Années scolaires | 2023-2024, 2024-2025 |
| `Niveaux` | Niveaux d'études | L1, L2, L3, M1, M2, Doctorat |
| `Specialites` | Spécialités enseignants | Informatique, Physique |
| `Grades` | Grades enseignants | Professeur, Maître de conférences, Assistant |
| `StatutsInscription` | Statuts d'inscription | Active, Suspendue, Annulée |
| `TypesEvaluation` | Types d'évaluation | Examen final, Partiel, Rattrapage, TP |

### Tables systèmes

| Table | Description |
|-------|-------------|
| `AspNetUsers` | Utilisateurs Identity (ApplicationUser) |
| `AspNetRoles` | Rôles applicatifs (ApplicationRole) |
| `AspNetUserRoles` | Association utilisateurs-rôles |
| `RolePermissions` | Matrice de permissions par rôle et ressource |
| `AuditLogs` | Journal d'audit des opérations CRUD |
| `EmailQueue` | File d'attente des e-mails |
| `EmailTemplates` | Templates d'e-mails paramétrables |
| `Logs` | Logs Serilog (auto-créée) |

---

## 📈 Statistiques du projet

| Métrique | Valeur |
|----------|--------|
| **Projets dans la solution** | 5 |
| **Entités métier** | 7 (Etudiant, Enseignant, Classe, Matiere, Inscription, Note, + références) |
| **Référentiels paramétrables** | 7 |
| **Controllers MVC** | 12 |
| **Vues Razor** | 50+ |
| **Tests unitaires** | 64 (100% passés) |
| **Commands CQRS** | 6 domaines |
| **Queries CQRS** | 6 domaines |
| **Services métier** | 6 |
| **Validators** | 4 |
| **Migrations EF Core** | 6 |

---

## 🔄 Changelog

### Version actuelle (dev/location-change)

- ✅ Architecture Clean Architecture complète
- ✅ Pattern CQRS avec MediatR
- ✅ Authentification ASP.NET Core Identity
- ✅ API REST avec JWT
- ✅ Journal d'audit automatique
- ✅ File d'e-mails asynchrone
- ✅ Templates d'e-mails éditables (Quill.js)
- ✅ Matrice de permissions par rôle
- ✅ DataTables server-side pour toutes les listes
- ✅ Validation métier séparée
- ✅ Logging Serilog (Console + SQL Server)
- ✅ 64 tests unitaires

---

## 📞 Contact

- **Repository** : [https://github.com/gloomandria/WebApplicationForSOLID](https://github.com/gloomandria/WebApplicationForSOLID)
- **Branche active** : `dev/location-change`
