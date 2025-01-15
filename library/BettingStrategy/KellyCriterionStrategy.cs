namespace Gambling.Library;

public class KellyCriterionStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.KellyCriterion;
    private readonly decimal _edge;
    private readonly decimal _odds;

    public KellyCriterionStrategy(decimal edge, decimal odds)
    {
        _edge = edge;
        _odds = odds;
    }

    public Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance)
    {
        decimal fraction = (_edge / _odds) * totalBalance;
        decimal nextBetAmount = Math.Max(startingBetAmount, fraction);

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
