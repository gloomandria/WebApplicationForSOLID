namespace ProjetScolariteSOLID.Tests.Validators;

public sealed class MatiereValidatorTests
{
    private readonly MatiereValidator _sut = new();

    [Fact]
    public void Valide_retourne_IsValid_true()
    {
        var result = _sut.Validate(MatiereBuilder.Valide());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Intitule_vide_retourne_erreur()
    {
        var result = _sut.Validate(MatiereBuilder.SansIntitule());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("intitulé"));
    }

    [Fact]
    public void Code_vide_retourne_erreur()
    {
        var result = _sut.Validate(MatiereBuilder.SansCode());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("code"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Coefficient_non_positif_retourne_erreur(int coefficient)
    {
        var matiere = new Matiere { Intitule = "Algo", Code = "ALGO-101", Coefficient = coefficient, VolumeHoraire = 45 };
        var result = _sut.Validate(matiere);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("coefficient"));
    }

    [Fact]
    public void Coefficient_positif_est_valide()
    {
        var result = _sut.Validate(MatiereBuilder.Valide());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void VolumeHoraire_negatif_retourne_erreur()
    {
        var result = _sut.Validate(MatiereBuilder.VolumeHoraireNegatif());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("volume"));
    }

    [Fact]
    public void VolumeHoraire_zero_est_valide()
    {
        var matiere = new Matiere { Intitule = "Algo", Code = "ALGO-101", Coefficient = 3, VolumeHoraire = 0 };
        var result = _sut.Validate(matiere);
        Assert.True(result.IsValid);
    }
}
