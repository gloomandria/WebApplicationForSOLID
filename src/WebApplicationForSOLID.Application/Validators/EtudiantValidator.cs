using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Validators;

/// <summary>
/// SRP — Seule responsabilité : valider un étudiant.
/// OCP — Nouvelles règles ajoutables sans toucher aux services.
/// </summary>
public sealed class EtudiantValidator : IValidator<Etudiant>
{
    public ValidationResult Validate(Etudiant etudiant)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(etudiant.Nom))
            result.AddError("Le nom est obligatoire.");

        if (etudiant.Nom.Length > 100)
            result.AddError("Le nom ne doit pas dépasser 100 caractères.");

        if (string.IsNullOrWhiteSpace(etudiant.Prenom))
            result.AddError("Le prénom est obligatoire.");

        if (string.IsNullOrWhiteSpace(etudiant.Email))
            result.AddError("L'email est obligatoire.");
        else if (!etudiant.Email.Contains('@'))
            result.AddError("L'adresse email n'est pas valide.");

        if (etudiant.DateNaissance == default)
            result.AddError("La date de naissance est obligatoire.");
        else if (etudiant.DateNaissance > DateOnly.FromDateTime(DateTime.Today))
            result.AddError("La date de naissance ne peut pas être dans le futur.");

        return result;
    }
}
