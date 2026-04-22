using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data;

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
            new Enseignant { Matricule = "ENS0001", Nom = "Petit",      Prenom = "Jean",     Email = "j.petit@ecole.fr",       Telephone = "0611111111", Specialite = "Mathématiques",           Grade = GradeEnseignant.Professeur },
            new Enseignant { Matricule = "ENS0002", Nom = "Roux",       Prenom = "Marie",    Email = "m.roux@ecole.fr",        Telephone = "0622222222", Specialite = "Informatique",             Grade = GradeEnseignant.MaitreDeConferences },
            new Enseignant { Matricule = "ENS0003", Nom = "Simon",      Prenom = "Paul",     Email = "p.simon@ecole.fr",       Telephone = "0633333333", Specialite = "Physique",                 Grade = GradeEnseignant.MaitreAssistant },
            new Enseignant { Matricule = "ENS0004", Nom = "Durand",     Prenom = "Sophie",   Email = "s.durand@ecole.fr",      Telephone = "0644444444", Specialite = "Chimie",                   Grade = GradeEnseignant.Professeur },
            new Enseignant { Matricule = "ENS0005", Nom = "Gauthier",   Prenom = "Pierre",   Email = "p.gauthier@ecole.fr",    Telephone = "0655555555", Specialite = "Biologie",                 Grade = GradeEnseignant.MaitreDeConferences },
            new Enseignant { Matricule = "ENS0006", Nom = "Laurent",    Prenom = "Isabelle", Email = "i.laurent@ecole.fr",     Telephone = "0666666666", Specialite = "Français",                 Grade = GradeEnseignant.MaitreAssistant },
            new Enseignant { Matricule = "ENS0007", Nom = "Renault",    Prenom = "Marc",     Email = "m.renault@ecole.fr",     Telephone = "0677777777", Specialite = "Anglais",                  Grade = GradeEnseignant.MaitreDeConferences },
            new Enseignant { Matricule = "ENS0008", Nom = "Leclerc",    Prenom = "Christine",Email = "c.leclerc@ecole.fr",     Telephone = "0688888888", Specialite = "Histoire-Géographie",      Grade = GradeEnseignant.Professeur },
            new Enseignant { Matricule = "ENS0009", Nom = "Fontaine",   Prenom = "Georges",  Email = "g.fontaine@ecole.fr",    Telephone = "0699999999", Specialite = "Éducation Physique",       Grade = GradeEnseignant.MaitreAssistant },
            new Enseignant { Matricule = "ENS0010", Nom = "Bertrand",   Prenom = "Sylvie",   Email = "s.bertrand@ecole.fr",    Telephone = "0610101010", Specialite = "Arts plastiques",          Grade = GradeEnseignant.MaitreDeConferences },
        };
        await _context.Enseignants.AddRangeAsync(enseignants, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedClassesAsync(CancellationToken ct)
    {
        var classes = new[]
        {
            new Classe { Nom = "L1-INFO-A", Niveau = NiveauClasse.L1, AnneeAcademique = "2024-2025", CapaciteMax = 35, Filiere = "Informatique" },
            new Classe { Nom = "L1-INFO-B", Niveau = NiveauClasse.L1, AnneeAcademique = "2024-2025", CapaciteMax = 35, Filiere = "Informatique" },
            new Classe { Nom = "L1-MATH-A", Niveau = NiveauClasse.L1, AnneeAcademique = "2024-2025", CapaciteMax = 30, Filiere = "Mathématiques" },
            new Classe { Nom = "L2-INFO-A", Niveau = NiveauClasse.L2, AnneeAcademique = "2024-2025", CapaciteMax = 30, Filiere = "Informatique" },
            new Classe { Nom = "L2-INFO-B", Niveau = NiveauClasse.L2, AnneeAcademique = "2024-2025", CapaciteMax = 30, Filiere = "Informatique" },
            new Classe { Nom = "L2-MATH-A", Niveau = NiveauClasse.L2, AnneeAcademique = "2024-2025", CapaciteMax = 25, Filiere = "Mathématiques" },
            new Classe { Nom = "L3-INFO-A", Niveau = NiveauClasse.L3, AnneeAcademique = "2024-2025", CapaciteMax = 25, Filiere = "Informatique" },
            new Classe { Nom = "L3-MATH-A", Niveau = NiveauClasse.L3, AnneeAcademique = "2024-2025", CapaciteMax = 20, Filiere = "Mathématiques" },
            new Classe { Nom = "M1-INFO-A", Niveau = NiveauClasse.M1, AnneeAcademique = "2024-2025", CapaciteMax = 25, Filiere = "Informatique" },
            new Classe { Nom = "M2-INFO-A", Niveau = NiveauClasse.M2, AnneeAcademique = "2024-2025", CapaciteMax = 20, Filiere = "Informatique" },
        };
        await _context.Classes.AddRangeAsync(classes, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedEtudiantsAsync(CancellationToken ct)
    {
        var etudiants = new[]
        {
            // L1-INFO
            new Etudiant { NumeroEtudiant = "ETU0001", Nom = "Dupont",     Prenom = "Alice",    Email = "alice.dupont@ecole.fr",    DateNaissance = new DateOnly(2002, 3,  15), Telephone = "0612345678", Adresse = "12 rue des Lilas, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0002", Nom = "Martin",     Prenom = "Bob",      Email = "bob.martin@ecole.fr",      DateNaissance = new DateOnly(2001, 7,  22), Telephone = "0623456789", Adresse = "5 av. Victor Hugo, Lyon" },
            new Etudiant { NumeroEtudiant = "ETU0003", Nom = "Bernard",    Prenom = "Clara",    Email = "clara.bernard@ecole.fr",   DateNaissance = new DateOnly(2003, 1,  10), Telephone = "0634567890", Adresse = "8 bd Gambetta, Bordeaux" },
            new Etudiant { NumeroEtudiant = "ETU0004", Nom = "Leroy",      Prenom = "David",    Email = "david.leroy@ecole.fr",    DateNaissance = new DateOnly(2000, 9,  5),  Telephone = "0645678901", Adresse = "23 rue Pasteur, Lille" },
            new Etudiant { NumeroEtudiant = "ETU0005", Nom = "Moreau",     Prenom = "Emma",     Email = "emma.moreau@ecole.fr",    DateNaissance = new DateOnly(2002, 11, 28), Telephone = "0656789012", Adresse = "17 rue de la Paix, Nantes" },
            new Etudiant { NumeroEtudiant = "ETU0006", Nom = "Girard",     Prenom = "Franck",   Email = "franck.girard@ecole.fr",  DateNaissance = new DateOnly(2001, 5,  12), Telephone = "0667890123", Adresse = "34 rue Colbert, Strasbourg" },
            new Etudiant { NumeroEtudiant = "ETU0007", Nom = "Dubois",     Prenom = "Gaëlle",   Email = "gaelle.dubois@ecole.fr",  DateNaissance = new DateOnly(2003, 4,  8),  Telephone = "0678901234", Adresse = "9 av. de la République, Marseille" },
            new Etudiant { NumeroEtudiant = "ETU0008", Nom = "Noel",       Prenom = "Hervé",    Email = "herve.noel@ecole.fr",     DateNaissance = new DateOnly(2002, 8,  20), Telephone = "0689012345", Adresse = "15 bd de Belgique, Toulouse" },
            new Etudiant { NumeroEtudiant = "ETU0009", Nom = "Olivier",    Prenom = "Ingrid",   Email = "ingrid.olivier@ecole.fr", DateNaissance = new DateOnly(2001, 10, 3),  Telephone = "0690123456", Adresse = "42 rue Montgolfier, Nice" },
            new Etudiant { NumeroEtudiant = "ETU0010", Nom = "Laurent",    Prenom = "Jérôme",   Email = "jerome.laurent@ecole.fr", DateNaissance = new DateOnly(2003, 2,  25), Telephone = "0601234567", Adresse = "11 rue Saint-Michel, Nîmes" },
            // L1-MATH
            new Etudiant { NumeroEtudiant = "ETU0011", Nom = "Arnaud",     Prenom = "Katia",    Email = "katia.arnaud@ecole.fr",   DateNaissance = new DateOnly(2002, 6,  14), Telephone = "0612345670", Adresse = "28 rue Voltaire, Angers" },
            new Etudiant { NumeroEtudiant = "ETU0012", Nom = "Blanc",      Prenom = "Ludovic",  Email = "ludovic.blanc@ecole.fr",  DateNaissance = new DateOnly(2001, 12, 11), Telephone = "0623456790", Adresse = "7 bd Saint-Denis, Le Havre" },
            new Etudiant { NumeroEtudiant = "ETU0013", Nom = "Charrier",   Prenom = "Madeleine",Email = "madeleine.charrier@ecole.fr",DateNaissance = new DateOnly(2003, 3,  30), Telephone = "0634567890", Adresse = "55 av. Foch, Grenoble" },
            // L2-INFO
            new Etudiant { NumeroEtudiant = "ETU0014", Nom = "Deschamps",  Prenom = "Nathan",   Email = "nathan.deschamps@ecole.fr",DateNaissance = new DateOnly(2000, 8,  17), Telephone = "0645678901", Adresse = "21 rue de Prague, Montpellier" },
            new Etudiant { NumeroEtudiant = "ETU0015", Nom = "Emond",      Prenom = "Océane",   Email = "oceane.emond@ecole.fr",   DateNaissance = new DateOnly(2000, 11, 6),  Telephone = "0656789012", Adresse = "18 bd Michelet, Saint-Étienne" },
            new Etudiant { NumeroEtudiant = "ETU0016", Nom = "Foucault",   Prenom = "Philippe", Email = "philippe.foucault@ecole.fr",DateNaissance = new DateOnly(2000, 4,  19), Telephone = "0667890123", Adresse = "66 rue Caulaincourt, Toulouse" },
            new Etudiant { NumeroEtudiant = "ETU0017", Nom = "Gérard",     Prenom = "Quentine", Email = "quentine.gerard@ecole.fr",  DateNaissance = new DateOnly(2000, 9,  28), Telephone = "0678901234", Adresse = "3 rue du Château, Bordeaux" },
            new Etudiant { NumeroEtudiant = "ETU0018", Nom = "Henri",      Prenom = "Raphaël",  Email = "raphael.henri@ecole.fr",  DateNaissance = new DateOnly(2000, 7,  10), Telephone = "0689012345", Adresse = "99 rue Soufflot, Paris" },
            // L2-MATH
            new Etudiant { NumeroEtudiant = "ETU0019", Nom = "Izard",      Prenom = "Stéphanie",Email = "stephanie.izard@ecole.fr", DateNaissance = new DateOnly(2000, 5,  22), Telephone = "0690123456", Adresse = "44 av. Montaigne, Lyon" },
            new Etudiant { NumeroEtudiant = "ETU0020", Nom = "Jacquet",    Prenom = "Thibault", Email = "thibault.jacquet@ecole.fr",DateNaissance = new DateOnly(2000, 2,  7),  Telephone = "0601234567", Adresse = "13 rue Mouffetard, Paris" },
            // L3-INFO
            new Etudiant { NumeroEtudiant = "ETU0021", Nom = "Keller",     Prenom = "Valérie",  Email = "valerie.keller@ecole.fr", DateNaissance = new DateOnly(1999, 10, 31), Telephone = "0612345679", Adresse = "82 bd Raspail, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0022", Nom = "Leconte",    Prenom = "William",  Email = "william.leconte@ecole.fr",DateNaissance = new DateOnly(1999, 6,  14), Telephone = "0623456791", Adresse = "27 rue Dauphine, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0023", Nom = "Maillard",   Prenom = "Ximena",   Email = "ximena.maillard@ecole.fr",DateNaissance = new DateOnly(1999, 8,  23), Telephone = "0634567891", Adresse = "50 quai de la Tournelle, Paris" },
            // L3-MATH
            new Etudiant { NumeroEtudiant = "ETU0024", Nom = "Naudin",     Prenom = "Yves",     Email = "yves.naudin@ecole.fr",    DateNaissance = new DateOnly(1999, 12, 5),  Telephone = "0645678902", Adresse = "36 rue de Rivoli, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0025", Nom = "Orban",      Prenom = "Zoe",      Email = "zoe.orban@ecole.fr",      DateNaissance = new DateOnly(1999, 3,  18), Telephone = "0656789013", Adresse = "81 bd Saint-Germain, Paris" },
            // M1-INFO
            new Etudiant { NumeroEtudiant = "ETU0026", Nom = "Pichon",     Prenom = "André",    Email = "andre.pichon@ecole.fr",   DateNaissance = new DateOnly(1999, 1,  9),  Telephone = "0667890124", Adresse = "14 rue de l'Odéon, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0027", Nom = "Quantin",    Prenom = "Béatrice", Email = "beatrice.quantin@ecole.fr",DateNaissance = new DateOnly(1998, 11, 20), Telephone = "0678901235", Adresse = "57 rue Cassette, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0028", Nom = "Racine",     Prenom = "Cédric",   Email = "cedric.racine@ecole.fr",  DateNaissance = new DateOnly(1998, 9,  12), Telephone = "0689012346", Adresse = "70 rue Monsieur-le-Prince, Paris" },
            // M2-INFO
            new Etudiant { NumeroEtudiant = "ETU0029", Nom = "Saule",      Prenom = "Denise",   Email = "denise.saule@ecole.fr",   DateNaissance = new DateOnly(1998, 4,  27), Telephone = "0690123457", Adresse = "2 passage des Panoramas, Paris" },
            new Etudiant { NumeroEtudiant = "ETU0030", Nom = "Thibault",   Prenom = "Édouard",  Email = "edouard.thibault@ecole.fr",DateNaissance = new DateOnly(1998, 7,  16), Telephone = "0601234568", Adresse = "101 rue de Turenne, Paris" },
        };
        await _context.Etudiants.AddRangeAsync(etudiants, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedMatieresAsync(CancellationToken ct)
    {
        var enseignants = await _context.Enseignants.ToListAsync(ct);
        int idJean    = enseignants.First(e => e.Matricule == "ENS0001").Id;
        int idMarie   = enseignants.First(e => e.Matricule == "ENS0002").Id;
        int idPaul    = enseignants.First(e => e.Matricule == "ENS0003").Id;
        int idSophie  = enseignants.First(e => e.Matricule == "ENS0004").Id;
        int idPierre  = enseignants.First(e => e.Matricule == "ENS0005").Id;
        int idIsabelle= enseignants.First(e => e.Matricule == "ENS0006").Id;

        var matieres = new[]
        {
            // Mathématiques
            new Matiere { Code = "MATH101", Intitule = "Analyse mathématique",         Coefficient = 4, VolumeHoraire = 60, EnseignantId = idJean },
            new Matiere { Code = "MATH102", Intitule = "Algèbre linéaire",             Coefficient = 3, VolumeHoraire = 45, EnseignantId = idJean },
            new Matiere { Code = "MATH201", Intitule = "Calcul différentiel intégral",  Coefficient = 4, VolumeHoraire = 60, EnseignantId = idJean },
            new Matiere { Code = "MATH202", Intitule = "Théorie des groupes",          Coefficient = 3, VolumeHoraire = 45, EnseignantId = idJean },
            new Matiere { Code = "MATH301", Intitule = "Analyse réelle avancée",       Coefficient = 4, VolumeHoraire = 60, EnseignantId = idJean },
            // Informatique
            new Matiere { Code = "INFO101", Intitule = "Algorithmique",                Coefficient = 3, VolumeHoraire = 45, EnseignantId = idMarie },
            new Matiere { Code = "INFO102", Intitule = "Programmation orientée objet", Coefficient = 3, VolumeHoraire = 45, EnseignantId = idMarie },
            new Matiere { Code = "INFO201", Intitule = "Structures de données",        Coefficient = 3, VolumeHoraire = 45, EnseignantId = idMarie },
            new Matiere { Code = "INFO202", Intitule = "Bases de données",             Coefficient = 4, VolumeHoraire = 60, EnseignantId = idMarie },
            new Matiere { Code = "INFO301", Intitule = "Programmation Web",            Coefficient = 4, VolumeHoraire = 60, EnseignantId = idMarie },
            new Matiere { Code = "INFO302", Intitule = "Architecture Logicielle",      Coefficient = 3, VolumeHoraire = 45, EnseignantId = idMarie },
            // Physique
            new Matiere { Code = "PHYS101", Intitule = "Mécanique classique",          Coefficient = 3, VolumeHoraire = 45, EnseignantId = idPaul },
            new Matiere { Code = "PHYS102", Intitule = "Électricité et magnétisme",   Coefficient = 3, VolumeHoraire = 45, EnseignantId = idPaul },
            new Matiere { Code = "PHYS201", Intitule = "Thermodynamique",             Coefficient = 3, VolumeHoraire = 45, EnseignantId = idPaul },
            // Chimie
            new Matiere { Code = "CHIM101", Intitule = "Chimie générale",              Coefficient = 3, VolumeHoraire = 45, EnseignantId = idSophie },
            new Matiere { Code = "CHIM102", Intitule = "Chimie organique",             Coefficient = 3, VolumeHoraire = 45, EnseignantId = idSophie },
            // Biologie
            new Matiere { Code = "BIO101", Intitule = "Biologie cellulaire",          Coefficient = 3, VolumeHoraire = 45, EnseignantId = idPierre },
            // Langues
            new Matiere { Code = "LANG101", Intitule = "Anglais LV1",                 Coefficient = 2, VolumeHoraire = 30, EnseignantId = idIsabelle },
        };
        await _context.Matieres.AddRangeAsync(matieres, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedInscriptionsAsync(CancellationToken ct)
    {
        var etudiants = await _context.Etudiants.ToListAsync(ct);
        var classes   = await _context.Classes.ToListAsync(ct);

        int classeL1InfoA = classes.First(c => c.Nom == "L1-INFO-A").Id;
        int classeL1InfoB = classes.First(c => c.Nom == "L1-INFO-B").Id;
        int classeL1MathA = classes.First(c => c.Nom == "L1-MATH-A").Id;
        int classeL2InfoA = classes.First(c => c.Nom == "L2-INFO-A").Id;
        int classeL2InfoB = classes.First(c => c.Nom == "L2-INFO-B").Id;
        int classeL2MathA = classes.First(c => c.Nom == "L2-MATH-A").Id;
        int classeL3InfoA = classes.First(c => c.Nom == "L3-INFO-A").Id;
        int classeL3MathA = classes.First(c => c.Nom == "L3-MATH-A").Id;
        int classeM1InfoA = classes.First(c => c.Nom == "M1-INFO-A").Id;
        int classeM2InfoA = classes.First(c => c.Nom == "M2-INFO-A").Id;

        var inscriptions = new List<Inscription>();

        // L1-INFO-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[0].Id,  ClasseId = classeL1InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[1].Id,  ClasseId = classeL1InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[2].Id,  ClasseId = classeL1InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[3].Id,  ClasseId = classeL1InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[4].Id,  ClasseId = classeL1InfoA, Statut = StatutInscription.Active });

        // L1-INFO-B
        inscriptions.Add(new Inscription { EtudiantId = etudiants[5].Id,  ClasseId = classeL1InfoB, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[6].Id,  ClasseId = classeL1InfoB, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[7].Id,  ClasseId = classeL1InfoB, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[8].Id,  ClasseId = classeL1InfoB, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[9].Id,  ClasseId = classeL1InfoB, Statut = StatutInscription.Active });

        // L1-MATH-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[10].Id, ClasseId = classeL1MathA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[11].Id, ClasseId = classeL1MathA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[12].Id, ClasseId = classeL1MathA, Statut = StatutInscription.Active });

        // L2-INFO-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[13].Id, ClasseId = classeL2InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[14].Id, ClasseId = classeL2InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[15].Id, ClasseId = classeL2InfoA, Statut = StatutInscription.Active });

        // L2-INFO-B
        inscriptions.Add(new Inscription { EtudiantId = etudiants[16].Id, ClasseId = classeL2InfoB, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[17].Id, ClasseId = classeL2InfoB, Statut = StatutInscription.Active });

        // L2-MATH-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[18].Id, ClasseId = classeL2MathA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[19].Id, ClasseId = classeL2MathA, Statut = StatutInscription.Active });

        // L3-INFO-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[20].Id, ClasseId = classeL3InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[21].Id, ClasseId = classeL3InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[22].Id, ClasseId = classeL3InfoA, Statut = StatutInscription.Active });

        // L3-MATH-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[23].Id, ClasseId = classeL3MathA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[24].Id, ClasseId = classeL3MathA, Statut = StatutInscription.Active });

        // M1-INFO-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[25].Id, ClasseId = classeM1InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[26].Id, ClasseId = classeM1InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[27].Id, ClasseId = classeM1InfoA, Statut = StatutInscription.Active });

        // M2-INFO-A
        inscriptions.Add(new Inscription { EtudiantId = etudiants[28].Id, ClasseId = classeM2InfoA, Statut = StatutInscription.Active });
        inscriptions.Add(new Inscription { EtudiantId = etudiants[29].Id, ClasseId = classeM2InfoA, Statut = StatutInscription.Active });

        await _context.Inscriptions.AddRangeAsync(inscriptions, ct);
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedNotesAsync(CancellationToken ct)
    {
        var etudiants = await _context.Etudiants.ToListAsync(ct);
        var matieres  = await _context.Matieres.ToListAsync(ct);

        var math101 = matieres.First(m => m.Code == "MATH101").Id;
        var info101 = matieres.First(m => m.Code == "INFO101").Id;
        var info102 = matieres.First(m => m.Code == "INFO102").Id;
        var phys101 = matieres.First(m => m.Code == "PHYS101").Id;
        var math102 = matieres.First(m => m.Code == "MATH102").Id;
        var info201 = matieres.First(m => m.Code == "INFO201").Id;
        var lang101 = matieres.First(m => m.Code == "LANG101").Id;

        var notes = new List<Note>();
        var random = new Random(42);

        // Générer des notes pour les 30 étudiants sur différentes matières
        foreach (var etudiant in etudiants)
        {
            // Matières selon le niveau
            var matiereIds = new List<int> { math101, info101 };
            if (etudiant.NumeroEtudiant != null && etudiant.NumeroEtudiant.CompareTo("ETU0013") >= 0)
                matiereIds.Add(info102);
            if (etudiant.NumeroEtudiant != null && etudiant.NumeroEtudiant.CompareTo("ETU0020") >= 0)
                matiereIds.Add(info201);

            foreach (var matiereId in matiereIds)
            {
                // Examen final
                decimal note1 = (decimal)(8 + random.NextDouble() * 12);
                notes.Add(new Note
                {
                    EtudiantId = etudiant.Id,
                    MatiereId = matiereId,
                    Valeur = Math.Round(note1, 1),
                    TypeEvaluation = TypeEvaluation.ExamenFinal,
                    Date = new DateOnly(2025, 1, 15)
                });

                // Contrôle continu
                decimal note2 = (decimal)(7 + random.NextDouble() * 13);
                notes.Add(new Note
                {
                    EtudiantId = etudiant.Id,
                    MatiereId = matiereId,
                    Valeur = Math.Round(note2, 1),
                    TypeEvaluation = TypeEvaluation.ControleContinu,
                    Date = new DateOnly(2025, 1, 10)
                });
            }

            // Ajouter Anglais pour tous
            decimal noteAnglais = (decimal)(10 + random.NextDouble() * 10);
            notes.Add(new Note
            {
                EtudiantId = etudiant.Id,
                MatiereId = lang101,
                Valeur = Math.Round(noteAnglais, 1),
                TypeEvaluation = TypeEvaluation.ExamenFinal,
                Date = new DateOnly(2025, 1, 20)
            });
        }

        await _context.Notes.AddRangeAsync(notes, ct);
        await _context.SaveChangesAsync(ct);
    }
}
