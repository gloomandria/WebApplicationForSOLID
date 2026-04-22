using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Infrastructure.Data.Configurations;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data;

/// <summary>
/// DbContext central de l'application — applique toutes les configurations via ApplyConfigurationsFromAssembly.
/// OCP : ajouter une entité = créer une IEntityTypeConfiguration sans toucher ce fichier.
/// </summary>
public sealed class ScolariteDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    // Tables Identity exclues de l'audit
    private static readonly HashSet<string> _identityTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "AspNetUsers", "AspNetRoles", "AspNetUserRoles", "AspNetUserClaims",
        "AspNetRoleClaims", "AspNetUserLogins", "AspNetUserTokens"
    };

    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ScolariteDbContext(
        DbContextOptions<ScolariteDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Etudiant> Etudiants => Set<Etudiant>();
    public DbSet<Enseignant> Enseignants => Set<Enseignant>();
    public DbSet<Matiere> Matieres => Set<Matiere>();
    public DbSet<Classe> Classes => Set<Classe>();
    public DbSet<Inscription> Inscriptions => Set<Inscription>();
    public DbSet<Note> Notes => Set<Note>();

    // Référentiels
    public DbSet<Filiere> Filieres => Set<Filiere>();
    public DbSet<AnneeAcademique> AnneesAcademiques => Set<AnneeAcademique>();
    public DbSet<Niveau> Niveaux => Set<Niveau>();
    public DbSet<Specialite> Specialites => Set<Specialite>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<StatutInscriptionRef> StatutsInscription => Set<StatutInscriptionRef>();
    public DbSet<TypeEvaluationRef> TypesEvaluation => Set<TypeEvaluationRef>();

    // Auth
    public DbSet<EmailQueue>     EmailQueue      => Set<EmailQueue>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // OCP — Découverte automatique de toutes les IEntityTypeConfiguration<T> de l'assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EtudiantConfiguration).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = BuildAuditEntries();
        var result = await base.SaveChangesAsync(cancellationToken);

        // Pour les INSERT, la clé est disponible après SaveChanges
        foreach (var (entry, audit) in auditEntries.Where(x => x.audit.KeyValues == "{}"))
        {
            audit.KeyValues = SerializeKeys(entry);
        }

        if (auditEntries.Count > 0)
            await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    // ── Privé ────────────────────────────────────────────────────────────────────

    private List<(EntityEntry entry, AuditLog audit)> BuildAuditEntries()
    {
        ChangeTracker.DetectChanges();

        var userId = _httpContextAccessor?.HttpContext?.User
                         .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var entries = new List<(EntityEntry, AuditLog)>();

        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            var tableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name;

            if (_identityTables.Contains(tableName))
                continue;

            // Exclure AuditLog lui-même pour éviter la récursivité
            if (entry.Entity is AuditLog)
                continue;

            var audit = new AuditLog
            {
                TableName = tableName,
                Action    = entry.State switch
                {
                    EntityState.Added    => "INSERT",
                    EntityState.Modified => "UPDATE",
                    EntityState.Deleted  => "DELETE",
                    _                   => "UNKNOWN"
                },
                UserId    = userId,
                Timestamp = DateTime.UtcNow,
                KeyValues = entry.State == EntityState.Added ? "{}" : SerializeKeys(entry),
                OldValues = entry.State == EntityState.Added
                    ? null
                    : SerializeValues(entry.Properties
                        .Where(p => p.IsModified || entry.State == EntityState.Deleted)
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue)),
                NewValues = entry.State == EntityState.Deleted
                    ? null
                    : SerializeValues(entry.Properties
                        .Where(p => p.IsModified || entry.State == EntityState.Added)
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue))
            };

            AuditLogs.Add(audit);
            entries.Add((entry, audit));
        }

        return entries;
    }

    private static string SerializeKeys(EntityEntry entry)
    {
        var keys = entry.Metadata.FindPrimaryKey()?.Properties
            .ToDictionary(p => p.Name, p => entry.Property(p.Name).CurrentValue);
        return keys is null ? "{}" : JsonSerializer.Serialize(keys);
    }

    private static string? SerializeValues(Dictionary<string, object?> values)
        => values.Count == 0 ? null : JsonSerializer.Serialize(values);
}
