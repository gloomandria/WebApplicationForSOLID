namespace WebApplicationForSOLID.Application.Contracts;

/// <summary>
/// ISP — Interface de validation découplée des services.
/// OCP — Ajout de nouvelles règles sans modifier les consommateurs.
/// </summary>
public interface IValidator<T>
{
    ValidationResult Validate(T entity);
}

public sealed class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];

    public void AddError(string message) => Errors.Add(message);
}
