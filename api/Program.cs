using System.Text.Json;
using System.Text.Json.Serialization;
using Gambling.API;
using Gambling.Library;
using Gambling.Library.Games;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAstro", builder =>
    {
        builder.WithOrigins(
                "http://localhost:4321",
                "http://localhost:9070",
                "http://192.168.178.47:9070",
                "https://gambling.ziou.xyz"
               )
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.WebHost.UseUrls("http://*:80");

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAstro");

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/", () => "1");

app.MapPost("/play", (IMemoryCache cache, GameInputDto gameInputDto) =>
{
    string cacheKey = JsonSerializer.Serialize(gameInputDto);

    if (cache.TryGetValue(cacheKey, out Stats cachedStats))
    {
        return Results.Ok(cachedStats);
    }

    if (gameInputDto.InitialBet.HasValue && 1000 < gameInputDto.InitialBalance / gameInputDto.InitialBet.Value)
    {
        return Results.BadRequest("Initial bet cannot exceed 1000x the balance");
    }

    var randomness = new ProvablyFairRandomness(
        serverSeed: gameInputDto.ServerSeed,
        clientSeed: gameInputDto.ClientSeed,
        initialNonce: gameInputDto.Nonce
    );

    IBettingStrategy bettingStrategy;

    switch (gameInputDto.Strategy)
    {
        case Strategies.ModifiedMartingale:
            bettingStrategy = new ModifiedMartingaleStrategy();
            break;
        case Strategies.Martingale:
            bettingStrategy = new MartingaleStrategy();
            break;
        case Strategies.ErrorProneModifiedMartingale:
            bettingStrategy = new ErrorProneModifiedMartingaleStrategy();
            break;
        case Strategies.ErrorProneMartingale:
            bettingStrategy = new ErrorProneMartingaleStrategy();
            break;
        case Strategies.FlatBetting:
            bettingStrategy = new FlatBettingStrategy();
            break;
        case Strategies.FibonacciSystem:
            bettingStrategy = new FibonacciStrategy();
            break;
        case Strategies.KellyCriterion:
            return Results.BadRequest("No Strategy Not Implemented yet.");
        case Strategies.ReverseMartingaleParoli:
            bettingStrategy = new ReverseMartingaleStrategy();
            break;
        default:
            return Results.BadRequest("No Strategy Found.");
    }

    try
    {
        Gambler gambler = new Gambler(bettingStrategy, gameInputDto.InitialBalance, gameInputDto.InitialBet, gameInputDto.InitialBetPercentage);

        var game = new Plinko(randomness, rows: 8, risk: Risk.Low);

        Setup setup = new Setup(game, gambler);
        Stats stats = setup.Run();

        cache.Set(cacheKey, stats);

        return Results.Ok(stats);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception)
    {
        return Results.BadRequest("An error occured");
    }

})
.WithName("Play");

app.Run();
