using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Validators;

public sealed class NoteValidator : IValidator<Note>
{
    private const decimal NoteMin = 0m;
    private const decimal NoteMax = 20m;

    public ValidationResult Validate(Note note)
    {
        var result = new ValidationResult();

        if (note.EtudiantId <= 0)
            result.AddError("L'étudiant est obligatoire.");

        if (note.MatiereId <= 0)
            result.AddError("La matière est obligatoire.");

        if (note.Valeur < NoteMin || note.Valeur > NoteMax)
            result.AddError($"La note doit être comprise entre {NoteMin} et {NoteMax}.");

        if (note.Date == default)
            result.AddError("La date de l'évaluation est obligatoire.");

        return result;
    }
}
