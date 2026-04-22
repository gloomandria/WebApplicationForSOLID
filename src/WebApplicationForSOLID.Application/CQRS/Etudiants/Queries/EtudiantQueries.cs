using MediatR;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;

public sealed record GetEtudiantsQuery(int Page, int PageSize) : IRequest<PagedResult<Etudiant>>;
public sealed record GetEtudiantByIdQuery(int Id) : IRequest<Etudiant?>;
public sealed record GetAllEtudiantsQuery() : IRequest<IReadOnlyList<Etudiant>>;
public sealed record GetEtudiantBulletinQuery(int EtudiantId) : IRequest<BulletinEtudiant?>;
