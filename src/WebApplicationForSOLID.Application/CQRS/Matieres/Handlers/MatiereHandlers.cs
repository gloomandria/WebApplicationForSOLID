using MediatR;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Application.CQRS.Matieres.Commands;
using WebApplicationForSOLID.Application.CQRS.Matieres.Queries;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Matieres.Handlers;

public sealed class GetAllMatieresQueryHandler : IRequestHandler<GetAllMatieresQuery, IReadOnlyList<Matiere>>
{
    private readonly IMatiereService _service;
    public GetAllMatieresQueryHandler(IMatiereService service) => _service = service;
    public Task<IReadOnlyList<Matiere>> Handle(GetAllMatieresQuery request, CancellationToken ct)
        => _service.GetAllAsync(ct);
}

public sealed class GetMatiereByIdQueryHandler : IRequestHandler<GetMatiereByIdQuery, Matiere?>
{
    private readonly IMatiereService _service;
    public GetMatiereByIdQueryHandler(IMatiereService service) => _service = service;
    public Task<Matiere?> Handle(GetMatiereByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id, ct);
}

public sealed class CreateMatiereCommandHandler : IRequestHandler<CreateMatiereCommand, OperationResult<Matiere>>
{
    private readonly IMatiereService _service;
    public CreateMatiereCommandHandler(IMatiereService service) => _service = service;
    public Task<OperationResult<Matiere>> Handle(CreateMatiereCommand request, CancellationToken ct)
        => _service.CreateAsync(request.Matiere, ct);
}

public sealed class UpdateMatiereCommandHandler : IRequestHandler<UpdateMatiereCommand, OperationResult>
{
    private readonly IMatiereService _service;
    public UpdateMatiereCommandHandler(IMatiereService service) => _service = service;
    public Task<OperationResult> Handle(UpdateMatiereCommand request, CancellationToken ct)
        => _service.UpdateAsync(request.Matiere, ct);
}

public sealed class DeleteMatiereCommandHandler : IRequestHandler<DeleteMatiereCommand, OperationResult>
{
    private readonly IMatiereService _service;
    public DeleteMatiereCommandHandler(IMatiereService service) => _service = service;
    public Task<OperationResult> Handle(DeleteMatiereCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.Id, ct);
}
