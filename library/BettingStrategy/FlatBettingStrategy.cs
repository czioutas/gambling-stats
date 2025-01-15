namespace Gambling.Library;

public class FlatBettingStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.FlatBetting;

    public Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance)
    {
        Bet newBet = new(startingBetAmount, bet.Strategy)
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
