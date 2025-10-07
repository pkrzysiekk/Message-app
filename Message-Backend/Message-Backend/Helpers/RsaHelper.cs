using System.Security.Cryptography;
using Message_Backend.Models.RSA;

namespace Message_Backend.Helpers;

public static class RsaHelper
{
    public static RSA LoadRsaKey(string rsaKeyPath)
    {
        var rsa = RSA.Create();
        var path = Path.Combine(Directory.GetCurrentDirectory(), rsaKeyPath);
        if (!File.Exists(path))
            throw new FileNotFoundException("RSA key not found",path);
        var pemContents = File.ReadAllText(path);
        rsa.ImportFromPem(pemContents);
        return rsa;
    }

    public static JwtOptions LoadJwtOptions()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",optional:false,reloadOnChange:true)
            .Build();
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
        if (jwtOptions == null)
            throw new FileNotFoundException("JWT options not found");
        return jwtOptions;
    }
}