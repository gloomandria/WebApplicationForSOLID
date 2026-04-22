using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.ViewModels.Auth;

public sealed class ForgotPasswordViewModel
{
    [Required, EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}

public sealed class ResetPasswordViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Nouveau mot de passe")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
    [Display(Name = "Confirmer le mot de passe")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
