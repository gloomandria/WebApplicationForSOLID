using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetScolariteSOLID.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedEmailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var templates = new[]
            {
                new {
                    Code        = "CONFIRMATION_EMAIL",
                    Nom         = "Confirmation d'email",
                    Sujet       = "Confirmation de votre compte — Gestion Scolarité",
                    Description = "Envoyé lors de l'inscription pour confirmer l'adresse email.",
                    Corps       = "<h2>Bienvenue {{NomComplet}} !</h2><p>Merci de vous être inscrit sur le portail <strong>Gestion Scolarité</strong>.</p><p>Veuillez confirmer votre adresse email en cliquant sur le lien ci-dessous :</p><p><a href=\"{{Lien}}\" style=\"background:#0d6efd;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none\">Confirmer mon email</a></p><p>Ce lien est valable 24 heures.</p>"
                },
                new {
                    Code        = "RESET_MDP",
                    Nom         = "Réinitialisation de mot de passe",
                    Sujet       = "Réinitialisation de mot de passe — Gestion Scolarité",
                    Description = "Envoyé lorsque l'utilisateur demande une réinitialisation de mot de passe.",
                    Corps       = "<h2>Bonjour {{NomComplet}},</h2><p>Une demande de réinitialisation de mot de passe a été effectuée pour votre compte.</p><p><a href=\"{{Lien}}\" style=\"background:#dc3545;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none\">Réinitialiser mon mot de passe</a></p><p>Si vous n'êtes pas à l'origine de cette demande, ignorez cet email.</p>"
                },
                new {
                    Code        = "NOUVELLE_INSCRIPTION_ADMIN",
                    Nom         = "Nouvelle inscription (admin)",
                    Sujet       = "Nouvelle inscription en attente de validation",
                    Description = "Envoyé à l'admin pour signaler une nouvelle inscription en attente.",
                    Corps       = "<h2>Nouvelle inscription en attente de validation</h2><p>Un nouvel utilisateur s'est inscrit sur le portail <strong>Gestion Scolarité</strong> et attend votre validation :</p><ul><li><strong>Nom :</strong> {{NomComplet}}</li><li><strong>Email :</strong> {{Email}}</li><li><strong>Rôle demandé :</strong> {{Role}}</li></ul><p>Connectez-vous à l'interface d'administration pour activer ou refuser ce compte.</p>"
                },
                new {
                    Code        = "COMPTE_ACTIVE",
                    Nom         = "Compte activé",
                    Sujet       = "Votre compte a été activé — Gestion Scolarité",
                    Description = "Envoyé lorsque le compte est activé par un administrateur.",
                    Corps       = "<h2>Bonjour {{NomComplet}},</h2><p>Votre compte sur le portail <strong>Gestion Scolarité</strong> a été <strong>activé</strong> par un administrateur.</p><p>Vous pouvez désormais vous connecter avec vos identifiants.</p>"
                },
                new {
                    Code        = "COMPTE_DESACTIVE",
                    Nom         = "Compte désactivé",
                    Sujet       = "Votre compte a été désactivé — Gestion Scolarité",
                    Description = "Envoyé lorsque le compte est désactivé par un administrateur.",
                    Corps       = "<h2>Bonjour {{NomComplet}},</h2><p>Votre compte sur le portail <strong>Gestion Scolarité</strong> a été <strong>désactivé</strong> par un administrateur.</p><p>Si vous pensez qu'il s'agit d'une erreur, veuillez contacter l'administration.</p>"
                },
                new {
                    Code        = "VALIDATION_COMPTE",
                    Nom         = "Validation de compte (lien d'activation)",
                    Sujet       = "Activation de votre compte — Gestion Scolarité",
                    Description = "Envoyé par l'admin pour inviter l'utilisateur à définir son mot de passe.",
                    Corps       = "<h2>Bonjour {{NomComplet}},</h2><p>Un administrateur vous invite à activer votre compte sur le portail <strong>Gestion Scolarité</strong>.</p><p>Cliquez sur le lien ci-dessous pour définir votre mot de passe et activer votre compte :</p><p><a href=\"{{Lien}}\" style=\"background:#0d6efd;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none\">Activer mon compte</a></p><p>Ce lien est valable 24 heures.</p>"
                },
            };

            foreach (var t in templates)
            {
                migrationBuilder.Sql($"""
                    IF NOT EXISTS (SELECT 1 FROM [EmailTemplates] WHERE [Code] = N'{t.Code}')
                    INSERT INTO [EmailTemplates] ([Code],[Nom],[Sujet],[Description],[Corps],[EstActif],[DateCreation])
                    VALUES (N'{t.Code}', N'{t.Nom.Replace("'", "''")}', N'{t.Sujet.Replace("'", "''")}',
                            N'{t.Description.Replace("'", "''")}', N'{t.Corps.Replace("'", "''")}',
                            1, GETUTCDATE());
                    ELSE
                    UPDATE [EmailTemplates]
                    SET [Nom]=N'{t.Nom.Replace("'", "''")}', [Sujet]=N'{t.Sujet.Replace("'", "''")}',
                        [Description]=N'{t.Description.Replace("'", "''")}', [Corps]=N'{t.Corps.Replace("'", "''")}',
                        [DateModification]=GETUTCDATE()
                    WHERE [Code]=N'{t.Code}';
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [EmailTemplates]
                WHERE [Code] IN (N'CONFIRMATION_EMAIL', N'RESET_MDP', N'NOUVELLE_INSCRIPTION_ADMIN',
                                 N'COMPTE_ACTIVE', N'COMPTE_DESACTIVE', N'VALIDATION_COMPTE');
                """);
        }
    }
}
