using Microsoft.EntityFrameworkCore;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Infrastructure.Data;

/// <summary>
/// SRP — Responsabilité unique : insérer les données de référence initiales.
/// Idempotent : ne réinsère pas si les données existent déjà (safe pour re-run).
/// </summary>
public sealed class DataSeeder : IDataSeeder
{
    private readonly ScolariteDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ScolariteDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // Idempotence : on ne seed que si la base est vide
        if (await _context.Enseignants.AnyAsync(ct))
        {
            _logger.LogInformation("DataSeeder : données déjà présentes, seed ignoré.");
            return;
        }

        _logger.LogInformation("DataSeeder : insertion des données initiales...");

        await SeedEnseignantsAsync(ct);
        await SeedClassesAsync(ct);
        await SeedEtudiantsAsync(ct);
        await SeedMatieresAsync(ct);
        await SeedInscriptionsAsync(ct);
        await SeedNotesAsync(ct);

        _logger.LogInformation("DataSeeder : seed terminé avec succès.");
    }

    private async Task SeedEnseignantsAsync(CancellationToken ct)
    {
        var enseignants = new[]
        {
            new Enseignant { Matricule = "ENS0001", Nom = "Petit",  Prenom = "Jean",  Email = "j.petit@ecole.fr",  Telephone = "0611111111", Specialite = "Mathématiques", Grade = GradeEnseignant.Professeur },
            new Enseignant { Matricule = "ENS0002", Nom = "Roux",   Prenom = "Marie", Email = "m.roux@ecole.fr",   Telephone = "0622222222", Specialite = "Informatique",    Grade = GradeEnseignant.MaitreDeConferences },
            new Enseignant { Matricule = "ENS0003", Nom = "Simon",  Prenom = "Paul",  Email = "p.simon@ecole.fr",  Telephone = "0633333333", Specialite = "Physique",        Grade = GradeEnseignant.MaitreAssistant },
        };
        await _context.Enseignants.AddRangeAsync(enseignants, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedClassesAsync(CancellationToken ct)
    {
        var classes = new[]
        {
            new Classe { Nom = "L1-INFO-A", Niveau = NiveauClasse.L1, AnneeAcademique = "2024-2025", CapaciteMax = 35, Filiere = "Informatique" },
            new Classe { Nom = "L2-INFO-A", Niveau = NiveauClasse.L2, AnneeAcademique = "2024-2025", CapaciteMax = 30, Filiere = "Informatique" },
            new Classe { Nom = "L3-MATH-A", Niveau = NiveauClasse.L3, AnneeAcademique = "2024-2025", CapaciteMax = 25, Filiere = "Mathématiques" },
        };
        await _context.Classes.AddRangeAsync(classes, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedEtudiantsAsync(CancellationToken ct)
    {
        var etudiants = new[]
        {
            new Etudiant { NumeroEtudiant = "ETU0001", Nom = "Dupont",  Prenom = "Alice", Email = "alice.dupont@ecole.fr",  DateNaissance = new DateOnly(2002, 3,  15), Telephone = "0612345678", Adresse = "12 rue des Lilas, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0002", Nom = "Martin",  Prenom = "Bob",   Email = "bob.martin@ecole.fr",    DateNaissance = new DateOnly(2001, 7,  22), Telephone = "0623456789", Adresse = "5 av. Victor Hugo, Lyon" },
            new Etudiant { NumeroEtudiant = "ETU0003", Nom = "Bernard", Prenom = "Clara", Email = "clara.bernard@ecole.fr", DateNaissance = new DateOnly(2003, 1,  10), Telephone = "0634567890", Adresse = "8 bd Gambetta, Bordeaux" },
            new Etudiant { NumeroEtudiant = "ETU0004", Nom = "Leroy",   Prenom = "David", Email = "david.leroy@ecole.fr",   DateNaissance = new DateOnly(2000, 9,  5),  Telephone = "0645678901", Adresse = "23 rue Pasteur, Lille" },
            new Etudiant { NumeroEtudiant = "ETU0005", Nom = "Moreau",  Prenom = "Emma",  Email = "emma.moreau@ecole.fr",   DateNaissance = new DateOnly(2002, 11, 28), Telephone = "0656789012", Adresse = "17 rue de la Paix, Nantes" },
        };
        await _context.Etudiants.AddRangeAsync(etudiants, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedMatieresAsync(CancellationToken ct)
    {
        var enseignants = await _context.Enseignants.ToListAsync(ct);
        int idJean  = enseignants.First(e => e.Matricule == "ENS0001").Id;
        int idMarie = enseignants.First(e => e.Matricule == "ENS0002").Id;
        int idPaul  = enseignants.First(e => e.Matricule == "ENS0003").Id;

        var matieres = new[]
        {
            new Matiere { Code = "MATH101", Intitule = "Analyse mathématique",         Coefficient = 4, VolumeHoraire = 60, EnseignantId = idJean },
            new Matiere { Code = "INFO101", Intitule = "Algorithmique",                Coefficient = 3, VolumeHoraire = 45, EnseignantId = idMarie },
            new Matiere { Code = "INFO102", Intitule = "Programmation orientée objet", Coefficient = 3, VolumeHoraire = 45, EnseignantId = idMarie },
            new Matiere { Code = "PHYS101", Intitule = "Mécanique classique",          Coefficient = 3, VolumeHoraire = 45, EnseignantId = idPaul },
            new Matiere { Code = "MATH102", Intitule = "Algèbre linéaire",             Coefficient = 3, VolumeHoraire = 45, EnseignantId = idJean },
        };
        await _context.Matieres.AddRangeAsync(matieres, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedInscriptionsAsync(CancellationToken ct)
    {
        var etudiants = await _context.Etudiants.ToListAsync(ct);
        var classes   = await _context.Classes.ToListAsync(ct);

        int classeL1 = classes.First(c => c.Nom == "L1-INFO-A").Id;
        int classeL2 = classes.First(c => c.Nom == "L2-INFO-A").Id;
        int classeL3 = classes.First(c => c.Nom == "L3-MATH-A").Id;

        var inscriptions = new[]
        {
            new Inscription { EtudiantId = etudiants[0].Id, ClasseId = classeL1, Statut = StatutInscription.Active },
            new Inscription { EtudiantId = etudiants[1].Id, ClasseId = classeL1, Statut = StatutInscription.Active },
            new Inscription { EtudiantId = etudiants[2].Id, ClasseId = classeL2, Statut = StatutInscription.Active },
            new Inscription { EtudiantId = etudiants[3].Id, ClasseId = classeL2, Statut = StatutInscription.Active },
            new Inscription { EtudiantId = etudiants[4].Id, ClasseId = classeL3, Statut = StatutInscription.Active },
        };
        await _context.Inscriptions.AddRangeAsync(inscriptions, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedNotesAsync(CancellationToken ct)
    {
        var etudiants = await _context.Etudiants.ToListAsync(ct);
        var matieres  = await _context.Matieres.ToListAsync(ct);

        int e1 = etudiants[0].Id; int e2 = etudiants[1].Id;
        int e3 = etudiants[2].Id; int e4 = etudiants[3].Id; int e5 = etudiants[4].Id;
        int math101 = matieres.First(m => m.Code == "MATH101").Id;
        int info101 = matieres.First(m => m.Code == "INFO101").Id;
        int info102 = matieres.First(m => m.Code == "INFO102").Id;
        int phys101 = matieres.First(m => m.Code == "PHYS101").Id;
        int math102 = matieres.First(m => m.Code == "MATH102").Id;

        var notes = new[]
        {
            new Note { EtudiantId = e1, MatiereId = math101, Valeur = 15.5m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 15) },
            new Note { EtudiantId = e1, MatiereId = info101, Valeur = 12.0m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 16) },
            new Note { EtudiantId = e1, MatiereId = info102, Valeur = 14.0m, TypeEvaluation = TypeEvaluation.ControleContinu, Date = new DateOnly(2025, 1, 10) },
            new Note { EtudiantId = e2, MatiereId = math101, Valeur = 11.0m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 15) },
            new Note { EtudiantId = e2, MatiereId = info101, Valeur = 16.5m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 16) },
            new Note { EtudiantId = e3, MatiereId = math101, Valeur = 9.5m,  TypeEvaluation = TypeEvaluation.ExamenPartiel, Date = new DateOnly(2025, 1, 15) },
            new Note { EtudiantId = e3, MatiereId = phys101, Valeur = 18.0m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 17) },
            new Note { EtudiantId = e4, MatiereId = info101, Valeur = 13.0m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 16) },
            new Note { EtudiantId = e5, MatiereId = math102, Valeur = 17.0m, TypeEvaluation = TypeEvaluation.ExamenFinal,   Date = new DateOnly(2025, 1, 18) },
        };
        await _context.Notes.AddRangeAsync(notes, ct);
        await _context.SaveChangesAsync(ct);
    }
}
