using WebApplicationForSOLID.Domain.Repositories;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Services;

public sealed class ClasseService : IClasseService
{
    private readonly IClasseRepository _repository;
    private readonly ILogger<ClasseService> _logger;

    public ClasseService(IClasseRepository repository, ILogger<ClasseService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);

    public Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public async Task<OperationResult<Classe>> CreateAsync(Classe classe, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(classe.Nom))
            return OperationResult<Classe>.Failure("Le nom de la classe est obligatoire.");

        if (classe.CapaciteMax <= 0)
            return OperationResult<Classe>.Failure("La capacité maximale doit être supérieure à zéro.");

        var created = await _repository.AddAsync(classe, ct);
        _logger.LogInformation("Classe créée : {Id} — {Nom}", created.Id, created.Nom);
        return OperationResult<Classe>.Success(created);
    }

    public async Task<OperationResult> UpdateAsync(Classe classe, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(classe.Id, ct))
            return OperationResult.Failure($"Classe introuvable (Id={classe.Id}).");

        await _repository.UpdateAsync(classe, ct);
        _logger.LogInformation("Classe mise à jour : {Id}", classe.Id);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct))
            return OperationResult.Failure($"Classe introuvable (Id={id}).");

        await _repository.DeleteAsync(id, ct);
        _logger.LogInformation("Classe supprimée : {Id}", id);
        return OperationResult.Success();
    }
}
