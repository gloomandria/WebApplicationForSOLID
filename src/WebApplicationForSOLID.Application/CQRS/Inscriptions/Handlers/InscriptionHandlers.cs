using MediatR;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Application.CQRS.Inscriptions.Commands;
using WebApplicationForSOLID.Application.CQRS.Inscriptions.Queries;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Inscriptions.Handlers;

public sealed class GetInscriptionsQueryHandler : IRequestHandler<GetInscriptionsQuery, PagedResult<Inscription>>
{
    private readonly IInscriptionService _service;
    public GetInscriptionsQueryHandler(IInscriptionService service) => _service = service;
    public Task<PagedResult<Inscription>> Handle(GetInscriptionsQuery request, CancellationToken ct)
        => _service.GetInscriptionsAsync(request.Page, request.PageSize, ct);
}

public sealed class GetInscriptionsByEtudiantQueryHandler : IRequestHandler<GetInscriptionsByEtudiantQuery, IReadOnlyList<Inscription>>
{
    private readonly IInscriptionService _service;
    public GetInscriptionsByEtudiantQueryHandler(IInscriptionService service) => _service = service;
    public Task<IReadOnlyList<Inscription>> Handle(GetInscriptionsByEtudiantQuery request, CancellationToken ct)
        => _service.GetByEtudiantAsync(request.EtudiantId, ct);
}

public sealed class GetInscriptionsByClasseQueryHandler : IRequestHandler<GetInscriptionsByClasseQuery, IReadOnlyList<Inscription>>
{
    private readonly IInscriptionService _service;
    public GetInscriptionsByClasseQueryHandler(IInscriptionService service) => _service = service;
    public Task<IReadOnlyList<Inscription>> Handle(GetInscriptionsByClasseQuery request, CancellationToken ct)
        => _service.GetByClasseAsync(request.ClasseId, ct);
}

public sealed class InscrireEtudiantCommandHandler : IRequestHandler<InscrireEtudiantCommand, OperationResult<Inscription>>
{
    private readonly IInscriptionService _service;
    public InscrireEtudiantCommandHandler(IInscriptionService service) => _service = service;
    public Task<OperationResult<Inscription>> Handle(InscrireEtudiantCommand request, CancellationToken ct)
        => _service.InscrireEtudiantAsync(request.EtudiantId, request.ClasseId, ct);
}

public sealed class ModifierStatutInscriptionCommandHandler : IRequestHandler<ModifierStatutInscriptionCommand, OperationResult>
{
    private readonly IInscriptionService _service;
    public ModifierStatutInscriptionCommandHandler(IInscriptionService service) => _service = service;
    public Task<OperationResult> Handle(ModifierStatutInscriptionCommand request, CancellationToken ct)
        => _service.ModifierStatutAsync(request.InscriptionId, request.Statut, ct);
}
