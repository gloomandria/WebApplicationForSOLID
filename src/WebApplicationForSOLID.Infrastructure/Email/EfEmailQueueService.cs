using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Email;

public sealed class EfEmailQueueService : IEmailQueueService
{
    private readonly ScolariteDbContext _db;

    public EfEmailQueueService(ScolariteDbContext db) => _db = db;

    public async Task EnqueueAsync(string destinataire, string sujet, string corps, bool estHtml = true, CancellationToken ct = default)
    {
        _db.EmailQueue.Add(new EmailQueue
        {
            Destinataire = destinataire,
            Sujet        = sujet,
            Corps        = corps,
            EstHtml      = estHtml
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<EmailQueue>> GetPendingAsync(int max, CancellationToken ct = default)
        => await _db.EmailQueue
                    .Where(e => e.Statut == EmailStatut.EnAttente && e.NbTentatives < 5)
                    .OrderBy(e => e.DateCreation)
                    .Take(max)
                    .ToListAsync(ct);

    public async Task MarkSentAsync(int id, CancellationToken ct = default)
    {
        var email = await _db.EmailQueue.FindAsync([id], ct);
        if (email is null) return;
        email.Statut   = EmailStatut.Envoye;
        email.DateEnvoi = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkFailedAsync(int id, string erreur, CancellationToken ct = default)
    {
        var email = await _db.EmailQueue.FindAsync([id], ct);
        if (email is null) return;
        email.NbTentatives++;
        email.MessageErreur = erreur;
        if (email.NbTentatives >= 5) email.Statut = EmailStatut.Echoue;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<EmailQueue>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        => await _db.EmailQueue
                    .OrderByDescending(e => e.DateCreation)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await _db.EmailQueue.CountAsync(ct);
}
