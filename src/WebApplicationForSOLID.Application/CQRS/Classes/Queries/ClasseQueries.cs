using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Classes.Queries;

public sealed record GetAllClassesQuery() : IRequest<IReadOnlyList<Classe>>;
public sealed record GetClasseByIdQuery(int Id) : IRequest<Classe?>;
public sealed record GetMoyennesParClasseQuery() : IRequest<IReadOnlyList<MoyenneClasseDto>>;

public sealed record MoyenneClasseDto(int ClasseId, string ClasseNom, double? Moyenne);
