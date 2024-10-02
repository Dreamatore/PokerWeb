using PockerBot.Domain.Core;

namespace PockerBot.Presentation;

public class TelegramPlayer : Player
{
    public string Token { get; set; }
    public TelegramPlayer( long chatId, string name, int bank,long id,string token) : base(name, bank,id)
    {
        ChatId = chatId;
        Token = token;
    }
    public string LastSentMessage { get; set; }
    public int MessageId { get; set; }
    public long ChatId { get; set; }
    
}
