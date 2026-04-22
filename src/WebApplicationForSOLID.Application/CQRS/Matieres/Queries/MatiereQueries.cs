using MediatR;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;

public sealed record GetAllMatieresQuery() : IRequest<IReadOnlyList<Matiere>>;
public sealed record GetMatiereByIdQuery(int Id) : IRequest<Matiere?>;
