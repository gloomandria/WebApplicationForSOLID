using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels.Admin;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = ApplicationRole.Administrateur)]
public sealed class AuditController : Controller
{
    private readonly IAuditLogRepository _auditRepo;

    public AuditController(IAuditLogRepository auditRepo) => _auditRepo = auditRepo;

    // ── Page principale (shell vide pour DataTables) ───────────────────────────
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var tables = await _auditRepo.GetDistinctTablesAsync(ct);
        return View(new AuditListViewModel { Tables = tables });
    }

    // ── Endpoint DataTables server-side ───────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> DataJson(
        int     draw         = 1,
        int     start        = 0,
        int     length       = 20,
        string  searchValue  = "",
        string? tableFilter  = null,
        string? userFilter   = null,
        int     sortCol      = 0,
        string  sortDir      = "desc",
        CancellationToken ct = default)
    {
        length = length is 10 or 20 or 50 or 100 ? length : 20;

        var (items, total) = await _auditRepo.GetPagedAsync(
            start, length, searchValue, tableFilter, userFilter, sortCol, sortDir, ct);

        var data = items.Select(a => new
        {
            id        = a.Id,
            timestamp = a.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
            tableName = a.TableName,
            action    = a.Action,
            keyValues = a.KeyValues,
            userId    = a.UserId is not null
                            ? a.UserId[..Math.Min(8, a.UserId.Length)] + "…"
                            : null
        });

        return Json(new { draw, recordsTotal = total, recordsFiltered = total, data });
    }

    // ── Détail d'un log ───────────────────────────────────────────────────────
    public async Task<IActionResult> Details(long id, CancellationToken ct)
    {
        var log = await _auditRepo.GetByIdAsync(id, ct);
        if (log is null) return NotFound();
        return View(new AuditDetailViewModel { Log = log });
    }
}

