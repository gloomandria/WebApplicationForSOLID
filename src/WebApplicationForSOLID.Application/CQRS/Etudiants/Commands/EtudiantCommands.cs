using MediatR;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.CQRS.Etudiants.Commands;

public sealed record CreateEtudiantCommand(Etudiant Etudiant) : IRequest<OperationResult<Etudiant>>;
public sealed record UpdateEtudiantCommand(Etudiant Etudiant) : IRequest<OperationResult>;
public sealed record DeleteEtudiantCommand(int Id) : IRequest<OperationResult>;
