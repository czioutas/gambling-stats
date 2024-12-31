using Gambling.Library;
using Gambling.Library.Games;

namespace Gambling.Library;

public class Gambler
{
    public readonly IBettingStrategy strategy;
    public decimal Balance { get; set; }
    private decimal _startingBetAmount;

    public Gambler(IBettingStrategy strategy, decimal initialBalance)
    {
        this.strategy = strategy;
        this.Balance = initialBalance;
        this._startingBetAmount = Balance * 0.01m;
    }


    public Bet? RunStrategy(Bet? lastBet)
    {
        Bet newBet;

        if (lastBet is null)
        {
            newBet = new(
               Balance * 0.01m,
               strategy.GetStrategy()
           );

            return newBet;
        }
        else
        {
            var bet = strategy.Decide(lastBet, this._startingBetAmount, this.Balance);

            return bet;
        }
    }
}