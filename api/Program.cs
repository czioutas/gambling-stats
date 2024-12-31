using System.Text.Json;
using System.Text.Json.Serialization;
using Gambling.API;
using Gambling.Library;
using Gambling.Library.Games;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAstro", builder =>
    {
        builder.WithOrigins("http://localhost:4322")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAstro");

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/play", (GameInputDto gameInputDto) =>
{
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
        default:
            return Results.BadRequest("No Strategy Found.");
    }

    decimal initialBalance = decimal.Parse(gameInputDto.InitialBalance);

    Gambler gambler = new Gambler(bettingStrategy, initialBalance);

    var game = new Plinko(randomness, rows: 8, risk: Risk.Low);

    Setup setup = new Setup(game, gambler);
    Stats stats = setup.Run();

    return Results.Ok(stats);
})
.WithName("Play");

app.Run();
