namespace ProjetScolariteSOLID.Tests.Services;

public sealed class NoteServiceTests
{
    private readonly Mock<INoteRepository> _noteRepoMock = new();
    private readonly Mock<IEtudiantRepository> _etudiantRepoMock = new();
    private readonly Mock<IMatiereRepository> _matiereRepoMock = new();
    private readonly Mock<IValidator<Note>> _validatorMock = new();
    private readonly Mock<INotificationService> _notifMock = new();
    private readonly Mock<ILogger<NoteService>> _loggerMock = new();
    private readonly NoteService _sut;

    public NoteServiceTests()
    {
        _sut = new NoteService(
            _noteRepoMock.Object,
            _etudiantRepoMock.Object,
            _matiereRepoMock.Object,
            _validatorMock.Object,
            _notifMock.Object,
            _loggerMock.Object);
    }

    // ── AjouterNoteAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task AjouterNoteAsync_note_valide_retourne_succes_et_notifie()
    {
        var note = NoteBuilder.Valide();
        var creee = NoteBuilder.Valide(10);
        var etudiant = EtudiantBuilder.Valide(note.EtudiantId);
        var matiere = MatiereBuilder.Valide(note.MatiereId);

        _validatorMock.Setup(v => v.Validate(note)).Returns(new ValidationResult());
        _etudiantRepoMock.Setup(r => r.ExistsAsync(note.EtudiantId, default)).ReturnsAsync(true);
        _matiereRepoMock.Setup(r => r.ExistsAsync(note.MatiereId, default)).ReturnsAsync(true);
        _noteRepoMock.Setup(r => r.AddAsync(note, default)).ReturnsAsync(creee);
        _etudiantRepoMock.Setup(r => r.GetByIdAsync(note.EtudiantId, default)).ReturnsAsync(etudiant);
        _matiereRepoMock.Setup(r => r.GetByIdAsync(note.MatiereId, default)).ReturnsAsync(matiere);

        var result = await _sut.AjouterNoteAsync(note);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value!.Id);
        _notifMock.Verify(n => n.NotifyNoteAjouteeAsync(
            etudiant.Id, etudiant.NomComplet, matiere.Intitule, note.Valeur, default), Times.Once);
    }

    [Fact]
    public async Task AjouterNoteAsync_validation_echouee_retourne_failure()
    {
        var note = NoteBuilder.ValeurNegative();
        var validation = new ValidationResult();
        validation.AddError("La note doit être comprise entre 0 et 20.");

        _validatorMock.Setup(v => v.Validate(note)).Returns(validation);

        var result = await _sut.AjouterNoteAsync(note);

        Assert.False(result.IsSuccess);
        _noteRepoMock.Verify(r => r.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AjouterNoteAsync_etudiant_inexistant_retourne_failure()
    {
        var note = NoteBuilder.Valide();

        _validatorMock.Setup(v => v.Validate(note)).Returns(new ValidationResult());
        _etudiantRepoMock.Setup(r => r.ExistsAsync(note.EtudiantId, default)).ReturnsAsync(false);

        var result = await _sut.AjouterNoteAsync(note);

        Assert.False(result.IsSuccess);
        Assert.Contains("Étudiant", result.ErrorMessage);
    }

    [Fact]
    public async Task AjouterNoteAsync_matiere_inexistante_retourne_failure()
    {
        var note = NoteBuilder.Valide();

        _validatorMock.Setup(v => v.Validate(note)).Returns(new ValidationResult());
        _etudiantRepoMock.Setup(r => r.ExistsAsync(note.EtudiantId, default)).ReturnsAsync(true);
        _matiereRepoMock.Setup(r => r.ExistsAsync(note.MatiereId, default)).ReturnsAsync(false);

        var result = await _sut.AjouterNoteAsync(note);

        Assert.False(result.IsSuccess);
        Assert.Contains("Matière", result.ErrorMessage);
    }

    // ── ModifierNoteAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task ModifierNoteAsync_note_valide_retourne_succes()
    {
        var note = NoteBuilder.Valide(7);

        _validatorMock.Setup(v => v.Validate(note)).Returns(new ValidationResult());
        _noteRepoMock.Setup(r => r.ExistsAsync(7, default)).ReturnsAsync(true);

        var result = await _sut.ModifierNoteAsync(note);

        Assert.True(result.IsSuccess);
        _noteRepoMock.Verify(r => r.UpdateAsync(note, default), Times.Once);
    }

    [Fact]
    public async Task ModifierNoteAsync_note_inexistante_retourne_failure()
    {
        var note = NoteBuilder.Valide(99);

        _validatorMock.Setup(v => v.Validate(note)).Returns(new ValidationResult());
        _noteRepoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.ModifierNoteAsync(note);

        Assert.False(result.IsSuccess);
        Assert.Contains("99", result.ErrorMessage);
    }

    // ── SupprimerNoteAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task SupprimerNoteAsync_note_existante_retourne_succes()
    {
        _noteRepoMock.Setup(r => r.ExistsAsync(3, default)).ReturnsAsync(true);

        var result = await _sut.SupprimerNoteAsync(3);

        Assert.True(result.IsSuccess);
        _noteRepoMock.Verify(r => r.DeleteAsync(3, default), Times.Once);
    }

    [Fact]
    public async Task SupprimerNoteAsync_note_inexistante_retourne_failure()
    {
        _noteRepoMock.Setup(r => r.ExistsAsync(99, default)).ReturnsAsync(false);

        var result = await _sut.SupprimerNoteAsync(99);

        Assert.False(result.IsSuccess);
    }
}
