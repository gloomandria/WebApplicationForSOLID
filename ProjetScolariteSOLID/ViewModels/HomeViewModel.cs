using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class HomeViewModel
{
    public int NombreEtudiants   { get; init; }
    public int NombreEnseignants { get; init; }
    public int NombreMatieres    { get; init; }
    public int NombreClasses     { get; init; }
    public IReadOnlyList<MoyenneClasseDto> MoyennesParClasse { get; init; } = [];
}
