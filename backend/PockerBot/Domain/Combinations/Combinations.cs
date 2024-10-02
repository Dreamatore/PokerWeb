using PockerBot.Domain.Card;

namespace PockerBot.Domain.Combinations;

public class Combinations
{
    public bool CheckCombinationRoyalFlush(Card.Card[] cards)
    {
        var combination = new List<Card.Card>();
        var hearts = 0;
        var spades = 0;
        var diamonds = 0;
        var clubs = 0;

        foreach (var card in cards)
        {
            if (card.Suit == Suit.Hearts)
            {
                hearts += 1;
            }

            if (card.Suit == Suit.Spades)
            {
                spades += 1;
            }

            if (card.Suit == Suit.Diamonds)
            {
                diamonds += 1;
            }

            if (card.Suit == Suit.Clubs)
            {
                clubs += 1;
            }
        }

        if (hearts < 5 && spades < 5 && diamonds < 5 && clubs < 5)
        {
            return false;
        }

        var list = new List<int>() { hearts, spades, diamonds, clubs };
        var max = list.Max();
        var suit = Suit.Hearts;
        if (max == hearts)
        {
            suit = Suit.Hearts;
        }

        if (max == spades)
        {
            suit = Suit.Spades;
        }

        if (max == diamonds)
        {
            suit = Suit.Diamonds;
        }

        if (max == clubs)
        {
            suit = Suit.Clubs;
        }

        foreach (var card in cards)
        {
            if (card.Suit != suit)
            {
                continue;
            }

            switch (card.Rank)
            {
                case Rank.Ace:
                case Rank.Jack:
                case Rank.King:
                case Rank.Queen:
                case Rank.Ten:
                    combination.Add(card);
                    break;
            }
        }

        if (combination.Count == 5)
        {
            return true;
        }

        return false;
    }

    public bool CheckCombinationStraightFlush(Card.Card[] cards)
    {
        var hearts = 0;
        var spades = 0;
        var diamonds = 0;
        var clubs = 0;

        foreach (var card in cards)
        {
            if (card.Suit == Suit.Hearts)
            {
                hearts += 1;
            }

            if (card.Suit == Suit.Spades)
            {
                spades += 1;
            }

            if (card.Suit == Suit.Diamonds)
            {
                diamonds += 1;
            }

            if (card.Suit == Suit.Clubs)
            {
                clubs += 1;
            }
        }

        if (hearts < 5 && spades < 5 && diamonds < 5 && clubs < 5)
        {
            return false;
        }


        var list = new List<int>() { hearts, spades, diamonds, clubs };
        var max = list.Max();
        var suit = Suit.Hearts;


        if (max == hearts)
        {
            suit = Suit.Hearts;
        }

        if (max == spades)
        {
            suit = Suit.Spades;
        }

        if (max == diamonds)
        {
            suit = Suit.Diamonds;
        }

        if (max == clubs)
        {
            suit = Suit.Clubs;
        }

        var straightFlush = new List<int>();

        foreach (var card in cards)
        {
            if (card.Suit != suit)
            {
                continue;
            }

            straightFlush.Add((int)card.Rank);
        }

        straightFlush.Sort();

        if (straightFlush.Contains((int)Rank.Ace) &&
            straightFlush.Contains((int)Rank.Two) &&
            straightFlush.Contains((int)Rank.Three) &&
            straightFlush.Contains((int)Rank.Four) &&
            straightFlush.Contains((int)Rank.Five))
        {
            return true;
        }

        var result = new List<int>();


        for (var i = 0; i < straightFlush.Count - 1; i++)
        {
            var res = straightFlush[i + 1] - straightFlush[i];

            if (res != 1)
            {
                result.Clear();
                continue;
            }
            switch (result.Count)
                {
                    case 0:
                    case > 0 when straightFlush[i] - result[^1] == 1:
                        result.Add(straightFlush[i]);
                        break;
                }
                if (result.Count != 4) continue;
                
                return true;
        }
        return false;
    }

public bool CheckCombinationQuads(Card.Card[] cards)
    {
        var caree = new List<int>();
        foreach (var cardd in cards)
        {
            caree.Add((int)cardd.Rank);
        }

        caree.Sort();
        var result = new List<int>();
        for (var i = 0; i < caree.Count - 1; i++)
        {
            var res = caree[i + 1] - caree[i];

            if (res == 0)
            {
                result.Add(0);

                if (result.Count == 3)
                {
                    return true;
                }
            }
            else
            {
                result.Clear();
            }
        }

        return false;
    }

    public bool CheckCombinationFullHouse(Card.Card[] cards)
    {
        var cardRank = new List<int>();
        foreach (var card in cards)
        {
            cardRank.Add((int)card.Rank);
        }

        int setRank = 0;
        int pairRank = 0;
        for (int i = 2; i <= 14; i++)
        {
            if (cardRank.Count(rank => rank == i) == 3)
            {
                setRank = i;
                break;
            }
        }

        for (int i = 2; i <= 14; i++)
        {
            if (cardRank.Count(rank => rank == i) >= 2 && i != setRank)
            {
                pairRank = i;
                break;
            }
        }

        return setRank > 0 && pairRank > 0;
    }

    public bool CheckCombinationFlush(Card.Card[] cards)
    {
        var hearts = 0;
        var spades = 0;
        var diamonds = 0;
        var clubs = 0;

        foreach (var card in cards)
        {
            if (card.Suit == Suit.Hearts)
            {
                hearts += 1;
            }

            if (card.Suit == Suit.Spades)
            {
                spades += 1;
            }

            if (card.Suit == Suit.Diamonds)
            {
                diamonds += 1;
            }

            if (card.Suit == Suit.Clubs)
            {
                clubs += 1;
            }
        }

        if (hearts >= 5 || spades >= 5 || diamonds >= 5 || clubs >= 5)
        {
            return true;
        }

        return false;
    }

    public bool CheckCombinationStraight(Card.Card[] cards)
    {
        var street = new List<int>();
        foreach (var z in cards)
        {
            street.Add((int)z.Rank);
        }

        street.Sort();
        var result = new List<int>();

        if (street.Contains((int)Rank.Ace) &&
            street.Contains((int)Rank.Two) &&
            street.Contains((int)Rank.Three) &&
            street.Contains((int)Rank.Four) &&
            street.Contains((int)Rank.Five))
        {
            return true;
        }

        for (var i = 0; i < street.Count - 1; i++)
        {
            var res = street[i + 1] - street[i];
            if (res != 1)
            {
                result.Clear();
                continue;
            }
            switch (result.Count)
            {
                case 0:
                case > 0 when street[i] - result[^1] == 1:
                    result.Add(street[i]);
                    break;
            }
            if (result.Count != 4)
                continue;
            return true;
        }

        return false;
    }

    public bool CheckCombinationSet(Card.Card[] cards)
    {
        var arraySet = new List<int>();

        foreach (var card in cards)
        {
            arraySet.Add((int)card.Rank);
        }

        arraySet.Sort();
        var countSet = new List<int>();

        for (var i = 0; i < arraySet.Count - 1; i++)
        {
            var difference = arraySet[i + 1] - arraySet[i];

            if (difference != 0) continue;
            countSet.Add(arraySet[i]);

            if (countSet.Count != 2) continue;
            break;
        }

        if (countSet.Count <= 1) return false;
        var res = countSet.All(x => x == countSet.FirstOrDefault());

        if (res)
        {
            //(//(Console.WriteLine("Check Set: true");))
        }

        return res;
    }

    public bool CheckCombinationTwoPairs(Card.Card[] cards)
    {
        var arrayTwoPairs = new List<int>();

        foreach (var card in cards)
        {
            arrayTwoPairs.Add((int)card.Rank);
        }

        arrayTwoPairs.Sort();
        var countTwoPairs = new List<int>();

        for (var i = 0; i < arrayTwoPairs.Count - 1; i++)
        {
            var difference = arrayTwoPairs[i + 1] - arrayTwoPairs[i];

            if (difference != 0) continue;
            countTwoPairs.Add(arrayTwoPairs[i]);

            if (countTwoPairs.Count != 2) continue;
            break;
        }

        if (countTwoPairs.Count <= 1) return false;
        var res = countTwoPairs[0] != countTwoPairs[1];

        if (res)
        {
            //(//(Console.WriteLine("Check 2 pairs: true");))
        }

        return res;
    }

    public bool CheckCombinationOnePair(Card.Card[] cards)
    {
        var arrayOnePair = new List<int>();

        foreach (var card in cards)
        {
            arrayOnePair.Add((int)card.Rank);
        }

        arrayOnePair.Sort();
        var countTwoPairs = new List<int>();

        for (var i = 0; i < arrayOnePair.Count - 1; i++)
        {
            var difference = arrayOnePair[i + 1] - arrayOnePair[i];

            if (difference != 0) continue;
            countTwoPairs.Add(0);

            if (countTwoPairs.Count != 1) continue;
            //(//(Console.WriteLine("Check 1 pair: true");))
            return true;
        }

        return false;
    }

    public bool CheckCombinationHighCard(Card.Card[] cards)
    {
        return cards.Length > 0;
    }
}