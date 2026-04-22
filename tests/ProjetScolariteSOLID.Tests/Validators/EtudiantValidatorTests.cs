namespace ProjetScolariteSOLID.Tests.Validators;

public sealed class EtudiantValidatorTests
{
    private readonly EtudiantValidator _sut = new();

    [Fact]
    public void Valide_retourne_IsValid_true()
    {
        var result = _sut.Validate(EtudiantBuilder.Valide());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DateNaissance_defaut_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.DateNaissanceDefaut());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("naissance"));
    }

    [Fact]
    public void DateNaissance_future_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.DateNaissanceFuture());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("futur"));
    }
}
