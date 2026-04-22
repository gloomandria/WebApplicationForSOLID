using MediatR;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.CQRS.Inscriptions.Queries;

public sealed record GetInscriptionsQuery(int Page, int PageSize) : IRequest<PagedResult<Inscription>>;
public sealed record GetInscriptionByIdQuery(int Id) : IRequest<Inscription?>;
public sealed record GetInscriptionsByEtudiantQuery(int EtudiantId) : IRequest<IReadOnlyList<Inscription>>;
public sealed record GetInscriptionsByClasseQuery(int ClasseId) : IRequest<IReadOnlyList<Inscription>>;
