using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Contracts;

public interface IEmailTemplateService
{
    Task<IReadOnlyList<EmailTemplate>> GetAllAsync(CancellationToken ct = default);
    Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmailTemplate?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task CreateAsync(EmailTemplate template, CancellationToken ct = default);
    Task UpdateAsync(EmailTemplate template, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
