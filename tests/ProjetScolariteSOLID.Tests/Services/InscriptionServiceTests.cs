namespace ProjetScolariteSOLID.Tests.Services;

public sealed class InscriptionServiceTests
{
    private readonly Mock<IInscriptionRepository> _inscriptionRepoMock = new();
    private readonly Mock<IEtudiantRepository> _etudiantRepoMock = new();
    private readonly Mock<IClasseRepository> _classeRepoMock = new();
    private readonly Mock<IReferentielRepository<StatutInscriptionRef>> _statutRepoMock = new();
    private readonly Mock<INotificationService> _notifMock = new();
    private readonly Mock<ILogger<InscriptionService>> _loggerMock = new();
    private readonly InscriptionService _sut;

    private static readonly StatutInscriptionRef StatutActive = new() { Id = 1, Libelle = "Active" };

    public InscriptionServiceTests()
    {
        _sut = new InscriptionService(
            _inscriptionRepoMock.Object,
            _etudiantRepoMock.Object,
            _classeRepoMock.Object,
            _statutRepoMock.Object,
            _notifMock.Object,
            _loggerMock.Object);
    }

    private void SetupStatutActive()
        => _statutRepoMock.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync(new List<StatutInscriptionRef> { StatutActive });

    // ── InscrireEtudiantAsync ────────────────────────────────────────────────

    [Fact]
    public async Task InscrireEtudiantAsync_succes_retourne_inscription()
    {
        var etudiant = EtudiantBuilder.Valide(1);
        var classe = new Classe { Id = 2, Nom = "L1-INFO", CapaciteMax = 30 };
        var inscriptionCreee = new Inscription { Id = 10, EtudiantId = 1, ClasseId = 2, StatutId = 1 };

        _etudiantRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(etudiant);
        _classeRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(classe);
        _inscriptionRepoMock.Setup(r => r.ExistsAsync(1, 2, default)).ReturnsAsync(false);
        _classeRepoMock.Setup(r => r.GetNombreEtudiantsAsync(2, default)).ReturnsAsync(10);
        SetupStatutActive();
        _inscriptionRepoMock.Setup(r => r.AddAsync(It.IsAny<Inscription>(), default)).ReturnsAsync(inscriptionCreee);

        var result = await _sut.InscrireEtudiantAsync(1, 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value!.Id);
        _notifMock.Verify(n => n.NotifyInscriptionAsync(etudiant.Id, etudiant.NomComplet, classe.Nom, default), Times.Once);
    }

    [Fact]
    public async Task InscrireEtudiantAsync_etudiant_inexistant_retourne_failure()
    {
        _etudiantRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Etudiant?)null);

        var result = await _sut.InscrireEtudiantAsync(99, 1);

        Assert.False(result.IsSuccess);
        Assert.Contains("Étudiant", result.ErrorMessage);
    }

    [Fact]
    public async Task InscrireEtudiantAsync_classe_inexistante_retourne_failure()
    {
        _etudiantRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(EtudiantBuilder.Valide(1));
        _classeRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Classe?)null);

        var result = await _sut.InscrireEtudiantAsync(1, 99);

        Assert.False(result.IsSuccess);
        Assert.Contains("Classe", result.ErrorMessage);
    }

    [Fact]
    public async Task InscrireEtudiantAsync_deja_inscrit_retourne_failure()
    {
        var etudiant = EtudiantBuilder.Valide(1);
        var classe = new Classe { Id = 2, Nom = "L1-INFO", CapaciteMax = 30 };

        _etudiantRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(etudiant);
        _classeRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(classe);
        _inscriptionRepoMock.Setup(r => r.ExistsAsync(1, 2, default)).ReturnsAsync(true);

        var result = await _sut.InscrireEtudiantAsync(1, 2);

        Assert.False(result.IsSuccess);
        Assert.Contains(etudiant.NomComplet, result.ErrorMessage);
    }

    [Fact]
    public async Task InscrireEtudiantAsync_capacite_max_atteinte_retourne_failure()
    {
        var etudiant = EtudiantBuilder.Valide(1);
        var classe = new Classe { Id = 2, Nom = "L1-INFO", CapaciteMax = 5 };

        _etudiantRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(etudiant);
        _classeRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(classe);
        _inscriptionRepoMock.Setup(r => r.ExistsAsync(1, 2, default)).ReturnsAsync(false);
        _classeRepoMock.Setup(r => r.GetNombreEtudiantsAsync(2, default)).ReturnsAsync(5);

        var result = await _sut.InscrireEtudiantAsync(1, 2);

        Assert.False(result.IsSuccess);
        Assert.Contains("capacité", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InscrireEtudiantAsync_statut_active_absent_leve_exception()
    {
        var etudiant = EtudiantBuilder.Valide(1);
        var classe = new Classe { Id = 2, Nom = "L1-INFO", CapaciteMax = 30 };

        _etudiantRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(etudiant);
        _classeRepoMock.Setup(r => r.GetByIdAsync(2, default)).ReturnsAsync(classe);
        _inscriptionRepoMock.Setup(r => r.ExistsAsync(1, 2, default)).ReturnsAsync(false);
        _classeRepoMock.Setup(r => r.GetNombreEtudiantsAsync(2, default)).ReturnsAsync(0);
        _statutRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(new List<StatutInscriptionRef>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.InscrireEtudiantAsync(1, 2));
    }

    // ── ModifierStatutAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task ModifierStatutAsync_inscription_existante_retourne_succes()
    {
        var inscription = new Inscription { Id = 5, EtudiantId = 1, ClasseId = 2, StatutId = 1 };

        _inscriptionRepoMock.Setup(r => r.GetByIdAsync(5, default)).ReturnsAsync(inscription);

        var result = await _sut.ModifierStatutAsync(5, 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, inscription.StatutId);
    }

    [Fact]
    public async Task ModifierStatutAsync_inscription_inexistante_retourne_failure()
    {
        _inscriptionRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Inscription?)null);

        var result = await _sut.ModifierStatutAsync(99, 2);

        Assert.False(result.IsSuccess);
        Assert.Contains("Inscription", result.ErrorMessage);
    }

    // ── SupprimerAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task SupprimerAsync_inscription_existante_retourne_succes()
    {
        var inscription = new Inscription { Id = 8, EtudiantId = 1, ClasseId = 2, StatutId = 1 };

        _inscriptionRepoMock.Setup(r => r.GetByIdAsync(8, default)).ReturnsAsync(inscription);

        var result = await _sut.SupprimerAsync(8);

        Assert.True(result.IsSuccess);
        _inscriptionRepoMock.Verify(r => r.DeleteAsync(8, default), Times.Once);
    }

    [Fact]
    public async Task SupprimerAsync_inscription_inexistante_retourne_failure()
    {
        _inscriptionRepoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Inscription?)null);

        var result = await _sut.SupprimerAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }
}
