using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Controllers.Api;

/// <summary>
/// Endpoint REST pour obtenir un JWT — utile pour les clients API (Postman, apps mobiles, etc.).
/// Route : POST /api/auth/token
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthApiController : ControllerBase
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration               _config;

    public AuthApiController(
        UserManager<ApplicationUser>  userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration               config)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
        _config        = config;
    }

    public sealed record TokenRequest(string Email, string Password);
    public sealed record TokenResponse(string Token, DateTime Expires, string Role);

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email et mot de passe requis." });

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.EstActif)
            return Unauthorized(new { error = "Identifiants invalides." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new { error = result.IsLockedOut ? "Compte verrouillé." : "Identifiants invalides." });

        var roles  = await _userManager.GetRolesAsync(user);
        var jwtCfg = _config.GetSection("Jwt");
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["Key"]!));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(jwtCfg.GetValue<int>("ExpiresInMinutes", 480));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.Name,               user.Email!),
            new("name",                        user.NomComplet)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:             jwtCfg["Issuer"],
            audience:           jwtCfg["Audience"],
            claims:             claims,
            expires:            expires,
            signingCredentials: creds);

        return Ok(new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expires,
            roles.FirstOrDefault() ?? string.Empty));
    }
}
