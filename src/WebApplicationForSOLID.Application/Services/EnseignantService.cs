namespace ProjetScolariteSOLID.Application.Services;

public sealed class EnseignantService : IEnseignantService
{
    private readonly IEnseignantRepository _repository;
    private readonly IValidator<Enseignant> _validator;
    private readonly ILogger<EnseignantService> _logger;

    public EnseignantService(
        IEnseignantRepository repository,
        IValidator<Enseignant> validator,
        ILogger<EnseignantService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public Task<PagedResult<Enseignant>> GetEnseignantsAsync(int page, int pageSize, CancellationToken ct = default)
        => _repository.GetPagedAsync(page, pageSize, ct);

    public Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);

    public Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public async Task<OperationResult<Enseignant>> CreateAsync(Enseignant enseignant, CancellationToken ct = default)
    {
        var validation = _validator.Validate(enseignant);
        if (!validation.IsValid)
            return OperationResult<Enseignant>.Failure(string.Join(" | ", validation.Errors));

        if (await _repository.EmailExistsAsync(enseignant.Email, ct: ct))
            return OperationResult<Enseignant>.Failure("Cet email est déjà utilisé.");

        var created = await _repository.AddAsync(enseignant, ct);
        _logger.LogInformation("Enseignant créé : {Id} — {NomComplet}", created.Id, created.NomComplet);
        return OperationResult<Enseignant>.Success(created);
    }

    public async Task<OperationResult> UpdateAsync(Enseignant enseignant, CancellationToken ct = default)
    {
        var validation = _validator.Validate(enseignant);
        if (!validation.IsValid)
            return OperationResult.Failure(string.Join(" | ", validation.Errors));

        if (!await _repository.ExistsAsync(enseignant.Id, ct))
            return OperationResult.Failure($"Enseignant introuvable (Id={enseignant.Id}).");

        if (await _repository.EmailExistsAsync(enseignant.Email, enseignant.Id, ct))
            return OperationResult.Failure("Cet email est déjà utilisé.");

        await _repository.UpdateAsync(enseignant, ct);
        _logger.LogInformation("Enseignant mis à jour : {Id}", enseignant.Id);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            return OperationResult.Failure($"Enseignant introuvable (Id={id}).");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Enseignant supprimé : {Id}", id);
        return OperationResult.Success();
    }
}
