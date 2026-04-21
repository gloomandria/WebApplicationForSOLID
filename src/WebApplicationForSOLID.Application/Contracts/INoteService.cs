using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Contracts;

public interface INoteService
{
    Task<PagedResult<Note>> GetNotesAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Note?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Note>> AjouterNoteAsync(Note note, CancellationToken ct = default);
    Task<OperationResult> ModifierNoteAsync(Note note, CancellationToken ct = default);
    Task<OperationResult> SupprimerNoteAsync(int id, CancellationToken ct = default);
    Task<BulletinEtudiant?> GetBulletinAsync(int etudiantId, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default);
}
