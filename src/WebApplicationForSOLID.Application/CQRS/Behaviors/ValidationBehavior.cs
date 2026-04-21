using MediatR;
using WebApplicationForSOLID.Application.Contracts;

namespace WebApplicationForSOLID.Application.CQRS.Behaviors;

/// <summary>
/// SRP — Cross-cutting concern : validation automatique des Commands via IValidator&lt;T&gt;.
/// OCP — Fonctionne pour toute nouvelle Command sans modification.
/// Seules les Commands exposant une propriété typée sont validées.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Seules les Commands (non Queries) passent par la validation
        var requestName = typeof(TRequest).Name;
        if (!requestName.EndsWith("Command", StringComparison.Ordinal))
            return await next();

        // Cherche un IValidator<T> pour la propriété principale de la Command
        var entityProp = typeof(TRequest)
            .GetProperties()
            .FirstOrDefault(p => !p.PropertyType.IsPrimitive && p.PropertyType != typeof(string));

        if (entityProp is null)
            return await next();

        var validatorType = typeof(IValidator<>).MakeGenericType(entityProp.PropertyType);
        var validator = _serviceProvider.GetService(validatorType);

        if (validator is null)
            return await next();

        var validateMethod = validatorType.GetMethod(nameof(IValidator<object>.Validate))!;
        var entity = entityProp.GetValue(request);
        var result = (ValidationResult)validateMethod.Invoke(validator, [entity])!;

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return await next();
    }
}

/// <summary>Exception levée par le ValidationBehavior en cas d'échec.</summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IReadOnlyList<string> errors)
        : base($"Validation échouée : {string.Join(" | ", errors)}")
        => Errors = errors;
}
