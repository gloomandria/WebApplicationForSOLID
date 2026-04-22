namespace ProjetScolariteSOLID.Tests.Services;

public sealed class MatiereServiceTests
{
    private readonly Mock<IMatiereRepository> _repoMock = new();
    private readonly Mock<IValidator<Matiere>> _validatorMock = new();
    private readonly Mock<ILogger<MatiereService>> _loggerMock = new();
    private readonly MatiereService _sut;

    public MatiereServiceTests()
    {
        _sut = new MatiereService(_repoMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_matiere_valide_retourne_succes()
    {
        var matiere = MatiereBuilder.Valide(0);
        var creee = MatiereBuilder.Valide(5);

        _validatorMock.Setup(v => v.Validate(matiere)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.CodeExistsAsync(matiere.Code, null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(matiere, default)).ReturnsAsync(creee);

        var result = await _sut.CreateAsync(matiere);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.Id);
    }

    [Fact]
    public async Task CreateAsync_validation_echouee_retourne_failure()
    {
        var matiere = MatiereBuilder.SansIntitule();
        var validation = new ValidationResult();
        validation.AddError("L'intitulé de la matière est obligatoire.");

        _validatorMock.Setup(v => v.Validate(matiere)).Returns(validation);

        var result = await _sut.CreateAsync(matiere);

        Assert.False(result.IsSuccess);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Matiere>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_code_duplique_retourne_failure()
    {
        var matiere = MatiereBuilder.Valide(0);

        _validatorMock.Setup(v => v.Validate(matiere)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.CodeExistsAsync(matiere.Code, null, default)).ReturnsAsync(true);

        var result = await _sut.CreateAsync(matiere);

        Assert.False(result.IsSuccess);
        Assert.Contains(matiere.Code, result.ErrorMessage);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_matiere_valide_retourne_succes()
    {
        var matiere = MatiereBuilder.Valide(4);

        _validatorMock.Setup(v => v.Validate(matiere)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(4, default)).ReturnsAsync(true);
        _repoMock.Setup(r => r.CodeExistsAsync(matiere.Code, 4, default)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(matiere);

        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.UpdateAsync(matiere, default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_matiere_inexistante_retourne_failure()
    {
        var matiere = MatiereBuilder.Valide(99);

        _validatorMock.Setup(v => v.Validate(matiere)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.UpdateAsync(matiere);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_code_duplique_retourne_failure()
    {
        var matiere = MatiereBuilder.Valide(4);

        _validatorMock.Setup(v => v.Validate(matiere)).Returns(new ValidationResult());
        _repoMock.Setup(r => r.ExistsAsync(4, default)).ReturnsAsync(true);
        _repoMock.Setup(r => r.CodeExistsAsync(matiere.Code, 4, default)).ReturnsAsync(true);

        var result = await _sut.UpdateAsync(matiere);

        Assert.False(result.IsSuccess);
        Assert.Contains(matiere.Code, result.ErrorMessage);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_matiere_existante_retourne_succes()
    {
        _repoMock.Setup(r => r.ExistsAsync(3, default)).ReturnsAsync(true);

        var result = await _sut.DeleteAsync(3);

        Assert.True(result.IsSuccess);
        _repoMock.Verify(r => r.DeleteAsync(3, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_matiere_inexistante_retourne_failure()
    {
        _repoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(99);

        Assert.False(result.IsSuccess);
    }
}
