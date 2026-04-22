using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Infrastructure.Data;
using ProjetScolariteSOLID.Infrastructure.Extensions;
using Serilog;
using System.Text;
using ProjetScolariteSOLID.Application.Extensions;
using ProjetScolariteSOLID.Web.Middleware;

// Bootstrap logger pour capturer les erreurs au démarrage avant la configuration complète
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Remplacer le logging ASP.NET Core par Serilog configuré depuis appsettings.json
builder.Host.UseSerilog((ctx, services, cfg) =>
{
    try
    {
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .Enrich.WithMachineName()
           .Enrich.WithEnvironmentName();
    }
    catch (Exception ex)
    {
        // Si le sink SQL échoue (DB inaccessible), fallback console uniquement
        cfg.WriteTo.Console()
           .Enrich.FromLogContext()
           .Enrich.WithMachineName()
           .Enrich.WithEnvironmentName();
        Console.Error.WriteLine($"Serilog SQL sink failed, falling back to console: {ex.Message}");
    }
});

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices();                          // Application : MediatR + Services + Validators
builder.Services.AddInfrastructureServices(builder.Configuration); // Infrastructure : DbContext + Repositories + Identity

// ── Cookie auth (MVC) + JWT (API) ─────────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey     = jwtSection["Key"] ?? "ChangeMe_Use_A_Real_Secret_Key_32chars!";

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan   = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSection["Issuer"] ?? "ScolariteApp",
            ValidAudience            = jwtSection["Audience"] ?? "ScolariteApp",
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app);

// Mode seed-only : migrations + seed sans démarrer le serveur web
if (args.Contains("--seed-only"))
{
    Log.Information("Mode --seed-only : migrations et seed terminés. Arrêt.");
    Log.CloseAndFlush();
    return;
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirection HTTPS uniquement si le serveur écoute sur HTTPS
var httpsPort = app.Configuration.GetValue<int?>("HTTPS_PORT") ?? app.Configuration.GetValue<int?>("ANCM_HTTPS_PORT");
if (httpsPort.HasValue)
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers(); // Pour les API controllers (JWT endpoint)

app.Run();

Log.CloseAndFlush();

static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ScolariteDbContext>();
        logger.LogInformation("Application des migrations...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations appliquées.");

        // Seed des rôles Identity
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var roleName in ApplicationRole.TousLesRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name        = roleName,
                    Description = roleName
                });
                logger.LogInformation("Rôle créé : {Role}", roleName);
            }
        }

        // Seed de l'admin par défaut
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminConfig = app.Configuration.GetSection("AdminDefault");
        var adminEmail  = adminConfig["Email"] ?? "admin@scolarite.local";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                Prenom         = "Admin",
                Nom            = "Système",
                EmailConfirmed = true,
                EstActif       = true
            };
            var adminPwd = adminConfig["Password"] ?? "Admin@12345!";
            var result   = await userManager.CreateAsync(admin, adminPwd);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, ApplicationRole.Administrateur);
                logger.LogInformation("Compte administrateur créé : {Email}", adminEmail);
            }
        }

        // Seed en Development et Release (idempotent, ne réinsère pas si données existent)
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync();

        // Seed des permissions par défaut (idempotent)
        await SeedDefaultPermissionsAsync(scope.ServiceProvider, db, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur lors de la migration ou du seed. L'application démarre sans migration.");
    }
}

static async Task SeedDefaultPermissionsAsync(
    IServiceProvider services,
    ScolariteDbContext db,
    Microsoft.Extensions.Logging.ILogger<Program> logger)
{
    if (await db.RolePermissions.AnyAsync()) return; // Idempotent

    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var ressources  = new[] { "Etudiants", "Enseignants", "Matieres", "Classes", "Inscriptions", "Notes", "Referentiels" };

    // Matrice par défaut
    // Enseignant : tout voir + éditer (sauf supprimer), pas les Référentiels
    // Etudiant   : voir Etudiants et Notes uniquement
    // Visiteur   : rien (lecture seule sur le dashboard uniquement)
    var matrix = new Dictionary<string, (bool voir, bool editer, bool suppr)[]>
    {
        [ApplicationRole.Enseignant] =
        [
            (true,  true,  false), // Etudiants
            (true,  true,  false), // Enseignants
            (true,  true,  false), // Matieres
            (true,  true,  false), // Classes
            (true,  true,  false), // Inscriptions
            (true,  true,  false), // Notes
            (false, false, false), // Referentiels
        ],
        [ApplicationRole.Etudiant] =
        [
            (true,  false, false), // Etudiants
            (false, false, false), // Enseignants
            (false, false, false), // Matieres
            (false, false, false), // Classes
            (false, false, false), // Inscriptions
            (true,  false, false), // Notes
            (false, false, false), // Referentiels
        ],
        [ApplicationRole.Visiteur] =
        [
            (false, false, false),
            (false, false, false),
            (false, false, false),
            (false, false, false),
            (false, false, false),
            (false, false, false),
            (false, false, false),
        ]
    };

    foreach (var (roleName, perms) in matrix)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) continue;
        for (int i = 0; i < ressources.Length; i++)
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleId        = role.Id,
                Ressource     = ressources[i],
                PeutVoir      = perms[i].voir,
                PeutEditer    = perms[i].editer,
                PeutSupprimer = perms[i].suppr
            });
        }
    }
    await db.SaveChangesAsync();
    logger.LogInformation("Permissions par défaut seedées.");
}
