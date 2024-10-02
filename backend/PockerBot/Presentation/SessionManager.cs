using PockerBot.Domain.Core;
using Telegram.Bot;

namespace PockerBot.Presentation;

public class SessionManager
{
    private readonly TelegramBotClient _client;
    private readonly Config _config;

    public SessionManager(TelegramBotClient client, Config config)
    {
        _client = client;
        _config = config;
    }

    private readonly Dictionary<int, Session> _sessions = new();

    public bool Create(long userId, long chatId, string name, int sessionId)
    {
        var sessionExists = _sessions.ContainsKey(sessionId);
        if (sessionExists)
        {
            return false;
        }

        var session = new Session
        {
            Id = sessionId,
            Players = [],
            GameManager = new GameManager(_client, [])
        };
        var token = Guid.NewGuid().ToString().Replace("-", "");
        var player = new TelegramPlayer(chatId, name, 1000, chatId, token);
        Console.WriteLine($"Name {name}, Token {token}");
        session.Players.Add(player);
        session.GameManager.AddTgPendingPlayer(player);
        _sessions.Add(sessionId, session);
        return true;
    }

    public void AddBot(int sessionId)
    {
        var sessionExists = _sessions.ContainsKey(sessionId);
        if (!sessionExists)
        {
            return;
        }

        var rand = new Random();
        var bot = new BotPlayer($"Bot-{rand.Next(1, 100)}", 1000, rand.Next(1, 100));

        var session = _sessions[sessionId];

        session.GameManager.AddBotPendingPlayer(bot);
        session.Players.Add(bot);
    }

    public async Task<bool> Join(long userId, long chatId, string name, int sessionId)
    {
        var sessionExists = _sessions.ContainsKey(sessionId);
        if (!sessionExists)
        {
            return false;
        }

        var token = Guid.NewGuid().ToString().Replace("-", "");

        var session = _sessions[sessionId];
        var player = new TelegramPlayer(chatId, name, 1000, chatId, token);
        Console.WriteLine($"Name {name}, Token {token}");
        if (session.Players.Any(p => p.Equals(player)))
        {
            return false;
        }

        session.GameManager.AddTgPendingPlayer(player);
        session.Players.Add(player);

        foreach (var tgPlayer in session.Players.Where(x => !x.Equals(player) && x is TelegramPlayer))
        {
            var playerToNotify = tgPlayer as TelegramPlayer;
            await _client.SendTextMessageAsync(playerToNotify!.ChatId,
                $"{player.Name} was added to session {sessionId}");
        }

        return true;
    }

    public bool StartGameByUserId(long chatId)
    {
        var session = GetSession(chatId);
        if (session == null)
        {
            return false;
        }

        if (session.Players.Count < 2)
        {
            return false;
        }

        session.GameManager.StartGame();

        foreach (var player in session.Players.Where(x => x is TelegramPlayer))
        {
            var playerToNotify = player as TelegramPlayer;

            var url = $"{_config!.FrontendBaseUrl}/game/{session.Id}?token={playerToNotify!.Token}";
            Task.Run(async () =>
            {
                try
                {
                    await _client.SendTextMessageAsync(playerToNotify!.ChatId, url);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        session.GameManager.GameEnded += async () =>
        {
            foreach (var player in session.Players.Where(x => x is TelegramPlayer))
            {
                var playerToNotify = player as TelegramPlayer;
                var winner = session.GameManager.Winner;
                var core = session.GameManager.Core;
                var resultString =
                    $"Победитель сессии: {winner.Name},Hand: {string.Join(",", winner.Hand)}, Table: {string.Join(",", core.Table)},Score: {core.GetCombinationByHand(winner.Hand.ToArray())}";
                await _client.SendTextMessageAsync(playerToNotify!.ChatId, resultString);

                session.GameManager.Dispose();
                _sessions.Remove(session.Id);
            }
        };
        return true;
    }

    public Session? GetSession(long chatId)
    {
        return _sessions.Values.FirstOrDefault(x => x.Players.Any(p => p.ID == chatId));
    }

    public Session? GetSessionById(int sessionId)
    {
        var isSessionExists = _sessions.ContainsKey(sessionId);
        return isSessionExists ? _sessions[sessionId] : null;
    }
}