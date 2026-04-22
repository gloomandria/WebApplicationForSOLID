namespace ProjetScolariteSOLID.Tests.Fixtures;

/// <summary>
/// Builders pour construire des entités valides — modifiables par chaque test.
/// </summary>
public static class EtudiantBuilder
{
    public static Etudiant Valide(int id = 1) => new()
    {
        Id = id,
        Nom = "Dupont",
        Prenom = "Alice",
        Email = "alice.dupont@test.com",
        DateNaissance = new DateOnly(2000, 6, 15),
        NumeroEtudiant = "ETU-001",
        Telephone = "0600000000",
        Adresse = "1 rue de la Paix"
    };

    public static Etudiant SansNom() => new() { Id = 1, Nom = "", Prenom = "Alice", Email = "alice.dupont@test.com", DateNaissance = new DateOnly(2000, 6, 15) };
    public static Etudiant SansPrenom() => new() { Id = 1, Nom = "Dupont", Prenom = "", Email = "alice.dupont@test.com", DateNaissance = new DateOnly(2000, 6, 15) };
    public static Etudiant SansEmail() => new() { Id = 1, Nom = "Dupont", Prenom = "Alice", Email = "", DateNaissance = new DateOnly(2000, 6, 15) };
    public static Etudiant EmailInvalide() => new() { Id = 1, Nom = "Dupont", Prenom = "Alice", Email = "pas-un-email", DateNaissance = new DateOnly(2000, 6, 15) };
    public static Etudiant DateNaissanceFuture() => new() { Id = 1, Nom = "Dupont", Prenom = "Alice", Email = "alice.dupont@test.com", DateNaissance = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) };
    public static Etudiant DateNaissanceDefaut() => new() { Id = 1, Nom = "Dupont", Prenom = "Alice", Email = "alice.dupont@test.com" };
    public static Etudiant NomTropLong() => new() { Id = 1, Nom = new string('A', 101), Prenom = "Alice", Email = "alice.dupont@test.com", DateNaissance = new DateOnly(2000, 6, 15) };
}

public static class NoteBuilder
{
    public static Note Valide(int id = 0) => new()
    {
        Id = id,
        EtudiantId = 1,
        MatiereId = 1,
        Valeur = 14.5m,
        Date = DateOnly.FromDateTime(DateTime.Today),
        TypeEvaluationId = 1
    };

    public static Note ValeurNegative() => new() { Id = 0, EtudiantId = 1, MatiereId = 1, Valeur = -1m, Date = DateOnly.FromDateTime(DateTime.Today) };
    public static Note ValeurSuperieureAVingt() => new() { Id = 0, EtudiantId = 1, MatiereId = 1, Valeur = 21m, Date = DateOnly.FromDateTime(DateTime.Today) };
    public static Note SansEtudiant() => new() { Id = 0, EtudiantId = 0, MatiereId = 1, Valeur = 14m, Date = DateOnly.FromDateTime(DateTime.Today) };
    public static Note SansMatiere() => new() { Id = 0, EtudiantId = 1, MatiereId = 0, Valeur = 14m, Date = DateOnly.FromDateTime(DateTime.Today) };
    public static Note SansDate() => new() { Id = 0, EtudiantId = 1, MatiereId = 1, Valeur = 14m, Date = default };
}

public static class EnseignantBuilder
{
    public static Enseignant Valide(int id = 1) => new()
    {
        Id = id,
        Nom = "Martin",
        Prenom = "Bernard",
        Email = "b.martin@test.com",
        Matricule = "ENS-001",
        SpecialiteId = 1,
        GradeId = 1,
        Telephone = "0700000000"
    };

    public static Enseignant SansNom() => new() { Id = 1, Nom = "", Prenom = "Bernard", Email = "b.martin@test.com", Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1 };
    public static Enseignant SansPrenom() => new() { Id = 1, Nom = "Martin", Prenom = "", Email = "b.martin@test.com", Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1 };
    public static Enseignant SansEmail() => new() { Id = 1, Nom = "Martin", Prenom = "Bernard", Email = "", Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1 };
    public static Enseignant EmailInvalide() => new() { Id = 1, Nom = "Martin", Prenom = "Bernard", Email = "invalide", Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1 };
    public static Enseignant SansMatricule() => new() { Id = 1, Nom = "Martin", Prenom = "Bernard", Email = "b.martin@test.com", Matricule = "", SpecialiteId = 1, GradeId = 1 };
    public static Enseignant SansSpecialite() => new() { Id = 1, Nom = "Martin", Prenom = "Bernard", Email = "b.martin@test.com", Matricule = "ENS-001", SpecialiteId = 0, GradeId = 1 };
    public static Enseignant SansGrade() => new() { Id = 1, Nom = "Martin", Prenom = "Bernard", Email = "b.martin@test.com", Matricule = "ENS-001", SpecialiteId = 1, GradeId = 0 };
}

public static class MatiereBuilder
{
    public static Matiere Valide(int id = 1) => new()
    {
        Id = id,
        Intitule = "Algorithmique",
        Code = "ALGO-101",
        Coefficient = 3,
        VolumeHoraire = 45
    };

    public static Matiere SansIntitule() => new() { Id = 1, Intitule = "", Code = "ALGO-101", Coefficient = 3, VolumeHoraire = 45 };
    public static Matiere SansCode() => new() { Id = 1, Intitule = "Algorithmique", Code = "", Coefficient = 3, VolumeHoraire = 45 };
    public static Matiere CoefficientZero() => new() { Id = 1, Intitule = "Algorithmique", Code = "ALGO-101", Coefficient = 0, VolumeHoraire = 45 };
    public static Matiere CoefficientNegatif() => new() { Id = 1, Intitule = "Algorithmique", Code = "ALGO-101", Coefficient = -1, VolumeHoraire = 45 };
    public static Matiere VolumeHoraireNegatif() => new() { Id = 1, Intitule = "Algorithmique", Code = "ALGO-101", Coefficient = 3, VolumeHoraire = -1 };
}
