using MediatR;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;

namespace ProjetScolariteSOLID.Application.CQRS.Enseignants.Handlers;

public sealed class GetEnseignantsQueryHandler : IRequestHandler<GetEnseignantsQuery, PagedResult<Enseignant>>
{
    private readonly IEnseignantService _service;
    public GetEnseignantsQueryHandler(IEnseignantService service) => _service = service;
    public Task<PagedResult<Enseignant>> Handle(GetEnseignantsQuery request, CancellationToken ct)
        => _service.GetEnseignantsAsync(request.Page, request.PageSize, ct);
}

public sealed class GetAllEnseignantsQueryHandler : IRequestHandler<GetAllEnseignantsQuery, IReadOnlyList<Enseignant>>
{
    private readonly IEnseignantService _service;
    public GetAllEnseignantsQueryHandler(IEnseignantService service) => _service = service;
    public Task<IReadOnlyList<Enseignant>> Handle(GetAllEnseignantsQuery request, CancellationToken ct)
        => _service.GetAllAsync(ct);
}

public sealed class GetEnseignantByIdQueryHandler : IRequestHandler<GetEnseignantByIdQuery, Enseignant?>
{
    private readonly IEnseignantService _service;
    public GetEnseignantByIdQueryHandler(IEnseignantService service) => _service = service;
    public Task<Enseignant?> Handle(GetEnseignantByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id, ct);
}

public sealed class CreateEnseignantCommandHandler : IRequestHandler<CreateEnseignantCommand, OperationResult<Enseignant>>
{
    private readonly IEnseignantService _service;
    public CreateEnseignantCommandHandler(IEnseignantService service) => _service = service;
    public Task<OperationResult<Enseignant>> Handle(CreateEnseignantCommand request, CancellationToken ct)
        => _service.CreateAsync(request.Enseignant, ct);
}

public sealed class UpdateEnseignantCommandHandler : IRequestHandler<UpdateEnseignantCommand, OperationResult>
{
    private readonly IEnseignantService _service;
    public UpdateEnseignantCommandHandler(IEnseignantService service) => _service = service;
    public Task<OperationResult> Handle(UpdateEnseignantCommand request, CancellationToken ct)
        => _service.UpdateAsync(request.Enseignant, ct);
}

public sealed class DeleteEnseignantCommandHandler : IRequestHandler<DeleteEnseignantCommand, OperationResult>
{
    private readonly IEnseignantService _service;
    public DeleteEnseignantCommandHandler(IEnseignantService service) => _service = service;
    public Task<OperationResult> Handle(DeleteEnseignantCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.Id, ct);
}
