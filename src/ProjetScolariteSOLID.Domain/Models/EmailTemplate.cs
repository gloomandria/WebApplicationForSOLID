using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

/// <summary>
/// Template d'email paramétrable depuis le backoffice.
/// Les placeholders utilisent la syntaxe {{NomVariable}}.
/// </summary>
public sealed class EmailTemplate
{
    public int Id { get; set; }

    [Display(Name = "Code")]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Display(Name = "Sujet")]
    public string Sujet { get; set; } = string.Empty;

    [Display(Name = "Corps (HTML)")]
    public string Corps { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Actif")]
    public bool EstActif { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateModification { get; set; }

    /// <summary>Applique les placeholders au sujet et au corps.</summary>
    public (string Sujet, string Corps) Appliquer(Dictionary<string, string> variables)
    {
        var sujet = Sujet;
        var corps = Corps;
        foreach (var (cle, valeur) in variables)
        {
            var placeholder = $"{{{{{cle}}}}}";
            sujet = sujet.Replace(placeholder, valeur);
            corps = corps.Replace(placeholder, valeur);
        }
        return (sujet, corps);
    }
}

/// <summary>Codes bien connus de templates email.</summary>
public static class EmailTemplateCode
{
    public const string CompteActive            = "COMPTE_ACTIVE";
    public const string CompteDesactive         = "COMPTE_DESACTIVE";
    public const string ValidationCompte        = "VALIDATION_COMPTE";
    public const string ConfirmationEmail        = "CONFIRMATION_EMAIL";
    public const string ResetMotDePasse          = "RESET_MDP";
    public const string NouvelleInscriptionAdmin = "NOUVELLE_INSCRIPTION_ADMIN";
}
