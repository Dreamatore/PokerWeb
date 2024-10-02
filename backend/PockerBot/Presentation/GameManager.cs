using PockerBot.Domain;
using PockerBot.Domain.Core;
using PockerBot.Structures;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Timer = System.Timers.Timer;

namespace PockerBot.Presentation;

public class GameManager : IDisposable
{
    private readonly TelegramBotClient _client;
    private readonly List<Player> _lobbyPlayers;
    
    public int ActionTimeout => _timeout - (DateTime.UtcNow - _lastActionTime).Seconds;

    private List<TelegramPlayer> TgLobbyPlayers()
    {
        var result = new List<TelegramPlayer>();
        foreach (var player in _lobbyPlayers)
        {
            if (player is TelegramPlayer telegramPlayer)
            {
                result.Add(telegramPlayer);
            }
        }

        return result;
    }

    private GameCore _core;
    public GameCore Core => _core;
    private readonly Timer _timeoutTimer;
    private readonly Timer _notifyTimer;

    private int _timeout = 300;
    private DateTime _lastActionTime = DateTime.UtcNow;
    public Player Winner => _core.Players[0];

    public CircularBuffer<string> TelegramHistory { get; set; } = new(10);
    public CircularBuffer<string> RawHistory { get; set; } = new(30);

    public event Action GameEnded;

    public GameManager(TelegramBotClient client, List<Player> lobbyPlayers)
    {
        _client = client;
        _lobbyPlayers = lobbyPlayers;
        _timeoutTimer = new Timer();
        _timeoutTimer.Interval = TimeSpan.FromSeconds(_timeout).TotalMilliseconds;
        _timeoutTimer.AutoReset = false;

        _notifyTimer = new Timer();
        _notifyTimer.Interval = TimeSpan.FromSeconds(2).TotalMilliseconds;
        _notifyTimer.AutoReset = true;

        _notifyTimer.Elapsed += async (sender, args) =>
        {
            try
            {
                await SendUpdatedGameStatusAsync();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
            }
        };
    }


    public void AddTgPendingPlayer(TelegramPlayer player)
    {
        _lobbyPlayers.Add(player);
    }

    public void AddBotPendingPlayer(BotPlayer player)
    {
        _lobbyPlayers.Add(player);
    }


    public async void StartGame()
    {
        _core = new GameCore([.._lobbyPlayers]);

        _core.NotifyPlayerAboutRoundEnds += async s =>
        {
            RawHistory.PushBack(s);
            TelegramHistory.PushBack("\n" + s + "\n---------------------------------------\n");
        };

        _core.StartRound();

        _timeoutTimer.Elapsed += async (sender, args) =>
        {
            try
            {
                await CheckOrFoldAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                await ContinueGameAsync();
            }
        };

        _notifyTimer.Start();
        await ContinueGameAsync();
    }

    public async Task CheckOrFoldAsync()
    {
        var activePlayer = _core.CurrentActivePlayer;
        if (activePlayer.CurrentBet != _core.CurrentMaxBet)
        {
            _core.Fold(activePlayer);
            var message = $"{activePlayer.Name} folded (timeout)";
            RawHistory.PushBack(message);
            TelegramHistory.PushBack(message);
            //await _client.SendTextMessageAsync(activePlayer.ChatId, "Due to timeout you were forced to Fold");
        }
        else
        {
            _core.Check(activePlayer);
            var message = $"{activePlayer.Name} checked (timeout)";
            RawHistory.PushBack(message);
            TelegramHistory.PushBack(message);
            //await _client.SendTextMessageAsync(activePlayer.ChatId, "Due to timeout you were forced to Check");
        }
    }

    public async Task<bool> ContinueGameAsync()
    {
        if (_core.IsCircleEnded())
        {
            //ActionHistory.Clear();
            _core.StartNewCircle();
        }

        if (_core.IsGameEnded())
        {
            await SendUpdatedGameStatusAsync();
            GameEnded?.Invoke();
            return false;
        }

        await NotifyActivePlayerAction();
        return true;
    }

    private async Task SendUpdatedGameStatusAsync()
    {
        foreach (var player in TgLobbyPlayers())
        {
            var buttons = player.Equals(_core.CurrentActivePlayer)
                ? GetPokerMoveButtons(_core.GetAvailableMoves(player))
                : GetEmptyButtons();

            var message = GetInfoMessage(player);

            if (player.LastSentMessage == message) continue;

            if (player.MessageId == 0)
            {
                var msgId = await _client.SendTextMessageAsync(player.ChatId, message,
                    replyMarkup: buttons);

                player.MessageId = msgId.MessageId;
                player.LastSentMessage = message;
            }
            else
            {
                try
                {
                    await _client.EditMessageTextAsync(player.ChatId, player.MessageId, message,
                        replyMarkup: buttons);
                    player.LastSentMessage = message;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }

    public async Task<bool> PlayerAction(long chatId, string action)
    {
        _timeoutTimer.Stop();

        var activePlayer = _core.CurrentActivePlayer;

        if (activePlayer.ID != chatId)
        {
            return false;
        }

        var (minRaise, stepRaise) = _core.GetAvailableRaise();

        if (action == "check")
        {
            _core.Check(activePlayer);
            await NotifyOtherPlayersAboutAction(activePlayer, "checked");
            await ContinueGameAsync();
            return true;
        }

        if (action == "call")
        {
            _core.Call(activePlayer);
            await NotifyOtherPlayersAboutAction(activePlayer, "called");
            await ContinueGameAsync();
            return true;
        }

        if (action.Contains("raise"))
        {
            var providedSum = action.Split(" ").Skip(1).Take(1).FirstOrDefault();
            var sum = int.TryParse(providedSum, out var additionalSum) ? additionalSum + minRaise : minRaise;
            _core.Raise(activePlayer, sum);
            await NotifyOtherPlayersAboutAction(activePlayer, $"raised to {sum}");
            await ContinueGameAsync();
            return true;
        }

        if (action.Contains("allin"))
        {
            _core.AllIn(activePlayer);
            await NotifyOtherPlayersAboutAction(activePlayer, "all-in");
            await ContinueGameAsync();
            return true;
        }

        if (action == "fold")
        {
            _core.Fold(activePlayer);
            await NotifyOtherPlayersAboutAction(activePlayer, "folded");
            await ContinueGameAsync();
            return true;
        }

        _timeoutTimer.Start();
        return false;
    }

    public async Task NotifyOtherPlayersAboutAction(Player player, string action)
    {
        var message = $"{player.Name} {action}";
        TelegramHistory.PushBack(message);
        RawHistory.PushBack(message);
    }

    //todo: fix naming
    public async Task NotifyActivePlayerAction()
    {
        var activePlayer = _core.CurrentActivePlayer;

        if (activePlayer is BotPlayer)
        {
            var bot = new ChatGptBot();
            var botMove = await bot.MakeMove(activePlayer, _core);
            switch (botMove)
            {
                case PokerMove.Call:
                    await PlayerAction(activePlayer.ID, "call");
                    break;
                case PokerMove.Raise:
                    await PlayerAction(activePlayer.ID, "raise");
                    break;
                case PokerMove.Fold:
                    await PlayerAction(activePlayer.ID, "fold");
                    break;
                case PokerMove.Check:
                    await PlayerAction(activePlayer.ID, "check");
                    break;
            }

            return;
        }

        _timeoutTimer.Start();
        _lastActionTime = DateTime.UtcNow;
    }

    private string GetInfoMessage(Player player)
    {
        var otherPlayers = _core.Players.Where(x => x.ID != player.ID);
        var otherInfo =
            "\n" + string.Join("\n", otherPlayers.Select(x => $"{x.Name}: Bank {x.Bank}, Bet {x.CurrentBet}"));
        var bank = player.Bank;
        var table = _core.Table.Count > 0 ? "Community: " + string.Join(" ", _core.Table) + "\n" : "";
        var hand = string.Join(",", player.Hand);
        var gameBank = _core.GameBank;
        //var (minRaise, stepRaise) = _core.GetAvailableRaise();
        var score = $"Combo: {_core.GetCombinationByHand(player.Hand.ToArray())}";
        var timer = _timeout - (DateTime.UtcNow - _lastActionTime).Seconds;
        var timerMessage = player.Equals(_core.CurrentActivePlayer) ? $"\nTime left: {timer}" : "";

        var text =
            $"{table}" +
            $"Hand {hand}\n\n" +
            $"Bank {bank}\n" +
            $"Bet {player.CurrentBet}\n" +
            $"Pot {gameBank}\n" +
            $"{score}\n\n" +
            $"Players {otherInfo}\n\n" +
            //$"Min raise: {minRaise}, step: {stepRaise}\n\n" +
            $"History:\n{string.Join("\n", TelegramHistory.ToArray())}\n" +
            $"{timerMessage}";

        return text;
    }

    private InlineKeyboardMarkup GetEmptyButtons()
    {
        var inline = new List<List<InlineKeyboardButton>>();

        var row1 = new List<InlineKeyboardButton> { new("...") { CallbackData = "..." } };

        inline.Add(row1);

        return new InlineKeyboardMarkup(inline);
    }

    private InlineKeyboardMarkup GetPokerMoveButtons(PokerMove[] availableActions)
    {
        var inline = new List<List<InlineKeyboardButton>>();

        var row1 = availableActions.Take(2).Select(action => new InlineKeyboardButton(action.ToString())
            { CallbackData = action.ToString() }).ToList();
        var row2 = availableActions.Skip(2).Take(2).Select(action => new InlineKeyboardButton(action.ToString())
            { CallbackData = action.ToString() }).ToList();

        inline.Add(row1);
        inline.Add(row2);

        return new InlineKeyboardMarkup(inline);
    }

    public void Dispose()
    {
        _timeoutTimer.Dispose();
        _notifyTimer.Dispose();
    }
}