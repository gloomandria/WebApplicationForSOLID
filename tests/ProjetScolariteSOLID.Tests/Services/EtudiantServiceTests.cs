namespace ProjetScolariteSOLID.Tests.Services;

public sealed class EtudiantServiceTests
{
    private readonly Mock<IEtudiantRepository> _repoMock = new();
    private readonly Mock<IValidator<Etudiant>> _validatorMock = new();
    private readonly Mock<ILogger<EtudiantService>> _loggerMock = new();
    private readonly EtudiantService _sut;

    public EtudiantServiceTests()
    {
        _sut = new EtudiantService(_repoMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_etudiant_valide_retourne_succes()
    {
        var etudiant = EtudiantBuilder.Valide(0);
        var cree = EtudiantBuilder.Valide(42);

        _validatorMock.Setup(v => v.Validate(etudiant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.EmailExistsAsync(etudiant.Email, null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(etudiant, default)).ReturnsAsync(cree);

        var result = await _sut.CreateAsync(etudiant);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value!.Id);
    }

    [Fact]
    public async Task CreateAsync_validation_echouee_retourne_failure()
    {
        var etudiant = EtudiantBuilder.SansNom();
        var validation = new ValidationResult();
        validation.AddError("Le nom est obligatoire.");

        _validatorMock.Setup(v => v.Validate(etudiant)).Returns(validation);

        var result = await _sut.CreateAsync(etudiant);

        Assert.False(result.IsSuccess);
        Assert.Contains("nom", result.ErrorMessage);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Etudiant>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_email_duplique_retourne_failure()
    {
        var etudiant = EtudiantBuilder.Valide(0);

        _validatorMock.Setup(v => v.Validate(etudiant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.EmailExistsAsync(etudiant.Email, null, default)).ReturnsAsync(true);

        var result = await _sut.CreateAsync(etudiant);

        Assert.False(result.IsSuccess);
        Assert.Contains("email", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_etudiant_valide_retourne_succes()
    {
        var etudiant = EtudiantBuilder.Valide(5);

        _validatorMock.Setup(v => v.Validate(etudiant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(5, default)).ReturnsAsync(true);
        _repoMock.Setup(r => r.EmailExistsAsync(etudiant.Email, 5, default)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(etudiant);

        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.UpdateAsync(etudiant, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_etudiant_inexistant_retourne_failure()
    {
        var etudiant = EtudiantBuilder.Valide(99);

        _validatorMock.Setup(v => v.Validate(etudiant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(etudiant);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_email_duplique_retourne_failure()
    {
        var etudiant = EtudiantBuilder.Valide(5);

        _validatorMock.Setup(v => v.Validate(etudiant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(5, default)).ReturnsAsync(true);
        _repoMock.Setup(r => r.EmailExistsAsync(etudiant.Email, 5, default)).ReturnsAsync(true);

        var result = await _sut.UpdateAsync(etudiant);

        Assert.False(result.IsSuccess);
        Assert.Contains("email", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_etudiant_existant_retourne_succes()
    {
        _repoMock.Setup(r => r.ExistsAsync(7, default)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(7);

        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.DeleteAsync(7, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_etudiant_inexistant_retourne_failure()
    {
        _repoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }
}
