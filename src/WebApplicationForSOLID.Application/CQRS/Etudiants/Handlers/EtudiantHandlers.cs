using MediatR;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Application.CQRS.Etudiants.Commands;
using WebApplicationForSOLID.Application.CQRS.Etudiants.Queries;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Etudiants.Handlers;

public sealed class GetEtudiantsQueryHandler : IRequestHandler<GetEtudiantsQuery, PagedResult<Etudiant>>
{
    private readonly IEtudiantService _service;
    public GetEtudiantsQueryHandler(IEtudiantService service) => _service = service;
    public Task<PagedResult<Etudiant>> Handle(GetEtudiantsQuery request, CancellationToken ct)
        => _service.GetEtudiantsAsync(request.Page, request.PageSize, ct);
}

public sealed class GetAllEtudiantsQueryHandler : IRequestHandler<GetAllEtudiantsQuery, IReadOnlyList<Etudiant>>
{
    private readonly IEtudiantService _service;
    public GetAllEtudiantsQueryHandler(IEtudiantService service) => _service = service;
    public Task<IReadOnlyList<Etudiant>> Handle(GetAllEtudiantsQuery request, CancellationToken ct)
        => _service.GetAllAsync(ct);
}

public sealed class GetEtudiantByIdQueryHandler : IRequestHandler<GetEtudiantByIdQuery, Etudiant?>
{
    private readonly IEtudiantService _service;
    public GetEtudiantByIdQueryHandler(IEtudiantService service) => _service = service;
    public Task<Etudiant?> Handle(GetEtudiantByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id, ct);
}

public sealed class GetEtudiantBulletinQueryHandler : IRequestHandler<GetEtudiantBulletinQuery, BulletinEtudiant?>
{
    private readonly INoteService _noteService;
    public GetEtudiantBulletinQueryHandler(INoteService noteService) => _noteService = noteService;
    public Task<BulletinEtudiant?> Handle(GetEtudiantBulletinQuery request, CancellationToken ct)
        => _noteService.GetBulletinAsync(request.EtudiantId, ct);
}

public sealed class CreateEtudiantCommandHandler : IRequestHandler<CreateEtudiantCommand, OperationResult<Etudiant>>
{
    private readonly IEtudiantService _service;
    public CreateEtudiantCommandHandler(IEtudiantService service) => _service = service;
    public Task<OperationResult<Etudiant>> Handle(CreateEtudiantCommand request, CancellationToken ct)
        => _service.CreateAsync(request.Etudiant, ct);
}

public sealed class UpdateEtudiantCommandHandler : IRequestHandler<UpdateEtudiantCommand, OperationResult>
{
    private readonly IEtudiantService _service;
    public UpdateEtudiantCommandHandler(IEtudiantService service) => _service = service;
    public Task<OperationResult> Handle(UpdateEtudiantCommand request, CancellationToken ct)
        => _service.UpdateAsync(request.Etudiant, ct);
}

public sealed class DeleteEtudiantCommandHandler : IRequestHandler<DeleteEtudiantCommand, OperationResult>
{
    private readonly IEtudiantService _service;
    public DeleteEtudiantCommandHandler(IEtudiantService service) => _service = service;
    public Task<OperationResult> Handle(DeleteEtudiantCommand request, CancellationToken ct)
        => _service.DeleteAsync(request.Id, ct);
}
