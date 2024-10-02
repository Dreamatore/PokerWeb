using PockerBot.Domain.Core;

namespace PockerBot.Presentation;

public class Session
{
    public int Id { get; set; }
    public List<Player> Players { get; set; }
    public GameManager GameManager { get; set; }
}