using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Application.Contracts;

public interface IEmailQueueService
{
    Task EnqueueAsync(string destinataire, string sujet, string corps, bool estHtml = true, CancellationToken ct = default);
    Task<IReadOnlyList<EmailQueue>> GetPendingAsync(int max, CancellationToken ct = default);
    Task MarkSentAsync(int id, CancellationToken ct = default);
    Task MarkFailedAsync(int id, string erreur, CancellationToken ct = default);
    Task<IReadOnlyList<EmailQueue>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);

    // ── DataTable server-side ─────────────────────────────────────────────────
    Task<(IReadOnlyList<EmailQueue> Items, int FilteredTotal)> GetAllPagedAsync(
        int skip, int take,
        string search, int sortCol, string sortDir,
        CancellationToken ct = default);
}
