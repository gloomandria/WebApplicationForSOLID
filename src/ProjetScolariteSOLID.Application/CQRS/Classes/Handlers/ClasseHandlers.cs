using MediatR;
using ProjetScolariteSOLID.Application.CQRS.Classes.Commands;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;

namespace ProjetScolariteSOLID.Application.CQRS.Classes.Handlers;

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

public sealed class GetMoyennesParClasseQueryHandler : IRequestHandler<GetMoyennesParClasseQuery, IReadOnlyList<MoyenneClasseDto>>
{
    private readonly IClasseRepository _repository;
    public GetMoyennesParClasseQueryHandler(IClasseRepository repository) => _repository = repository;
    public async Task<IReadOnlyList<MoyenneClasseDto>> Handle(GetMoyennesParClasseQuery request, CancellationToken ct)
    {
        var data = await _repository.GetMoyennesParClasseAsync(ct);
        return data.Select(d => new MoyenneClasseDto(d.ClasseId, d.ClasseNom, d.Moyenne)).ToList();
    }
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
