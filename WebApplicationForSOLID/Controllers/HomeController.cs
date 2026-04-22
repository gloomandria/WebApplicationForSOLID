using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

[Authorize]
public sealed class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var etudiants   = await _mediator.Send(new GetAllEtudiantsQuery(), ct);
        var enseignants = await _mediator.Send(new GetAllEnseignantsQuery(), ct);
        var matieres    = await _mediator.Send(new GetAllMatieresQuery(), ct);
        var classes     = await _mediator.Send(new GetAllClassesQuery(), ct);
        var moyennes    = await _mediator.Send(new GetMoyennesParClasseQuery(), ct);

        return View(new HomeViewModel
        {
            NombreEtudiants   = etudiants.Count,
            NombreEnseignants = enseignants.Count,
            NombreMatieres    = matieres.Count,
            NombreClasses     = classes.Count,
            MoyennesParClasse = moyennes
        });
    }

    public IActionResult Error() => View();

    public IActionResult Privacy() => View();
}
