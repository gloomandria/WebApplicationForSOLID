using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Notes.Queries;

public sealed record GetNotesQuery(int Page, int PageSize) : IRequest<PagedResult<Note>>;
public sealed record GetNoteByIdQuery(int Id) : IRequest<Note?>;
public sealed record GetNotesByEtudiantQuery(int EtudiantId) : IRequest<IReadOnlyList<Note>>;
