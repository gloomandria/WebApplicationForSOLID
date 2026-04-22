namespace ProjetScolariteSOLID.Tests.Validators;

public sealed class NoteValidatorTests
{
    private readonly NoteValidator _sut = new();

    [Fact]
    public void Valide_retourne_IsValid_true()
    {
        var result = _sut.Validate(NoteBuilder.Valide());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void EtudiantId_zero_retourne_erreur()
    {
        var result = _sut.Validate(NoteBuilder.SansEtudiant());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("étudiant"));
    }

    [Fact]
    public void MatiereId_zero_retourne_erreur()
    {
        var result = _sut.Validate(NoteBuilder.SansMatiere());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("matière"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(21)]
    public void Valeur_hors_bornes_retourne_erreur(decimal valeur)
    {
        var note = new Note { EtudiantId = 1, MatiereId = 1, Valeur = valeur, Date = DateOnly.FromDateTime(DateTime.Today) };
        var result = _sut.Validate(note);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("entre"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(20)]
    public void Valeur_aux_bornes_est_valide(decimal valeur)
    {
        var note = new Note { EtudiantId = 1, MatiereId = 1, Valeur = valeur, Date = DateOnly.FromDateTime(DateTime.Today) };
        var result = _sut.Validate(note);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Date_defaut_retourne_erreur()
    {
        var result = _sut.Validate(NoteBuilder.SansDate());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("date"));
    }
}
