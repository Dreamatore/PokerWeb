using PockerBot.Presentation;
using Telegram.Bot.Types;

namespace PockerBot.Domain.Core;

public class Player:IEquatable<Player>
{
    public Player(string name, int bank,long id)
    {
        ID = id;
        Name = name;
        Score = 0;
        CurrentBet = 0;
        Bank = bank;
        Hand = new List<Card.Card>();
        IsFolded = false;
        IsCircleActionTaken = false;
    }

    public long ID;
    public int Score { get; set; }
    public int CurrentBet { get; set; }
    public int Bank { get; set; }
    public List<Card.Card> Hand { get; set; }
    public string Name { get; set; }
    public bool IsFolded { get; set; }
    public bool IsCircleActionTaken { get; set; }
    public bool Equals(Player? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return CurrentBet == other.CurrentBet && Bank == other.Bank && Hand.Equals(other.Hand) && Name == other.Name &&
               IsFolded == other.IsFolded;
    } 

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Player)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CurrentBet, Bank, Hand, Name, IsFolded);
    }
}