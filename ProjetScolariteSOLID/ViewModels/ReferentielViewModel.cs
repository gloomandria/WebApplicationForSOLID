using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

/// <summary>
/// Représente un élément de référentiel générique (Id + Libelle) pour les vues CRUD.
/// </summary>
public sealed class ReferentielItemDto
{
    public int    Id      { get; set; }
    public string Libelle { get; set; } = string.Empty;
}

public sealed class ReferentielViewModel
{
    public string                       NomReferentiel { get; init; } = string.Empty;
    public string                       Icone          { get; init; } = "📋";
    public IReadOnlyList<ReferentielItemDto> Items     { get; init; } = [];
    public ReferentielItemDto           Item           { get; init; } = new();
    public int                          ItemId         { get; init; }
    public ReferentielItemDto?          SelectedItem   { get; init; }
}
