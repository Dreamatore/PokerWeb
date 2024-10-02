using System.Text;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using PockerBot.Domain.Core;

namespace PockerBot.Domain;

public class ChatGptBot
{
    private static string _apiKey = "";

    private static string _prompt =
        //"As a seasoned poker champion, my expertise lies in analyzing opponents, calculating pot odds, and making strategic decisions under immense pressure." +
        "We are playing texas hold'em poker. You should use ICM poker rules. I will provide you game state and you should suggest your next move.";

    public async Task<PokerMove> MakeMove(Player player, GameCore core)
    {
        var bank = player.Bank;
        var table = string.Join(" ", core.Table);
        var hand = string.Join(" ", player.Hand);
        var gameBank = core.GameBank;

        var (minRaise, _) = core.GetAvailableRaise();

        var state = $"GameState: Dealer:{core.Dealer.Name};" +
                    $"Community:{table};" +
                    $"Hand:{hand};" +
                    $"MaxBet={core.CurrentMaxBet};Bet={player.CurrentBet};Bank={bank};Pot={gameBank};" +
                    $"Minimal Raise={minRaise}. Other players:";

        var builder = new StringBuilder(state);

        foreach (var gamePlayer in core.Players.Where(x => x.Name != player.Name))
        {
            var playerInfo =
                $"Player:{gamePlayer.Name};Bank={gamePlayer.Bank};Bet={gamePlayer.CurrentBet};Folded={gamePlayer.IsFolded}.";
            builder.Append(playerInfo);
        }
        
        var availableActions = core.GetAvailableMoves(player);

        builder.Append($"\nYou STRONGLY must use only one of this {availableActions.Length} actions: {string.Join(" ", availableActions)}. Without any additional characters!");

        var gameState = builder.ToString();

        var openAiApi = new OpenAIAPI(_apiKey);

        var response = await openAiApi.Chat.CreateChatCompletionAsync(new ChatRequest
        {
            Model = Model.ChatGPTTurbo,
            Messages = new ChatMessage[]
            {
                new(ChatMessageRole.System, _prompt),
                new(ChatMessageRole.User, gameState)
            }
        });

        var move = response.Choices.FirstOrDefault()?.Message.TextContent.Trim().ToLower();

        return move switch
        {
            "call" => PokerMove.Call,
            "raise" => PokerMove.Raise,
            "check" => PokerMove.Check,
            _ => PokerMove.Fold
        };
    }
}