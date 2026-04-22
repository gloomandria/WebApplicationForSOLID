using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.ViewModels.Auth;

namespace ProjetScolariteSOLID.Controllers;

public sealed class AccountController : Controller
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailQueueService            _emailQueue;
    private readonly IConfiguration               _configuration;

    public AccountController(
        UserManager<ApplicationUser>  userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailQueueService            emailQueue,
        IConfiguration               configuration)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
        _emailQueue    = emailQueue;
        _configuration = configuration;
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

        await _emailQueue.EnqueueAsync(
            destinataire: user.Email!,
            sujet:        "Confirmation de votre compte — Gestion Scolarité",
            corps:        BuildConfirmEmailHtml(user.NomComplet, confirmUrl));

        // Notifier l'administrateur qu'un nouveau compte attend sa validation
        var adminEmail = _configuration["AdminDefault:Email"] ?? "admin@scolarite.local";
        await _emailQueue.EnqueueAsync(
            destinataire: adminEmail,
            sujet:        "Nouvelle inscription en attente de validation",
            corps:        BuildAdminValidationEmailHtml(user.NomComplet, user.Email!, model.Role));

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
            await _emailQueue.EnqueueAsync(
                destinataire: user.Email!,
                sujet:        "Réinitialisation de mot de passe — Gestion Scolarité",
                corps:        BuildResetPasswordHtml(user.NomComplet, resetUrl));
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
    public IActionResult AccessDenied() => View();

    // ── Helpers ───────────────────────────────────────────────────────────────
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    private static string BuildConfirmEmailHtml(string nom, string url) => $"""
        <h2>Bienvenue {nom} !</h2>
        <p>Merci de vous être inscrit sur le portail Gestion Scolarité.</p>
        <p>Veuillez confirmer votre adresse email en cliquant sur le lien ci-dessous :</p>
        <p><a href="{url}" style="background:#0d6efd;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none">Confirmer mon email</a></p>
        <p>Ce lien est valable 24 heures.</p>
        """;

    private static string BuildResetPasswordHtml(string nom, string url) => $"""
        <h2>Bonjour {nom},</h2>
        <p>Une demande de réinitialisation de mot de passe a été effectuée pour votre compte.</p>
        <p><a href="{url}" style="background:#dc3545;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none">Réinitialiser mon mot de passe</a></p>
        <p>Si vous n'êtes pas à l'origine de cette demande, ignorez cet email.</p>
        """;

    private static string BuildAdminValidationEmailHtml(string nomComplet, string email, string role) => $"""
        <h2>Nouvelle inscription en attente de validation</h2>
        <p>Un nouvel utilisateur s'est inscrit sur le portail Gestion Scolarité et attend votre validation :</p>
        <ul>
          <li><strong>Nom :</strong> {nomComplet}</li>
          <li><strong>Email :</strong> {email}</li>
          <li><strong>Rôle demandé :</strong> {role}</li>
        </ul>
        <p>Connectez-vous à l'interface d'administration pour activer ou refuser ce compte.</p>
        """;

    private static string BuildAccountActivatedEmailHtml(string nomComplet) => $"""
        <h2>Bonjour {nomComplet},</h2>
        <p>Votre compte sur le portail <strong>Gestion Scolarité</strong> a été <strong>activé</strong> par un administrateur.</p>
        <p>Vous pouvez désormais vous connecter avec vos identifiants.</p>
        """;
}
