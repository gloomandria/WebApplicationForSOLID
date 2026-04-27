using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Classes.Queries;

public sealed record GetAllClassesQuery() : IRequest<IReadOnlyList<Classe>>;
public sealed record GetClassesPagedQuery(int Page, int PageSize, string Search = "", int SortCol = 0, string SortDir = "asc") : IRequest<PagedResult<Classe>>;
public sealed record GetClasseByIdQuery(int Id) : IRequest<Classe?>;
public sealed record GetMoyennesParClasseQuery() : IRequest<IReadOnlyList<MoyenneClasseDto>>;

public sealed record MoyenneClasseDto(int ClasseId, string ClasseNom, double? Moyenne);
