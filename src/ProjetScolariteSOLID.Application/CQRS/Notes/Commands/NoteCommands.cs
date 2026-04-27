using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Notes.Commands;

public sealed record AjouterNoteCommand(Note Note) : IRequest<OperationResult<Note>>;
public sealed record ModifierNoteCommand(Note Note) : IRequest<OperationResult>;
public sealed record SupprimerNoteCommand(int Id) : IRequest<OperationResult>;
