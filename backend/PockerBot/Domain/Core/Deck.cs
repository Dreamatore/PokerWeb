using PockerBot.Domain.Card;

namespace PockerBot.Domain.Core;

public class Deck
{
    public List<Card.Card> Storage;

    public Deck()
    {
        Storage = new List<Card.Card>();
        for (int i = (int)Rank.Two; i <= (int)Rank.Ace; i++)
        {
            for (int j = (int)Suit.Hearts; j <= (int)Suit.Spades; j++)
            {
                Storage.Add(new Card.Card((Suit)j, (Rank)i));
            }
        }
    }

    public Card.Card GetLastCard()
    {
        var lastcard = Storage[Storage.Count - 1];
        Storage.Remove(lastcard);
        Console.WriteLine($"Удаление {lastcard}");
        return lastcard;
    }

    public Card.Card GetFirstFromDeck()
    {
        var card = Storage.First();
        Storage.Remove(card);
        return card;
    }

    public Card.Card GetCard(Suit suit, Rank rank)
    {
        foreach (var card in Storage)
        {
            if (card.Rank == rank && card.Suit == suit)
            {
                Storage.Remove(card);
                return card;
            }
        }

        throw new Exception("Не получилось найти карту в колоде");
    }

    private static void Shuffle<T>(Random rng, List<T> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public void Shuffle()
    {
        var random = new Random();
        Shuffle(random, Storage);
    }
}