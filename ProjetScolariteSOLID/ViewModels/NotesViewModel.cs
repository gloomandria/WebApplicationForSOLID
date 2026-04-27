using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class NotesViewModel
{
    public PagedResult<Note> Notes          { get; init; } = new();
    public int               CurrentPage   { get; init; } = 1;
    public int               PageSize      { get; init; } = 10;
    public Note              Note          { get; init; } = new();
    public int               NoteId        { get; init; }
    public Note?             SelectedNote  { get; init; }
    public SelectList        EtudiantsList { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList        MatieresList  { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList        TypesEvalList { get; init; } = new SelectList(Enumerable.Empty<object>());
}
