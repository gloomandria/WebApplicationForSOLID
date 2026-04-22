namespace ProjetScolariteSOLID.Application.Services;

/// <summary>
/// SRP — Orchestre uniquement la logique métier des étudiants.
/// DIP — Dépend des abstractions, jamais des implémentations concrètes.
/// </summary>
public sealed class EtudiantService : IEtudiantService
{
    private readonly IEtudiantRepository _repository;
    private readonly IValidator<Etudiant> _validator;
    private readonly ILogger<EtudiantService> _logger;

    public EtudiantService(
        IEtudiantRepository repository,
        IValidator<Etudiant> validator,
        ILogger<EtudiantService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public Task<PagedResult<Etudiant>> GetEtudiantsAsync(int page, int pageSize, CancellationToken ct = default)
        => _repository.GetPagedAsync(page, pageSize, ct);

    public Task<IReadOnlyList<Etudiant>> GetAllAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);

    public Task<Etudiant?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public async Task<OperationResult<Etudiant>> CreateAsync(Etudiant etudiant, CancellationToken ct = default)
    {
        var validation = _validator.Validate(etudiant);
        if (!validation.IsValid)
            return OperationResult<Etudiant>.Failure(string.Join(" | ", validation.Errors));

        if (await _repository.EmailExistsAsync(etudiant.Email, ct: ct))
            return OperationResult<Etudiant>.Failure("Cet email est déjà utilisé par un autre étudiant.");

        var created = await _repository.AddAsync(etudiant, ct);
        _logger.LogInformation("Étudiant créé : {Id} — {NomComplet}", created.Id, created.NomComplet);
        return OperationResult<Etudiant>.Success(created);
    }

    public async Task<OperationResult> UpdateAsync(Etudiant etudiant, CancellationToken ct = default)
    {
        var validation = _validator.Validate(etudiant);
        if (!validation.IsValid)
            return OperationResult.Failure(string.Join(" | ", validation.Errors));

        if (!await _repository.ExistsAsync(etudiant.Id, ct))
            return OperationResult.Failure($"Étudiant introuvable (Id={etudiant.Id}).");

        if (await _repository.EmailExistsAsync(etudiant.Email, etudiant.Id, ct))
            return OperationResult.Failure("Cet email est déjà utilisé par un autre étudiant.");

        await _repository.UpdateAsync(etudiant, ct);
        _logger.LogInformation("Étudiant mis à jour : {Id}", etudiant.Id);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            return OperationResult.Failure($"Étudiant introuvable (Id={id}).");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Étudiant supprimé : {Id}", id);
        return OperationResult.Success();
    }
}
