using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Notes.Commands;

public sealed record AjouterNoteCommand(Note Note) : IRequest<OperationResult<Note>>;
public sealed record ModifierNoteCommand(Note Note) : IRequest<OperationResult>;
public sealed record SupprimerNoteCommand(int Id) : IRequest<OperationResult>;
