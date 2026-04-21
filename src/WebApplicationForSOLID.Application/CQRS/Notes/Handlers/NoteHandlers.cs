using MediatR;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Application.CQRS.Notes.Commands;
using WebApplicationForSOLID.Application.CQRS.Notes.Queries;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Notes.Handlers;

public sealed class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, PagedResult<Note>>
{
    private readonly INoteService _service;
    public GetNotesQueryHandler(INoteService service) => _service = service;
    public Task<PagedResult<Note>> Handle(GetNotesQuery request, CancellationToken ct)
        => _service.GetNotesAsync(request.Page, request.PageSize, ct);
}

public sealed class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Note?>
{
    private readonly INoteService _service;
    public GetNoteByIdQueryHandler(INoteService service) => _service = service;
    public Task<Note?> Handle(GetNoteByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id, ct);
}

public sealed class GetNotesByEtudiantQueryHandler : IRequestHandler<GetNotesByEtudiantQuery, IReadOnlyList<Note>>
{
    private readonly INoteService _service;
    public GetNotesByEtudiantQueryHandler(INoteService service) => _service = service;
    public Task<IReadOnlyList<Note>> Handle(GetNotesByEtudiantQuery request, CancellationToken ct)
        => _service.GetByEtudiantAsync(request.EtudiantId, ct);
}

public sealed class AjouterNoteCommandHandler : IRequestHandler<AjouterNoteCommand, OperationResult<Note>>
{
    private readonly INoteService _service;
    public AjouterNoteCommandHandler(INoteService service) => _service = service;
    public Task<OperationResult<Note>> Handle(AjouterNoteCommand request, CancellationToken ct)
        => _service.AjouterNoteAsync(request.Note, ct);
}

public sealed class ModifierNoteCommandHandler : IRequestHandler<ModifierNoteCommand, OperationResult>
{
    private readonly INoteService _service;
    public ModifierNoteCommandHandler(INoteService service) => _service = service;
    public Task<OperationResult> Handle(ModifierNoteCommand request, CancellationToken ct)
        => _service.ModifierNoteAsync(request.Note, ct);
}

public sealed class SupprimerNoteCommandHandler : IRequestHandler<SupprimerNoteCommand, OperationResult>
{
    private readonly INoteService _service;
    public SupprimerNoteCommandHandler(INoteService service) => _service = service;
    public Task<OperationResult> Handle(SupprimerNoteCommand request, CancellationToken ct)
        => _service.SupprimerNoteAsync(request.Id, ct);
}
