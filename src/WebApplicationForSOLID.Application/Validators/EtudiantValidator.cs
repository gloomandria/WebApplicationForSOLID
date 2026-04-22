namespace ProjetScolariteSOLID.Application.Validators;

/// <summary>
/// SRP — Seule responsabilité : valider un étudiant.
/// Nom/Prénom/Email sont désormais gérés par Identity via ApplicationUser.
/// </summary>
public sealed class EtudiantValidator : IValidator<Etudiant>
{
    public ValidationResult Validate(Etudiant etudiant)
    {
        var result = new ValidationResult();

        if (etudiant.DateNaissance == default)
            result.AddError("La date de naissance est obligatoire.");
        else if (etudiant.DateNaissance > DateOnly.FromDateTime(DateTime.Today))
            result.AddError("La date de naissance ne peut pas être dans le futur.");

        return result;
    }
}
