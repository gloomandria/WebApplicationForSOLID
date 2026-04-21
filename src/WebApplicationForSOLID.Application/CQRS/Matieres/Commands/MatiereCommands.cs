using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Matieres.Commands;

public sealed record CreateMatiereCommand(Matiere Matiere) : IRequest<OperationResult<Matiere>>;
public sealed record UpdateMatiereCommand(Matiere Matiere) : IRequest<OperationResult>;
public sealed record DeleteMatiereCommand(int Id) : IRequest<OperationResult>;
