namespace Gambling.Library;

public class FibonacciStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.FibonacciSystem;
    private int _currentStep = 0;
    private readonly List<decimal> _fibonacciSequence = new() { 1, 1 }; // Precompute the Fibonacci sequence

    public Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance)
    {
        // Ensure the Fibonacci sequence is long enough
        while (_fibonacciSequence.Count <= _currentStep + 1)
        {
            _fibonacciSequence.Add(_fibonacciSequence[^1] + _fibonacciSequence[^2]);
        }

        if (bet.BetResult == BetResult.Won && _currentStep >= 2)
        {
            _currentStep -= 2; // Step back two positions after a win
        }
        else if (bet.BetResult == BetResult.Lost)
        {
            _currentStep++; // Step forward on a loss
        }

        decimal nextBetAmount = startingBetAmount * _fibonacciSequence[_currentStep];
        Bet newBet = new(nextBetAmount, bet.Strategy)
        {
            BetResult = BetResult.NotPlayed,
            RawBetResult = null,
        };

        return newBet.Amount <= totalBalance ? newBet : null;
    }

    public Strategies GetStrategy()
    {
        return _strategy;
    }
}
