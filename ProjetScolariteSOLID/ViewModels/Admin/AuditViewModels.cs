using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels.Admin;

public sealed class AuditListViewModel
{
    public IReadOnlyList<AuditLog> Logs      { get; set; } = [];
    public string?                 TableFilter { get; set; }
    public string?                 UserFilter  { get; set; }
    public int                     Page        { get; set; } = 1;
    public int                     PageSize    { get; set; } = 50;
    public int                     Total       { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    public IReadOnlyList<string>   Tables      { get; set; } = [];
}

public sealed class AuditDetailViewModel
{
    public AuditLog Log { get; set; } = new();
}
