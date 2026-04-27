namespace ProjetScolariteSOLID.Infrastructure.Data;

/// <summary>
/// DIP — Program.cs dépend de cette abstraction, pas de DataSeeder directement.
/// </summary>
public interface IDataSeeder
{
    Task SeedAsync(CancellationToken ct = default);
}
