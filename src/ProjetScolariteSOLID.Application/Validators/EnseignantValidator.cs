namespace ProjetScolariteSOLID.Application.Validators;

public sealed class EnseignantValidator : IValidator<Enseignant>
{
    public ValidationResult Validate(Enseignant enseignant)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(enseignant.Nom))
            result.AddError("Le nom est obligatoire.");

        if (string.IsNullOrWhiteSpace(enseignant.Prenom))
            result.AddError("Le prénom est obligatoire.");

        if (string.IsNullOrWhiteSpace(enseignant.Email))
            result.AddError("L'email est obligatoire.");
        else if (!enseignant.Email.Contains('@'))
            result.AddError("L'adresse email n'est pas valide.");

        if (string.IsNullOrWhiteSpace(enseignant.Matricule))
            result.AddError("Le matricule est obligatoire.");

        if (enseignant.SpecialiteId <= 0)
            result.AddError("La spécialité est obligatoire.");

        if (enseignant.GradeId <= 0)
            result.AddError("Le grade est obligatoire.");

        return result;
    }
}
