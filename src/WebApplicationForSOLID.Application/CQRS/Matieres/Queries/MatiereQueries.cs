using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Matieres.Queries;

public sealed record GetAllMatieresQuery() : IRequest<IReadOnlyList<Matiere>>;
public sealed record GetMatiereByIdQuery(int Id) : IRequest<Matiere?>;
