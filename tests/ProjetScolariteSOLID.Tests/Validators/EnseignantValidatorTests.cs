namespace ProjetScolariteSOLID.Tests.Validators;

public sealed class EnseignantValidatorTests
{
    private readonly EnseignantValidator _sut = new();

    [Fact]
    public void Valide_retourne_IsValid_true()
    {
        var result = _sut.Validate(EnseignantBuilder.Valide());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void SpecialiteId_zero_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.SansSpecialite());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("spécialité"));
    }

    [Fact]
    public void GradeId_zero_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.SansGrade());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("grade"));
    }
}
