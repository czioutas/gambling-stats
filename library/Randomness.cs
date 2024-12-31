namespace Gambling.Library;

public interface IRandomness
{
    double NextDouble();
    string GetServerSeed();
    string GetClientSeed();
    string GetNonce();
}

public class ProvablyFairRandomness : IRandomness
{
    private readonly string _serverSeed;
    private readonly string _clientSeed;
    private long _nonce;

    public ProvablyFairRandomness(string serverSeed, string clientSeed, long initialNonce = 0)
    {
        _serverSeed = serverSeed;
        _clientSeed = clientSeed;
        _nonce = initialNonce;
    }

    public double NextDouble()
    {
        // This could be implemented using the same hashing mechanism if needed
        // but for Plinko we don't actually use this method
        throw new NotImplementedException("Not needed for Plinko implementation");
    }

    public string GetHashedServerSeed()
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_serverSeed));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public string GetServerSeed()
    {
        return _serverSeed;
    }

    public string GetClientSeed()
    {
        return _clientSeed;
    }

    public string GetNonce()
    {
        // Increment nonce for each use
        var currentNonce = _nonce++;
        return currentNonce.ToString();
    }
}