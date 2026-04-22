using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Validators;

public sealed class MatiereValidator : IValidator<Matiere>
{
    public ValidationResult Validate(Matiere matiere)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(matiere.Intitule))
            result.AddError("L'intitulé de la matière est obligatoire.");

        if (string.IsNullOrWhiteSpace(matiere.Code))
            result.AddError("Le code de la matière est obligatoire.");

        if (matiere.Coefficient <= 0)
            result.AddError("Le coefficient doit être supérieur à zéro.");

        if (matiere.VolumeHoraire < 0)
            result.AddError("Le volume horaire ne peut pas être négatif.");

        return result;
    }
}
