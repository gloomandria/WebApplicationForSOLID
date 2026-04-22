using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

/// <summary>Référentiel des filières (ex : Informatique, Mathématiques).</summary>
public sealed class Filiere
{
    public int Id { get; set; }

    [Display(Name = "Filière")]
    public string Libelle { get; set; } = string.Empty;
}

/// <summary>Référentiel des années académiques (ex : 2024-2025).</summary>
public sealed class AnneeAcademique
{
    public int Id { get; set; }

    [Display(Name = "Année académique")]
    public string Libelle { get; set; } = string.Empty;
}

/// <summary>Référentiel des niveaux de classe (ex : L1, M2, Doctorat).</summary>
public sealed class Niveau
{
    public int Id { get; set; }

    [Display(Name = "Niveau")]
    public string Libelle { get; set; } = string.Empty;
}

/// <summary>Référentiel des spécialités d'enseignants (ex : Informatique, Physique).</summary>
public sealed class Specialite
{
    public int Id { get; set; }

    [Display(Name = "Spécialité")]
    public string Libelle { get; set; } = string.Empty;
}

/// <summary>Référentiel des grades d'enseignants (ex : Professeur, Assistant).</summary>
public sealed class Grade
{
    public int Id { get; set; }

    [Display(Name = "Grade")]
    public string Libelle { get; set; } = string.Empty;
}

/// <summary>Référentiel des statuts d'inscription (ex : Active, Suspendue, Annulée).</summary>
public sealed class StatutInscriptionRef
{
    public int Id { get; set; }

    [Display(Name = "Statut")]
    public string Libelle { get; set; } = string.Empty;
}

/// <summary>Référentiel des types d'évaluation (ex : Examen final, Rattrapage).</summary>
public sealed class TypeEvaluationRef
{
    public int Id { get; set; }

    [Display(Name = "Type d'évaluation")]
    public string Libelle { get; set; } = string.Empty;
}
