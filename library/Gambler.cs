namespace Gambling.Library;

public class Gambler
{
    public readonly IBettingStrategy strategy;
    public decimal Balance { get; set; }
    public decimal? InitialBet { get; set; }
    public decimal? InitialBetPercentage { get; set; }

    public Gambler(IBettingStrategy strategy, decimal initialBalance, decimal? initialBet, decimal? initialBetPercentage)
    {
        this.strategy = strategy;
        this.Balance = initialBalance;
        this.InitialBet = initialBet;
        this.InitialBetPercentage = initialBetPercentage;

        if (this.InitialBet is null && this.InitialBetPercentage is null)
        {
            throw new ArgumentException("InitialBet or InitialBetPercentage must be set.");
        }
    }

    private decimal GetStartingBet()
    {
        if (InitialBet.HasValue)
        {
            return InitialBet.Value;
        }
        else
        {
            return this.Balance * this.InitialBetPercentage.Value;
        }
    }


    public Bet? RunStrategy(Bet? lastBet)
    {
        Bet newBet;

        if (lastBet is null)
        {
            newBet = new(
               GetStartingBet(),
               strategy.GetStrategy()
           );

            return newBet;
        }
        else
        {
            var bet = strategy.Decide(lastBet, GetStartingBet(), this.Balance);
            return bet;
        }
    }
}