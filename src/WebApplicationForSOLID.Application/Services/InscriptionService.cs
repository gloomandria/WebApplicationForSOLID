namespace ProjetScolariteSOLID.Application.Services;

public sealed class InscriptionService : IInscriptionService
{
    private readonly IInscriptionRepository _inscriptionRepository;
    private readonly IEtudiantRepository _etudiantRepository;
    private readonly IClasseRepository _classeRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<InscriptionService> _logger;

    public InscriptionService(
        IInscriptionRepository inscriptionRepository,
        IEtudiantRepository etudiantRepository,
        IClasseRepository classeRepository,
        INotificationService notificationService,
        ILogger<InscriptionService> logger)
    {
        _inscriptionRepository = inscriptionRepository;
        _etudiantRepository = etudiantRepository;
        _classeRepository = classeRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task<PagedResult<Inscription>> GetInscriptionsAsync(int page, int pageSize, CancellationToken ct = default)
        => _inscriptionRepository.GetPagedAsync(page, pageSize, ct);

    public Task<Inscription?> GetByIdAsync(int id, CancellationToken ct = default)
        => _inscriptionRepository.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<Inscription>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default)
        => _inscriptionRepository.GetByEtudiantAsync(etudiantId, ct);

    public Task<IReadOnlyList<Inscription>> GetByClasseAsync(int classeId, CancellationToken ct = default)
        => _inscriptionRepository.GetByClasseAsync(classeId, ct);

    public async Task<OperationResult<Inscription>> InscrireEtudiantAsync(int etudiantId, int classeId, CancellationToken ct = default)
    {
        var etudiant = await _etudiantRepository.GetByIdAsync(etudiantId, ct);
        if (etudiant is null)
            return OperationResult<Inscription>.Failure("Étudiant introuvable.");

        var classe = await _classeRepository.GetByIdAsync(classeId, ct);
        if (classe is null)
            return OperationResult<Inscription>.Failure("Classe introuvable.");

        if (await _inscriptionRepository.ExistsAsync(etudiantId, classeId, ct))
            return OperationResult<Inscription>.Failure($"{etudiant.NomComplet} est déjà inscrit dans cette classe.");

        var nbEtudiants = await _classeRepository.GetNombreEtudiantsAsync(classeId, ct);
        if (nbEtudiants >= classe.CapaciteMax)
            return OperationResult<Inscription>.Failure($"La classe {classe.Nom} a atteint sa capacité maximale ({classe.CapaciteMax} étudiants).");

        var inscription = new Inscription
        {
            EtudiantId = etudiantId,
            ClasseId = classeId,
            Statut = StatutInscription.Active
        };

        var created = await _inscriptionRepository.AddAsync(inscription, ct);
        _logger.LogInformation("Inscription créée : étudiant {EtudiantId} → classe {ClasseId}", etudiantId, classeId);

        await _notificationService.NotifyInscriptionAsync(etudiant.Id, etudiant.NomComplet, classe.Nom, ct);

        return OperationResult<Inscription>.Success(created);
    }

    public async Task<OperationResult> ModifierStatutAsync(int inscriptionId, StatutInscription statut, CancellationToken ct = default)
    {
        var inscription = await _inscriptionRepository.GetByIdAsync(inscriptionId, ct);
        if (inscription is null)
            return OperationResult.Failure("Inscription introuvable.");

        inscription.Statut = statut;
        await _inscriptionRepository.UpdateAsync(inscription, ct);
        _logger.LogInformation("Statut inscription {Id} modifié : {Statut}", inscriptionId, statut);
        return OperationResult.Success();
    }

    public async Task<OperationResult> SupprimerAsync(int id, CancellationToken ct = default)
    {
        var inscription = await _inscriptionRepository.GetByIdAsync(id, ct);
        if (inscription is null)
            return OperationResult.Failure($"Inscription introuvable (Id={id}).");

        await _inscriptionRepository.DeleteAsync(id, ct);
        _logger.LogInformation("Inscription supprimée : {Id}", id);
        return OperationResult.Success();
    }
}
