namespace ProjetScolariteSOLID.Tests.Fixtures;

/// <summary>
/// Builders pour construire des entités valides — modifiables par chaque test.
/// </summary>
public static class EtudiantBuilder
{
    public static Etudiant Valide(int id = 1) => new()
    {
        Id = id,
        DateNaissance = new DateOnly(2000, 6, 15),
        NumeroEtudiant = "ETU-001",
        Adresse = "1 rue de la Paix",
        UserId = "user-test-id"
    };

    public static Etudiant SansNom() => new() { Id = 1, DateNaissance = new DateOnly(2000, 6, 15), UserId = "user-test-id" };
    public static Etudiant SansPrenom() => new() { Id = 1, DateNaissance = new DateOnly(2000, 6, 15), UserId = "user-test-id" };
    public static Etudiant SansEmail() => new() { Id = 1, DateNaissance = new DateOnly(2000, 6, 15), UserId = "user-test-id" };
    public static Etudiant EmailInvalide() => new() { Id = 1, DateNaissance = new DateOnly(2000, 6, 15), UserId = "user-test-id" };
    public static Etudiant DateNaissanceFuture() => new() { Id = 1, DateNaissance = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), UserId = "user-test-id" };
    public static Etudiant DateNaissanceDefaut() => new() { Id = 1, UserId = "user-test-id" };
    public static Etudiant NomTropLong() => new() { Id = 1, DateNaissance = new DateOnly(2000, 6, 15), UserId = "user-test-id" };
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
        Matricule = "ENS-001",
        SpecialiteId = 1,
        GradeId = 1,
        UserId = "user-ens-test-id"
    };

    public static Enseignant SansNom() => new() { Id = 1, Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1, UserId = "user-ens-test-id" };
    public static Enseignant SansPrenom() => new() { Id = 1, Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1, UserId = "user-ens-test-id" };
    public static Enseignant SansEmail() => new() { Id = 1, Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1, UserId = "user-ens-test-id" };
    public static Enseignant EmailInvalide() => new() { Id = 1, Matricule = "ENS-001", SpecialiteId = 1, GradeId = 1, UserId = "user-ens-test-id" };
    public static Enseignant SansMatricule() => new() { Id = 1, Matricule = "", SpecialiteId = 1, GradeId = 1, UserId = "user-ens-test-id" };
    public static Enseignant SansSpecialite() => new() { Id = 1, Matricule = "ENS-001", SpecialiteId = 0, GradeId = 1, UserId = "user-ens-test-id" };
    public static Enseignant SansGrade() => new() { Id = 1, Matricule = "ENS-001", SpecialiteId = 1, GradeId = 0, UserId = "user-ens-test-id" };
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
