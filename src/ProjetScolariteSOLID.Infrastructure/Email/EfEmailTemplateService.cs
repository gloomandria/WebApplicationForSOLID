using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Email;

public sealed class EfEmailTemplateService : IEmailTemplateService
{
    private readonly ScolariteDbContext _db;

    public EfEmailTemplateService(ScolariteDbContext db) => _db = db;

    public async Task<IReadOnlyList<EmailTemplate>> GetAllAsync(CancellationToken ct = default)
        => await _db.EmailTemplates.OrderBy(t => t.Nom).ToListAsync(ct);

    public async Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.EmailTemplates.FindAsync([id], ct);

    public async Task<EmailTemplate?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.EmailTemplates.FirstOrDefaultAsync(t => t.Code == code && t.EstActif, ct);

    public async Task CreateAsync(EmailTemplate template, CancellationToken ct = default)
    {
        _db.EmailTemplates.Add(template);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(EmailTemplate template, CancellationToken ct = default)
    {
        template.DateModification = DateTime.UtcNow;
        _db.EmailTemplates.Update(template);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var template = await _db.EmailTemplates.FindAsync([id], ct);
        if (template is not null)
        {
            _db.EmailTemplates.Remove(template);
            await _db.SaveChangesAsync(ct);
        }
    }
}
