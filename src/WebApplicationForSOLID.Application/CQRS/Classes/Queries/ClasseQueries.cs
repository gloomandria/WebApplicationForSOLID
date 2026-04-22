using MediatR;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.CQRS.Classes.Queries;

public sealed record GetAllClassesQuery() : IRequest<IReadOnlyList<Classe>>;
public sealed record GetClasseByIdQuery(int Id) : IRequest<Classe?>;
