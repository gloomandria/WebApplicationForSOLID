using WebApplicationForSOLID.Domain.Repositories;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Services;

public sealed class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IEtudiantRepository _etudiantRepository;
    private readonly IMatiereRepository _matiereRepository;
    private readonly IValidator<Note> _validator;
    private readonly INotificationService _notificationService;
    private readonly ILogger<NoteService> _logger;

    public NoteService(
        INoteRepository noteRepository,
        IEtudiantRepository etudiantRepository,
        IMatiereRepository matiereRepository,
        IValidator<Note> validator,
        INotificationService notificationService,
        ILogger<NoteService> logger)
    {
        _noteRepository = noteRepository;
        _etudiantRepository = etudiantRepository;
        _matiereRepository = matiereRepository;
        _validator = validator;
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task<PagedResult<Note>> GetNotesAsync(int page, int pageSize, CancellationToken ct = default)
        => _noteRepository.GetPagedAsync(page, pageSize, ct);

    public Task<Note?> GetByIdAsync(int id, CancellationToken ct = default)
        => _noteRepository.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<Note>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default)
        => _noteRepository.GetByEtudiantAsync(etudiantId, ct);

    public async Task<OperationResult<Note>> AjouterNoteAsync(Note note, CancellationToken ct = default)
    {
        var validation = _validator.Validate(note);
        if (!validation.IsValid)
            return OperationResult<Note>.Failure(string.Join(" | ", validation.Errors));

        if (!await _etudiantRepository.ExistsAsync(note.EtudiantId, ct))
            return OperationResult<Note>.Failure("Étudiant introuvable.");

        if (!await _matiereRepository.ExistsAsync(note.MatiereId, ct))
            return OperationResult<Note>.Failure("Matière introuvable.");

        var created = await _noteRepository.AddAsync(note, ct);
        _logger.LogInformation("Note ajoutée : étudiant {EtudiantId}, matière {MatiereId}, valeur {Valeur}", note.EtudiantId, note.MatiereId, note.Valeur);

        var etudiant = await _etudiantRepository.GetByIdAsync(note.EtudiantId, ct);
        var matiere = await _matiereRepository.GetByIdAsync(note.MatiereId, ct);
        if (etudiant is not null && matiere is not null)
            await _notificationService.NotifyNoteAjouteeAsync(etudiant.Id, etudiant.NomComplet, matiere.Intitule, note.Valeur, ct);

        return OperationResult<Note>.Success(created);
    }

    public async Task<OperationResult> ModifierNoteAsync(Note note, CancellationToken ct = default)
    {
        var validation = _validator.Validate(note);
        if (!validation.IsValid)
            return OperationResult.Failure(string.Join(" | ", validation.Errors));

        if (!await _noteRepository.ExistsAsync(note.Id, ct))
            return OperationResult.Failure($"Note introuvable (Id={note.Id}).");

        await _noteRepository.UpdateAsync(note, ct);
        _logger.LogInformation("Note modifiée : {Id}", note.Id);
        return OperationResult.Success();
    }

    public async Task<OperationResult> SupprimerNoteAsync(int id, CancellationToken ct = default)
    {
        if (!await _noteRepository.ExistsAsync(id, ct))
            return OperationResult.Failure($"Note introuvable (Id={id}).");

        await _noteRepository.DeleteAsync(id, ct);
        _logger.LogInformation("Note supprimée : {Id}", id);
        return OperationResult.Success();
    }

    public async Task<BulletinEtudiant?> GetBulletinAsync(int etudiantId, CancellationToken ct = default)
    {
        var etudiant = await _etudiantRepository.GetByIdAsync(etudiantId, ct);
        if (etudiant is null) return null;

        var notes = await _noteRepository.GetByEtudiantAsync(etudiantId, ct);
        var matieres = await _matiereRepository.GetAllAsync(ct);

        var lignes = notes
            .GroupBy(n => n.MatiereId)
            .Select(g =>
            {
                var matiere = matieres.FirstOrDefault(m => m.Id == g.Key);
                var moyenne = g.Average(n => n.Valeur);
                return new LigneNote
                {
                    IntituleMatiere = matiere?.Intitule ?? "Inconnu",
                    Coefficient = matiere?.Coefficient ?? 1,
                    MoyenneMatiere = Math.Round(moyenne, 2)
                };
            })
            .ToList();

        decimal totalPondere = lignes.Sum(l => l.MoyennePonderee);
        int totalCoeff = lignes.Sum(l => l.Coefficient);
        decimal moyenneGenerale = totalCoeff > 0
            ? Math.Round(totalPondere / totalCoeff, 2)
            : 0;

        return new BulletinEtudiant
        {
            Etudiant = etudiant,
            Lignes = lignes,
            MoyenneGenerale = moyenneGenerale
        };
    }
}
