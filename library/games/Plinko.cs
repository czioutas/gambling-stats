namespace Gambling.Library.Games;

public enum Risk
{
    Low,
    Medium,
    High
}

public class Plinko : IGamblingGame
{
    readonly IRandomness _randomness;
    protected readonly Stats _stats;
    protected Strategies _currentStrategy;
    private readonly int _rows;
    private readonly Risk _risk;
    private readonly (decimal multiplier, decimal probability)[] _selectedProbabilities;

    private static readonly Dictionary<Risk, (decimal multiplier, decimal probability)[]> PROBABILITIES_8 = new()
    {
        {Risk.Low, new[]
            {
                (5.0m, 0.00391m),     // Outer edges
                (1.7m, 0.03125m),     // Second from edge
                (1.1m, 0.10938m),     // Third from edge
                (1.0m, 0.21875m),     // Fourth from edge
                (0.5m, 0.27344m),      // Middle
                (1.0m, 0.21875m),     // Fourth from edge
                (1.1m, 0.10938m),     // Third from edge
                (1.7m, 0.03125m),     // Second from edge
                (5.0m, 0.00391m),     // Outer edges
            }
        }
        // TODO: Add Medium and High risk probabilities
    };

    public Plinko(IRandomness randomness, int rows, Risk risk)
    {
        _stats = new Stats();
        _randomness = randomness;

        if (rows < 8 || rows > 16)
            throw new ArgumentException("Rows must be between 8 and 16");

        _rows = rows;
        _risk = risk;

        _selectedProbabilities = PROBABILITIES_8[risk];
    }

    public Bet Play(Bet bet)
    {
        // Get the cryptographic values from IRandomness (assuming it provides these)
        string serverSeed = _randomness.GetServerSeed();
        string clientSeed = _randomness.GetClientSeed();
        string nonce = _randomness.GetNonce();


        string combinedSeed = $"{serverSeed}:{clientSeed}:{nonce}";
        Console.WriteLine(combinedSeed);

        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combinedSeed));
            ulong hashInt = BitConverter.ToUInt64(hashBytes, 0); // Use first 8 bytes
            decimal randomValue = hashInt / (decimal)ulong.MaxValue;

            var probabilities = PROBABILITIES_8[_risk]; // TODO: Handle other row counts

            decimal currentProb = 0;
            decimal multiplier = 1; // Default fallback

            var sum = Math.Abs(_selectedProbabilities.Sum(p => p.probability) - 1.0m);

            if (sum > 0.0001m)
            {
                throw new InvalidOperationException("Probabilities do not sum to 1. Received: " + sum);
            }

            foreach (var outcome in _selectedProbabilities)
            {
                decimal probability = outcome.multiplier == 0.5m ?
                    outcome.probability : // Middle only appears once
                    outcome.probability * 2; // Others appear on both sides

                currentProb += probability;
                if (randomValue <= currentProb)
                {
                    multiplier = outcome.multiplier;
                    break;
                }
            }

            bet.Payout = bet.Amount * multiplier;
            bet.RawBetResult = multiplier;

            if (multiplier == 1)
            {

                bet.BetResult = BetResult.Stalemate;
            }
            else if (multiplier > 1)
            {
                bet.BetResult = BetResult.Won;
            }
            else
            {
                bet.BetResult = BetResult.Lost;
            }

            return bet;
        }
    }
}