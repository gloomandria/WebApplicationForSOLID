using MediatR;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Application.CQRS.Classes.Commands;
using WebApplicationForSOLID.Application.CQRS.Classes.Queries;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Classes.Handlers;

public sealed class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, IReadOnlyList<Classe>>
{
    private readonly IClasseService _service;
    public GetAllClassesQueryHandler(IClasseService service) => _service = service;
    public Task<IReadOnlyList<Classe>> Handle(GetAllClassesQuery request, CancellationToken ct)
        => _service.GetAllAsync(ct);
}

public sealed class GetClasseByIdQueryHandler : IRequestHandler<GetClasseByIdQuery, Classe?>
{
    private readonly IClasseService _service;
    public GetClasseByIdQueryHandler(IClasseService service) => _service = service;
    public Task<Classe?> Handle(GetClasseByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id, ct);
}

public sealed class CreateClasseCommandHandler : IRequestHandler<CreateClasseCommand, OperationResult<Classe>>
{
    private readonly IClasseService _service;
    public CreateClasseCommandHandler(IClasseService service) => _service = service;
    public Task<OperationResult<Classe>> Handle(CreateClasseCommand request, CancellationToken ct)
        => _service.CreateAsync(request.Classe, ct);
}

public sealed class UpdateClasseCommandHandler : IRequestHandler<UpdateClasseCommand, OperationResult>
{
    private readonly IClasseService _service;
    public UpdateClasseCommandHandler(IClasseService service) => _service = service;
    public Task<OperationResult> Handle(UpdateClasseCommand request, CancellationToken ct)
        => _service.UpdateAsync(request.Classe, ct);
}

public sealed class DeleteClasseCommandHandler : IRequestHandler<DeleteClasseCommand, OperationResult>
{
    private readonly IClasseService _service;
    public DeleteClasseCommandHandler(IClasseService service) => _service = service;
    public Task<OperationResult> Handle(DeleteClasseCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.Id, ct);
}
