namespace PockerBot.Domain.Card;

public class Card: IEquatable<Card>
{
    public Suit Suit { get; set; }
    public Rank Rank { get; set; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        if (Environment.OSVersion.Platform <= PlatformID.Unix)
        {
            var suit = Suit switch
            {
                Suit.Hearts => "\u2665\ufe0f",
                Suit.Diamonds => "\u2666\ufe0f",
                Suit.Clubs => "\u2663\ufe0f",
                Suit.Spades => "\u2660\ufe0f"
            };
        
            if(Rank == Rank.Jack)
                return $"J{suit}";
            if(Rank == Rank.Queen)
                return $"Q{suit}";
            if(Rank == Rank.King)
                return $"K{suit}";
            if(Rank == Rank.Ace)
                return $"A{suit}";
        
            return $"{(int)Rank}{suit}";
        }
        else
        {
            var suit = Suit switch
            {
                Suit.Hearts => "Hearts",
                Suit.Diamonds => "Diamonds",
                Suit.Clubs => "Clubs",
                Suit.Spades => "Spades"
            };
        
            if(Rank == Rank.Jack)
                return $"J{suit}";
            if(Rank == Rank.Queen)
                return $"Q{suit}";
            if(Rank == Rank.King)
                return $"K{suit}";
            if(Rank == Rank.Ace)
                return $"A{suit}";
        
            return $"{(int)Rank}{suit}";
        }
    }

    public bool Equals(Card? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Suit == other.Suit && Rank == other.Rank;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Card)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Suit, (int)Rank);
    }

    public static bool operator ==(Card? left, Card? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Card? left, Card? right)
    {
        return !Equals(left, right);
    }
}
