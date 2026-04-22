namespace ProjetScolariteSOLID.Domain.Models;

/// <summary>
/// Résumé académique d'un étudiant : moyennes pondérées par matière.
/// </summary>
public sealed class BulletinEtudiant
{
    public Etudiant Etudiant { get; init; } = default!;
    public IReadOnlyList<LigneNote> Lignes { get; init; } = [];
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
    public string IntituleMatiere { get; init; } = string.Empty;
    public int Coefficient { get; init; }
    public decimal MoyenneMatiere { get; init; }
    public decimal MoyennePonderee => MoyenneMatiere * Coefficient;
}
