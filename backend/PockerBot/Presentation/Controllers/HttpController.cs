using Microsoft.AspNetCore.Mvc;
using PockerBot.Domain.Card;

namespace PockerBot.Presentation.Controllers;

public class HttpController : Controller
{
    private SessionManager SessionManager { get; set; }
    public HttpController(SessionManager sessionManager)
    {
        SessionManager = sessionManager;
    }

    [HttpGet("/v1/session/{sessionId}/info/{token}")]
    public object GetTableInfo(int sessionId, string token)
    {
        var session = SessionManager.GetSessionById(sessionId);
        if (session == null) return BadRequest("Session not found");
        var manager = session.GameManager;
        var timer = manager.ActionTimeout;
        var core = manager.Core;
        var player = core.Players.Where(x => x is TelegramPlayer)
            .FirstOrDefault(x => (x as TelegramPlayer)!.Token == token);

        if (player == null) return BadRequest("Player not Found");
        var combo = core.GetCombinationWithCardsByHand(player.Hand.ToArray());

        var result = new TableInfo
        {
            DealerId = core.Dealer.ID,
            ActionHistory = manager.RawHistory.ToArray(),
            ComboName = combo.name,
            Combo = combo.result.Cards,
            ActionTimeLeftSec = timer,
            Id = player.ID,
            Table = core.Table.ToArray(),
            Hand = player.Hand.ToArray(),
            OtherPlayers = core.Players.Where(x => x.ID != player.ID)
                .Select(x => new PlayerInfo { CurrentBet = x.CurrentBet, Id = x.ID, PlayerBank = x.Bank, IsFolded = x.IsFolded, Name = x.Name})
                .ToArray(),
            Name = player.Name,
            GameBank = core.GameBank,
            PlayerBank = player.Bank,
            CurrentActiveId = core.CurrentActivePlayer.ID
        };
        
        return result;
    }

    [HttpPost("/v1/session/action")]
    public async Task<object> PlayerAction([FromQuery]int sessionId, [FromQuery]string action, [FromQuery]string token)
    {
        var session = SessionManager.GetSessionById(sessionId);
        if (session == null) return BadRequest("Session not found");
        var core = session.GameManager.Core;
        var player = core.Players.Where(x => x is TelegramPlayer)
            .FirstOrDefault(x => (x as TelegramPlayer)!.Token == token);
        
        if (player == null) return BadRequest("Player not Found");
        var playerId = player.ID;
        var actionToken = await session.GameManager.PlayerAction(playerId, action);
        
        if (actionToken) return Ok();
        return BadRequest();
    }
}

public class TableInfo
{
    public long DealerId { get; set; }
    public string[] ActionHistory { get; set; }
    public int ActionTimeLeftSec { get; set; }
    public int GameBank { get; set; }
    public string Name { get; set; }
    public Card[] Combo { get; set; }
    public string ComboName { get; set; }
    public long Id { get; set; }
    public Card[] Hand { get; set; }
    public Card[] Table { get; set; }
    public int PlayerBank { get; set; }
    public PlayerInfo[] OtherPlayers { get; set; }
    public long CurrentActiveId { get; set; }
}

public class PlayerInfo
{
    public string Name { get; set; }
    public int PlayerBank { get; set; }
    public int CurrentBet { get; set; }
    public long Id { get; set; }
    public bool IsFolded { get; set; }
}