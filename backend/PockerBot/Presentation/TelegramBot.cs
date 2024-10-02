using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PockerBot.Presentation
{
    public class TelegramBot
    {
        public TelegramBot(TelegramBotClient client, SessionManager sessionManager)
        {
            Client = client;
            SessionManager = sessionManager;
        }

        private readonly TelegramBotClient Client;
        private readonly SessionManager SessionManager;

        public void StartBot()
        {
            Client.StartReceiving(
                updateHandler: OnMessageHandler,
                pollingErrorHandler: HandlePollingErrorAsync);
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        async Task OnMessageHandler(object sender, Update update, CancellationToken cancellationToken)
        {
            if (update.Type is not (UpdateType.CallbackQuery or UpdateType.Message))
            {
                return;
            }

            if (update.Type == UpdateType.Message)
            {
                var msg = update.Message;
                if (msg?.Text == null)
                    return;

                if (msg.Text!.StartsWith("/create"))
                {
                    var stringId = msg.Text.Split(" ").Skip(1).Take(1).FirstOrDefault();
                    var isIdPresented = int.TryParse(stringId, out var sessionId);
                    if (!isIdPresented)
                    {
                        await Client.SendTextMessageAsync(msg.Chat.Id, "Session id not provided. Example: /create 123");
                        return;
                    }

                    var isCreated = SessionManager.Create(msg.From!.Id, msg.Chat.Id,
                        msg.From.Username ?? $"User{msg.From.Id}", sessionId);
                    if (isCreated)
                    {
                        await Client.SendTextMessageAsync(msg.Chat.Id, $"Session {sessionId} created");
                    }
                    else
                    {
                        await Client.SendTextMessageAsync(msg.Chat.Id,
                            $"Error! Session {sessionId} already exists. To join use /join {sessionId}");
                    }

                    return;
                }

                if (msg.Text.StartsWith("/join"))
                {
                    var stringId = msg.Text.Split(" ").Skip(1).Take(1).FirstOrDefault();
                    var isIdPresented = int.TryParse(stringId, out var sessionId);
                    if (!isIdPresented)
                    {
                        await Client.SendTextMessageAsync(msg.Chat.Id, "Session id not provided. Example: /join 123");
                        return;
                    }

                    var isJoined = await SessionManager.Join(msg.From!.Id, msg.Chat.Id,
                        msg.From.Username ?? $"User{msg.From.Id}", sessionId);
                    if (isJoined)
                    {
                        await Client.SendTextMessageAsync(msg.Chat.Id, $"You was added to session {sessionId}");
                    }
                    else
                    {
                        await Client.SendTextMessageAsync(msg.Chat.Id,
                            $"Error! You was already joined to session {sessionId}");
                    }

                    return;
                }

                var activeSession = SessionManager.GetSession(msg.Chat.Id);
                if (activeSession == null) return;

                if (msg.Text == "/addBot")
                {
                    SessionManager.AddBot(activeSession.Id);
                    return;
                }

                if (msg.Text == "/startSession")
                {
                    SessionManager.StartGameByUserId(msg.Chat.Id);
                    await Client.SendTextMessageAsync(msg.Chat.Id, "Game started");
                    return;
                }
            }
            else
            {
                var chatId = update.CallbackQuery?.Message?.Chat.Id ?? -1;
                var activeSession = SessionManager.GetSession(chatId);
                if (activeSession == null) return;

                var action = update?.CallbackQuery?.Data?.ToLowerInvariant() ?? "";
                if (action == "check" || action == "call" || action == "fold" || action.StartsWith("raise") ||
                    action.StartsWith("allin"))
                {
                    await activeSession.GameManager.PlayerAction(chatId, action);
                }
            }
        }
    }
}