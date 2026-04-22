using MediatR;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.CQRS.Classes.Commands;

public sealed record CreateClasseCommand(Classe Classe) : IRequest<OperationResult<Classe>>;
public sealed record UpdateClasseCommand(Classe Classe) : IRequest<OperationResult>;
public sealed record DeleteClasseCommand(int Id) : IRequest<OperationResult>;
