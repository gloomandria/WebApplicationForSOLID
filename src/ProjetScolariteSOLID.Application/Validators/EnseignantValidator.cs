namespace ProjetScolariteSOLID.Application.Validators;

/// <summary>
/// SRP — Seule responsabilité : valider un enseignant.
/// Nom/Prénom/Email sont désormais gérés par Identity via ApplicationUser.
/// </summary>
public sealed class EnseignantValidator : IValidator<Enseignant>
{
    public ValidationResult Validate(Enseignant enseignant)
    {
        var result = new ValidationResult();

        if (enseignant.SpecialiteId <= 0)
            result.AddError("La spécialité est obligatoire.");

        if (enseignant.GradeId <= 0)
            result.AddError("Le grade est obligatoire.");

        return result;
    }
}
