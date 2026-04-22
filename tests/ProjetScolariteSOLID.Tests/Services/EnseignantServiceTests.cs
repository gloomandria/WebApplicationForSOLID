namespace ProjetScolariteSOLID.Tests.Services;

public sealed class EnseignantServiceTests
{
    private readonly Mock<IEnseignantRepository> _repoMock = new();
    private readonly Mock<IValidator<Enseignant>> _validatorMock = new();
    private readonly Mock<ILogger<EnseignantService>> _loggerMock = new();
    private readonly EnseignantService _sut;

    public EnseignantServiceTests()
    {
        _sut = new EnseignantService(_repoMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_enseignant_valide_retourne_succes()
    {
        var enseignant = EnseignantBuilder.Valide(0);
        var cree = EnseignantBuilder.Valide(10);

        _validatorMock.Setup(v => v.Validate(enseignant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.EmailExistsAsync(enseignant.Email, null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(enseignant, default)).ReturnsAsync(cree);

        var result = await _sut.CreateAsync(enseignant);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value!.Id);
    }

    [Fact]
    public async Task CreateAsync_validation_echouee_retourne_failure()
    {
        var enseignant = EnseignantBuilder.SansNom();
        var validation = new ValidationResult();
        validation.AddError("Le nom est obligatoire.");

        _validatorMock.Setup(v => v.Validate(enseignant)).Returns(validation);

        var result = await _sut.CreateAsync(enseignant);

        Assert.False(result.IsSuccess);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Enseignant>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_email_duplique_retourne_failure()
    {
        var enseignant = EnseignantBuilder.Valide(0);

        _validatorMock.Setup(v => v.Validate(enseignant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.EmailExistsAsync(enseignant.Email, null, default)).ReturnsAsync(true);

        var result = await _sut.CreateAsync(enseignant);

        Assert.False(result.IsSuccess);
        Assert.Contains("email", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_enseignant_valide_retourne_succes()
    {
        var enseignant = EnseignantBuilder.Valide(3);

        _validatorMock.Setup(v => v.Validate(enseignant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(3, default)).ReturnsAsync(true);
        _repoMock.Setup(r => r.EmailExistsAsync(enseignant.Email, 3, default)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(enseignant);

        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.UpdateAsync(enseignant, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_enseignant_inexistant_retourne_failure()
    {
        var enseignant = EnseignantBuilder.Valide(99);

        _validatorMock.Setup(v => v.Validate(enseignant)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(enseignant);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_enseignant_existant_retourne_succes()
    {
        _repoMock.Setup(r => r.ExistsAsync(2, default)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(2);

        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.DeleteAsync(2, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_enseignant_inexistant_retourne_failure()
    {
        _repoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(99);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }
}
