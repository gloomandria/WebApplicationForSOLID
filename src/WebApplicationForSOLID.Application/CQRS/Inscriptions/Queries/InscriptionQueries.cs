using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Inscriptions.Queries;

public sealed record GetInscriptionsQuery(int Page, int PageSize) : IRequest<PagedResult<Inscription>>;
public sealed record GetInscriptionsByEtudiantQuery(int EtudiantId) : IRequest<IReadOnlyList<Inscription>>;
public sealed record GetInscriptionsByClasseQuery(int ClasseId) : IRequest<IReadOnlyList<Inscription>>;
