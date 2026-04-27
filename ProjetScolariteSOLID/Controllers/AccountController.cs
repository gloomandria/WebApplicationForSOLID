using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.ViewModels.Auth;

namespace ProjetScolariteSOLID.Controllers;

public sealed class AccountController : Controller
{
    private readonly UserManager<ApplicationUser>   _userManager;
    private readonly SignInManager<ApplicationUser>  _signInManager;
    private readonly IEmailQueueService             _emailQueue;
    private readonly IEmailTemplateService          _emailTemplateService;
    private readonly IConfiguration                _configuration;

    public AccountController(
        UserManager<ApplicationUser>   userManager,
        SignInManager<ApplicationUser>  signInManager,
        IEmailQueueService             emailQueue,
        IEmailTemplateService          emailTemplateService,
        IConfiguration                configuration)
    {
        _userManager          = userManager;
        _signInManager        = signInManager;
        _emailQueue           = emailQueue;
        _emailTemplateService = emailTemplateService;
        _configuration        = configuration;
    }

    // ── Login ─────────────────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToLocal(returnUrl);
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var connectedUser = await _userManager.FindByEmailAsync(model.Email);
            if (connectedUser is not null && !connectedUser.EstActif)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Votre compte est en attente de validation par un administrateur.");
                return View(model);
            }
            return RedirectToLocal(model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Compte verrouillé. Réessayez plus tard.");
            return View(model);
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "Vous devez confirmer votre email avant de vous connecter.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
        return View(model);
    }

    // ── Register ──────────────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Seuls ces rôles sont autorisés à l'auto-inscription
        var rolesAutorisés = new[] { ApplicationRole.Etudiant, ApplicationRole.Enseignant, ApplicationRole.Visiteur };
        if (!rolesAutorisés.Contains(model.Role))
        {
            ModelState.AddModelError("Role", "Rôle non autorisé.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName  = model.Email,
            Email     = model.Email,
            Prenom    = model.Prenom,
            Nom       = model.Nom,
            EstActif  = false
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, model.Role);

        var token      = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token }, Request.Scheme)!;

        await EnqueueFromTemplateAsync(
            EmailTemplateCode.ConfirmationEmail,
            user.Email!,
            new() { ["NomComplet"] = user.NomComplet, ["Email"] = user.Email!, ["Lien"] = confirmUrl },
            "Confirmation de votre compte — Gestion Scolarité",
            $"""<h2>Bienvenue {user.NomComplet} !</h2><p>Confirmez votre email : <a href="{confirmUrl}">ici</a></p>""");

        var adminEmail = _configuration["AdminDefault:Email"] ?? "admin@scolarite.local";
        await EnqueueFromTemplateAsync(
            EmailTemplateCode.NouvelleInscriptionAdmin,
            adminEmail,
            new() { ["NomComplet"] = user.NomComplet, ["Email"] = user.Email!, ["Role"] = model.Role },
            "Nouvelle inscription en attente de validation",
            $"""<p>Nouvelle inscription : {user.NomComplet} ({user.Email}) — Rôle : {model.Role}</p>""");

        TempData["Success"] = "Compte créé ! Vérifiez votre boîte email pour confirmer votre inscription. Votre compte sera activé après validation par un administrateur.";
        return RedirectToAction("Login");
    }

    // ── Confirm Email ─────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            TempData["Success"] = "Email confirmé ! Vous pouvez maintenant vous connecter.";
            return RedirectToAction("Login");
        }

        ViewBag.Error = "Le lien de confirmation est invalide ou expiré.";
        return View("Error");
    }

    // ── Forgot Password ───────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
        {
            var token      = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl   = Url.Action("ResetPassword", "Account",
                                new { email = user.Email, token }, Request.Scheme)!;
            await EnqueueFromTemplateAsync(
                EmailTemplateCode.ResetMotDePasse,
                user.Email!,
                new() { ["NomComplet"] = user.NomComplet, ["Email"] = user.Email!, ["Lien"] = resetUrl },
                "Réinitialisation de mot de passe — Gestion Scolarité",
                $"""<h2>Bonjour {user.NomComplet},</h2><p>Réinitialisez votre mot de passe : <a href="{resetUrl}">ici</a></p>""");
        }

        TempData["Success"] = "Si cet email existe, un lien de réinitialisation a été envoyé.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ResetPassword(string? email, string? token)
    {
        if (email is null || token is null) return BadRequest();
        return View(new ResetPasswordViewModel { Email = email, Token = token });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            TempData["Success"] = "Mot de passe réinitialisé avec succès.";
            return RedirectToAction("Login");
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            TempData["Success"] = "Mot de passe réinitialisé avec succès.";
            return RedirectToAction("Login");
        }
        foreach (var e in result.Errors)
            ModelState.AddModelError(string.Empty, e.Description);
        return View(model);
    }

    // ── Logout ────────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    // ── Accès refusé ──────────────────────────────────────────────────────────
        // Activation de compte
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ActivateAccount(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest();
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        return View(new ActivateAccountViewModel { UserId = userId, Token = token });
    }

    [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
    public async Task<IActionResult> ActivateAccount(ActivateAccountViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null) { TempData["Error"] = "Utilisateur introuvable."; return RedirectToAction("Login"); }
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description); return View(model); }
        user.EstActif = true; user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Votre compte a été activé avec succès ! Vous pouvez maintenant vous connecter.";
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();

    // ── Helpers ───────────────────────────────────────────────────────────────
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Envoie via le template en base si disponible, sinon utilise le fallback.
    /// </summary>
    private async Task EnqueueFromTemplateAsync(
        string templateCode,
        string destinataire,
        Dictionary<string, string> variables,
        string fallbackSujet,
        string fallbackCorps,
        CancellationToken ct = default)
    {
        var template = await _emailTemplateService.GetByCodeAsync(templateCode, ct);
        if (template is not null)
        {
            var (sujet, corps) = template.Appliquer(variables);
            await _emailQueue.EnqueueAsync(destinataire, sujet, corps, ct: ct);
        }
        else
        {
            await _emailQueue.EnqueueAsync(destinataire, fallbackSujet, fallbackCorps, ct: ct);
        }
    }
}
