using MediatR;

namespace ProjetScolariteSOLID.Application.CQRS.Inscriptions.Commands;

public sealed record InscrireEtudiantCommand(int EtudiantId, int ClasseId) : IRequest<OperationResult<Inscription>>;
public sealed record ModifierStatutInscriptionCommand(int InscriptionId, StatutInscription Statut) : IRequest<OperationResult>;
public sealed record SupprimerInscriptionCommand(int Id) : IRequest<OperationResult>;
