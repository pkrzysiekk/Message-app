namespace Message_Backend.Models.RSA;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string PublicKeyLocation { get; set; }
    public string PrivateKeyLocation { get; set; }
    public int MinutesBeforeExpiry { get; set; }
}