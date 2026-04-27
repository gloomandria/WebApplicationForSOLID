using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;

public sealed record GetEtudiantsQuery(int Page, int PageSize, string Search = "", int SortCol = 0, string SortDir = "asc") : IRequest<PagedResult<Etudiant>>;
public sealed record GetEtudiantByIdQuery(int Id) : IRequest<Etudiant?>;
public sealed record GetAllEtudiantsQuery() : IRequest<IReadOnlyList<Etudiant>>;
public sealed record GetEtudiantBulletinQuery(int EtudiantId) : IRequest<BulletinEtudiant?>;
