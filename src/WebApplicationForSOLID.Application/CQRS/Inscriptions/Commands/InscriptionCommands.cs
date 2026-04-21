using MediatR;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.CQRS.Inscriptions.Commands;

public sealed record InscrireEtudiantCommand(int EtudiantId, int ClasseId) : IRequest<OperationResult<Inscription>>;
public sealed record ModifierStatutInscriptionCommand(int InscriptionId, StatutInscription Statut) : IRequest<OperationResult>;
