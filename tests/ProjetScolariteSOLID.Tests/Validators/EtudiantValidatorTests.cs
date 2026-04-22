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
    public void Nom_vide_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.SansNom());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("nom"));
    }

    [Fact]
    public void Nom_trop_long_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.NomTropLong());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("100"));
    }

    [Fact]
    public void Prenom_vide_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.SansPrenom());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("prénom"));
    }

    [Fact]
    public void Email_vide_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.SansEmail());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("email"));
    }

    [Fact]
    public void Email_sans_arobase_retourne_erreur()
    {
        var result = _sut.Validate(EtudiantBuilder.EmailInvalide());
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("valide"));
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

    [Fact]
    public void Plusieurs_erreurs_sont_toutes_retournees()
    {
        var etudiant = new Etudiant { Id = 1, Nom = "", Prenom = "", Email = "alice@test.com", DateNaissance = new DateOnly(2000, 6, 15) };
        var result = _sut.Validate(etudiant);
        Assert.True(result.Errors.Count >= 2);
    }
}
