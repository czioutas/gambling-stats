namespace Gambling.Library;

public enum Odds
{
    FiftyFifty
}

public enum BetResult
{
    NotPlayed,
    Lost,
    Won,
    Stalemate
}

public class Bet
{
    private readonly TimeProvider _timeProvider;
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public readonly DateTimeOffset PlacedAt;
    public decimal? Payout { get; set; }
    public readonly Strategies Strategy;
    public BetResult BetResult { get; set; }
    public decimal? RawBetResult { get; set; }
    public decimal BalanceBeforeBet { get; set; }
    public decimal BalanceAfterBet { get; set; }

    public Bet(decimal amount, Strategies strategy)
    {
        Amount = amount;
        Strategy = strategy;
        _timeProvider = TimeProvider.System;
    }

    public override string ToString()
    {
        return $"Bet[ID: {Id}, Amount: ${Amount:F2}, Strategy: {Strategy}, " +
               $"Placed: {PlacedAt}, Result: {BetResult}, " +
               $"Payout: {(Payout.HasValue ? $"${Payout:F2}" : "Pending")}, " +
               $"Raw Result: {(RawBetResult.HasValue ? RawBetResult.Value.ToString("F2") : "Pending")}]";
    }
}