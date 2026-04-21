using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Enseignants.Commands;

public sealed record CreateEnseignantCommand(Enseignant Enseignant) : IRequest<OperationResult<Enseignant>>;
public sealed record UpdateEnseignantCommand(Enseignant Enseignant) : IRequest<OperationResult>;
public sealed record DeleteEnseignantCommand(int Id) : IRequest<OperationResult>;
