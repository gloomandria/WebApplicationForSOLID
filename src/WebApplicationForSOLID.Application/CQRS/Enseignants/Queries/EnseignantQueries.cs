using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;

public sealed record GetEnseignantsQuery(int Page, int PageSize) : IRequest<PagedResult<Enseignant>>;
public sealed record GetEnseignantByIdQuery(int Id) : IRequest<Enseignant?>;
public sealed record GetAllEnseignantsQuery() : IRequest<IReadOnlyList<Enseignant>>;
