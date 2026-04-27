using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;

public sealed record GetAllMatieresQuery() : IRequest<IReadOnlyList<Matiere>>;
public sealed record GetMatieresPagedQuery(int Page, int PageSize, string Search = "", int SortCol = 0, string SortDir = "asc") : IRequest<PagedResult<Matiere>>;
public sealed record GetMatiereByIdQuery(int Id) : IRequest<Matiere?>;
