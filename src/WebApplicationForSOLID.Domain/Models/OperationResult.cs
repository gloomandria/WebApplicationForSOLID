namespace WebApplicationForSOLID.Domain.Models;

/// <summary>
/// Encapsule le résultat d'une opération sans lever d'exception (pattern Result).
/// </summary>
public sealed class OperationResult
{
    public bool IsSuccess { get; private init; }
    public string? ErrorMessage { get; private init; }

    public static OperationResult Success() => new() { IsSuccess = true };
    public static OperationResult Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public sealed class OperationResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? ErrorMessage { get; private init; }

    public static OperationResult<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static OperationResult<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
