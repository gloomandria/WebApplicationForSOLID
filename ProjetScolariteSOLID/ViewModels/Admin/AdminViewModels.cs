using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.ViewModels.Admin;

public sealed class UserListViewModel
{
    public IReadOnlyList<UserRowViewModel> Users { get; set; } = [];
}

public sealed class UserRowViewModel
{
    public string Id          { get; set; } = string.Empty;
    public string NomComplet  { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Role        { get; set; } = string.Empty;
    public bool   EstActif    { get; set; }
    public bool   EmailConfirme { get; set; }
    public DateTime DateCreation { get; set; }
}

public sealed class AssignRoleViewModel
{
    public string UserId    { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string RoleActuel { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Veuillez choisir un rôle")]
    public string NouveauRole { get; set; } = string.Empty;
    public IReadOnlyList<string> RolesDisponibles { get; set; } = ApplicationRole.TousLesRoles;
}

public sealed class PermissionMatrixViewModel
{
    public IReadOnlyList<string> Ressources { get; set; } = [];
    public IReadOnlyList<RolePermissionsRow> Rows { get; set; } = [];
}

public sealed class RolePermissionsRow
{
    public string RoleId   { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public Dictionary<string, RolePermission> Permissions { get; set; } = [];
}

public sealed class EmailQueueListViewModel
{
    public IReadOnlyList<EmailQueue> Emails  { get; set; } = [];
    public int Page     { get; set; }
    public int PageSize { get; set; }
    public int Total    { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
}

public sealed class EmailTemplateListViewModel
{
    public IReadOnlyList<EmailTemplate> Templates { get; set; } = [];
}

public sealed class EmailTemplateFormModel
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le code est obligatoire")]
    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le nom est obligatoire")]
    [System.ComponentModel.DataAnnotations.MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le sujet est obligatoire")]
    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string Sujet { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le corps est obligatoire")]
    public string Corps { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string? Description { get; set; }

    public bool EstActif { get; set; } = true;
}
