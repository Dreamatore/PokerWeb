namespace PockerBot.Domain.Combinations;

public class ComboResult: IEquatable<ComboResult>
{
    public Card.Card[] Cards { get; set; }

    public Combo Combo { get; set; }

    public int Score { get; set; }

    public static ComboResult Empty => new()
    {
        Cards = [],
        Combo = Combo.Unknown,
        Score = 0
    };
    
    public bool Equals(ComboResult? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        if (Cards.Length != other.Cards.Length) return false;
        
        foreach (var card in Cards)
        {
            if (!other.Cards.Contains(card)) return false;
        }
        
        return Combo == other.Combo && Score == other.Score;
    }
    
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ComboResult)obj);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Cards, (int)Combo, Score);
    }
    
    public static bool operator ==(ComboResult? left, ComboResult? right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(ComboResult? left, ComboResult? right)
    {
        return !Equals(left, right);
    }
}