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
    public void Nom_vide_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.SansNom());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("nom"));
    }

    [Fact]
    public void Prenom_vide_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.SansPrenom());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("prénom"));
    }

    [Fact]
    public void Email_vide_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.SansEmail());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("email"));
    }

    [Fact]
    public void Email_invalide_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.EmailInvalide());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("valide"));
    }

    [Fact]
    public void Matricule_vide_retourne_erreur()
    {
        var result = _sut.Validate(EnseignantBuilder.SansMatricule());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("matricule"));
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
