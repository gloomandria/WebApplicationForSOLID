using WebApplicationForSOLID.Domain.Repositories;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Services;

public sealed class MatiereService : IMatiereService
{
    private readonly IMatiereRepository _repository;
    private readonly IValidator<Matiere> _validator;
    private readonly ILogger<MatiereService> _logger;

    public MatiereService(
        IMatiereRepository repository,
        IValidator<Matiere> validator,
        ILogger<MatiereService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default)
        => _repository.GetAllWithEnseignantAsync(ct);

    public Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public async Task<OperationResult<Matiere>> CreateAsync(Matiere matiere, CancellationToken ct = default)
    {
        var validation = _validator.Validate(matiere);
        if (!validation.IsValid)
            return OperationResult<Matiere>.Failure(string.Join(" | ", validation.Errors));

        if (await _repository.CodeExistsAsync(matiere.Code, ct: ct))
            return OperationResult<Matiere>.Failure($"Le code matière '{matiere.Code}' existe déjà.");

        var created = await _repository.AddAsync(matiere, ct);
        _logger.LogInformation("Matière créée : {Id} — {Code}", created.Id, created.Code);
        return OperationResult<Matiere>.Success(created);
    }

    public async Task<OperationResult> UpdateAsync(Matiere matiere, CancellationToken ct = default)
    {
        var validation = _validator.Validate(matiere);
        if (!validation.IsValid)
            return OperationResult.Failure(string.Join(" | ", validation.Errors));

        if (!await _repository.ExistsAsync(matiere.Id, ct))
            return OperationResult.Failure($"Matière introuvable (Id={matiere.Id}).");

        if (await _repository.CodeExistsAsync(matiere.Code, matiere.Id, ct))
            return OperationResult.Failure($"Le code matière '{matiere.Code}' est déjà utilisé.");

        await _repository.UpdateAsync(matiere, ct);
        _logger.LogInformation("Matière mise à jour : {Id}", matiere.Id);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            return OperationResult.Failure($"Matière introuvable (Id={id}).");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Matière supprimée : {Id}", id);
        return OperationResult.Success();
    }
}
