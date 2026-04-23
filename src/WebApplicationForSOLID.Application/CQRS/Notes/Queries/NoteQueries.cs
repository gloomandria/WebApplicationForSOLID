using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Notes.Queries;

public sealed record GetNotesQuery(int Page, int PageSize, string Search = "", int SortCol = 0, string SortDir = "asc") : IRequest<PagedResult<Note>>;
public sealed record GetNoteByIdQuery(int Id) : IRequest<Note?>;
public sealed record GetNotesByEtudiantQuery(int EtudiantId) : IRequest<IReadOnlyList<Note>>;
