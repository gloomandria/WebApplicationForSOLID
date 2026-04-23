# ?? Gestion Scolarité — ProjetScolariteSOLID

Application web ASP.NET Core MVC de gestion de scolarité, conçue selon les **principes SOLID** et l'**Architecture en couches (Clean Architecture)**. Elle couvre la gestion complète des étudiants, enseignants, classes, matières, inscriptions et notes, avec un back-office d'administration, un journal d'audit, un système d'e-mails et une API JWT.

---

## ?? Table des matières

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
| **Étudiants** | CRUD complet, création de compte Identity lié, bulletin de notes |
| **Enseignants** | CRUD complet, spécialité, grade, compte Identity lié |
| **Classes** | Gestion des promotions/classes |
| **Matières** | Catalogue avec coefficient |
| **Inscriptions** | Inscription étudiant ? classe ? année scolaire, contrôle de capacité |
| **Notes** | Saisie et consultation, génération de bulletins par étudiant |
| **Référentiels** | Spécialités et grades (admin) |
| **Administration** | Gestion des utilisateurs, rôles, matrice de permissions, file d'e-mails |
| **Journal d'audit** | Traçabilité de toutes les opérations INSERT/UPDATE/DELETE en base |
| **E-mails** | Templates WYSIWYG (Quill), file d'envoi asynchrone (SMTP) |
| **Compte** | Inscription, connexion, confirmation e-mail, mot de passe oublié / réinitialisation |
| **API JWT** | Authentification programmatique via `/api/auth` |

Toutes les listes (Étudiants, Enseignants, Classes, Matières, Inscriptions, Notes) utilisent **DataTables 2.1.8** avec pagination, tri et sélection de taille de page côté serveur.

---

## Architecture

Le projet est organisé en **5 couches** distinctes :

```
WebApplicationForSOLID/                     ? Couche Présentation (ASP.NET Core MVC)
src/
  WebApplicationForSOLID.Domain/            ? Modèles métier & interfaces de repositories
  WebApplicationForSOLID.Application/       ? CQRS (Commands/Queries/Handlers), Services, Validators
  WebApplicationForSOLID.Infrastructure/    ? EF Core, Repositories, SMTP, Migrations
tests/
  ProjetScolariteSOLID.Tests/               ? Tests unitaires (xUnit + Moq)
```

### Flux d'une requête

```
Razor View ? Controller ? IMediator (LoggingBehavior ? ValidationBehavior) ? Handler ? Repository ? SQL Server
```

- Les **lectures** passent par des `Query` MediatR renvoyant des modèles de domaine.
- Les **écritures** passent par des `Command` MediatR renvoyant un `OperationResult<T>`.
- Les **contrôleurs** ne touchent jamais EF Core directement.

---

## Principes SOLID appliqués

| Principe | Exemple concret dans le projet |
|---|---|
| **S** — Single Responsibility | Chaque handler CQRS a une seule responsabilité. Les contrôleurs délèguent à MediatR. |
| **O** — Open/Closed | `ScolariteDbContext` utilise `ApplyConfigurationsFromAssembly` : ajouter une entité ne modifie pas le DbContext. |
| **L** — Liskov Substitution | Les repositories EF (`EfEtudiantRepository`) sont substituables via leurs interfaces (`IEtudiantRepository`). |
| **I** — Interface Segregation | Les contrats applicatifs (`IEtudiantService`, `IEmailQueueService`…) sont séparés par domaine fonctionnel. |
| **D** — Dependency Inversion | Les couches hautes (Application, Présentation) dépendent d'abstractions (interfaces), jamais des implémentations concrètes. |

---

## Technologies

| Catégorie | Technologie | Version |
|---|---|---|
| Framework | ASP.NET Core MVC | .NET 10 |
| ORM | Entity Framework Core | 10.x |
| Base de données | SQL Server | 2019+ |
| Authentification | ASP.NET Core Identity | — |
| CQRS | MediatR | 12.x |
| Pagination / Tri | DataTables | 2.1.8 |
| UI | Bootstrap | 5.3 |
| Éditeur HTML | Quill.js | 2.x |
| Logging | Serilog (Console + SQL Server) | — |
| API | JWT Bearer | — |
| Tests | xUnit + Moq | — |

---

## Structure du projet

```
WebApplicationForSOLID/
??? Controllers/
?   ??? AccountController.cs        # Connexion, inscription, activation, reset
?   ??? AdminController.cs          # Back-office : users, rôles, permissions, e-mails
?   ??? AuditController.cs          # Journal d'audit paginé + détail
?   ??? ClassesController.cs        # CRUD Classes (DataTables server-side)
?   ??? EmailTemplatesController.cs # Gestion des templates e-mail (Quill)
?   ??? EnseignantsController.cs    # CRUD Enseignants (DataTables server-side)
?   ??? EtudiantsController.cs      # CRUD Étudiants + bulletin (DataTables server-side)
?   ??? HomeController.cs           # Dashboard avec statistiques
?   ??? InscriptionsController.cs   # CRUD Inscriptions (DataTables server-side)
?   ??? MatieresController.cs       # CRUD Matières (DataTables server-side)
?   ??? NotesController.cs          # CRUD Notes (DataTables server-side)
?   ??? ReferentielsController.cs   # Spécialités et grades
?   ??? Api/
?       ??? AuthApiController.cs    # POST /api/auth/login ? JWT
??? Middleware/
?   ??? GlobalExceptionMiddleware.cs# Gestion centralisée des exceptions
??? ViewModels/                     # ViewModels spécifiques par feature
??? Views/
?   ??? Shared/
?   ?   ??? _Layout.cshtml          # Layout principal (DataTables CDN, Bootstrap 5)
?   ?   ??? _AuthLayout.cshtml      # Layout pages authentification
?   ??? Etudiants / Enseignants / …   # Vues CRUD avec modals AJAX
?   ??? Admin/                      # Users, Rôles, Permissions, EmailQueue
?   ??? Audit/                      # Liste avec filtres + pagination ellipsis
??? wwwroot/js/
?   ??? etudiants.js                # DataTables init + AJAX CRUD
?   ??? enseignants.js
?   ??? classes.js / matieres.js / inscriptions.js / notes.js
?   ??? emailtemplates.js           # DataTables client-side + Quill
??? appsettings.json / Development / Release
??? web.config                      # Configuration ASP.NET Core Module V2 (IIS)
??? Program.cs                      # Point d'entrée, configuration DI

src/WebApplicationForSOLID.Domain/
??? Models/                         # Etudiant, Enseignant, Classe, Matiere,
?                                   # Inscription, Note, AuditLog, EmailTemplate…
?   ??? Auth/                       # ApplicationUser, ApplicationRole, RolePermission, EmailQueue
??? Repositories/                   # IEtudiantRepository, IAuditLogRepository, IXxxRepository…

src/WebApplicationForSOLID.Application/
??? CQRS/
?   ??? Behaviors/                  # LoggingBehavior, ValidationBehavior
?   ??? Etudiants|Enseignants|…/
?       ??? Commands/ Queries/ Handlers/
??? Contracts/                      # IValidator<T>, IEmailQueueService, IPermissionService…
??? Services/                       # EtudiantService, NoteService, InscriptionService…

src/WebApplicationForSOLID.Infrastructure/
??? Data/
?   ??? ScolariteDbContext.cs       # DbContext EF Core + Audit interceptor
?   ??? DataSeeder.cs               # Seed admin, rôles, templates (idempotent)
?   ??? Configurations/             # IEntityTypeConfiguration<T> par entité
?   ??? Migrations/                 # Migrations EF Core
??? Repositories/                   # EfEtudiantRepository, EfAuditLogRepository…
??? Email/
?   ??? EfEmailQueueService.cs      # File d'attente d'e-mails en base
?   ??? SmtpEmailSender.cs          # Envoi SMTP
?   ??? EmailQueueBackgroundService.cs # Service d'arrière-plan
??? Auth/
    ??? PermissionService.cs        # Permissions par rôle

tests/ProjetScolariteSOLID.Tests/
??? Fixtures/Builders.cs            # EtudiantBuilder, NoteBuilder…
??? Validators/                     # Tests des règles de validation
??? Services/                       # Tests avec Moq (logique métier)
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
git clone https://github.com/gloomandria/WebApplicationForSOLID.git
cd WebApplicationForSOLID
```

### 2. Configurer `appsettings.json`

Renseigner la chaîne de connexion et les autres paramètres (voir [Configuration](#configuration)).

### 3. Appliquer les migrations et le seed uniquement

```bash
dotnet run --project WebApplicationForSOLID/ProjetScolariteSOLID.csproj -- --seed-only
```

Cette commande applique toutes les migrations EF Core et insère les données initiales (compte admin, rôles, templates e-mail) puis s'arrête sans démarrer le serveur web.

### 4. Démarrer l'application

```bash
dotnet run --project WebApplicationForSOLID/ProjetScolariteSOLID.csproj
```

> Les migrations et le seed sont aussi appliqués automatiquement au démarrage normal.

L'application est accessible sur `https://localhost:5001` (ou le port configuré).

### 5. Connexion initiale

| Champ | Valeur |
|---|---|
| Email | `admin@scolarite.local` (configurable) |
| Mot de passe | valeur de `AdminDefault:Password` dans `appsettings.json` |

> ?? Changez le mot de passe admin immédiatement après la première connexion.

---

## Configuration

Toutes les clés se trouvent dans `WebApplicationForSOLID/appsettings.json` :

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

L'application définit trois rôles fixes :

| Rôle | Accès |
|---|---|
| `Administrateur` | Back-office complet, gestion des utilisateurs, audit, permissions |
| `Enseignant` | Consultation des étudiants, saisie des notes |
| `Etudiant` | Consultation de son bulletin de notes |

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
2. Un **BackgroundService** (`EmailQueueBackgroundService`) tente l'envoi périodiquement via SMTP.
3. En cas d'échec, le nombre de tentatives est incrémenté et l'envoi est retenté.

Les **templates** sont éditables par l'administrateur via un éditeur WYSIWYG Quill.js (`/EmailTemplates`). Les variables sont substituées dynamiquement (ex. `{{NomComplet}}`, `{{Email}}`).

Templates livrés par défaut :
- `CompteActive` — notification d'activation de compte
- `CompteDesactive` — notification de désactivation
- `ValidationCompte` — e-mail d'activation initial avec mot de passe temporaire

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
dotnet publish WebApplicationForSOLID/ProjetScolariteSOLID.csproj -c Release -o ./publish
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
| `EtudiantValidator` | — | Instanciation directe, règles de validation |
| `NoteValidator` | — | Theory sur les bornes 0 / 20 |
| `EnseignantValidator` | — | Instanciation directe |
| `MatiereValidator` | — | Instanciation directe |
| `EtudiantService` | — | `Mock<IEtudiantRepository>` + `Mock<IValidator<Etudiant>>` |
| `EnseignantService` | — | `Mock<IEnseignantRepository>` |
| `MatiereService` | — | `Mock<IMatiereRepository>` |
| `NoteService` | — | `Mock<INoteRepository>` + `Mock<INotificationService>` |
| `InscriptionService` | — | 6 mocks — cas capacité max + statut |
| **Total** | **64** | |
