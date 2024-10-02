using PockerBot.Domain.Card;

namespace PockerBot.Domain.Combinations;

public class ComboScoreCalculator
{
    private static readonly (int, Card.Card[]) Empty = (0, []);

    public ComboResult CalcScore(Card.Card[] cards)
    {
        var combination = new Combinations();
        if (combination.CheckCombinationRoyalFlush(cards))
        {
            var result = RoyalFlushScore(cards);
            return new ComboResult
            {
                Cards = result.Item2,
                Combo = Combo.RoyalFlush,
                Score = result.Item1
            };
        }

        if (combination.CheckCombinationStraightFlush(cards))
        {
            var res = StraightFlushScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.StraightFlush,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationQuads(cards))
        {
            var res = QuadsScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.Quads,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationFullHouse(cards))
        {
            var res = FullHouseScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.FullHouse,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationFlush(cards))
        {
            var res = FlushScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.Flush,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationStraight(cards))
        {
            var res = StraightScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.Straight,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationSet(cards))
        {
            var res = SetScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.Set,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationTwoPairs(cards))
        {
            var res = TwoPairsScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.TwoPairs,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationOnePair(cards))
        {
            var res = OnePairScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.OnePair,
                Score = res.Item1
            };
        }

        if (combination.CheckCombinationHighCard(cards))
        {
            var res = HighCardScore(cards);
            return new ComboResult
            {
                Cards = res.Item2,
                Combo = Combo.HighCard,
                Score = res.Item1
            };
        }

        return ComboResult.Empty;
    }

    public int HandScore(Card.Card[] cards)
    {
        var combination = new Combinations();
        if (combination.CheckCombinationRoyalFlush(cards))
        {
            var result = RoyalFlushScore(cards);
            return result.Item1;
        }

        if (combination.CheckCombinationStraightFlush(cards))
        {
            var res = StraightFlushScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationQuads(cards))
        {
            var res = QuadsScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationFullHouse(cards))
        {
            var res = FullHouseScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationFlush(cards))
        {
            var res = FlushScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationStraight(cards))
        {
            var res = StraightScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationSet(cards))
        {
            var res = SetScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationTwoPairs(cards))
        {
            var res = TwoPairsScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationOnePair(cards))
        {
            var res = OnePairScore(cards);
            return res.Item1;
        }

        if (combination.CheckCombinationHighCard(cards))
        {
            var res = HighCardScore(cards);
            return res.Item1;
        }

        return 0;
    }

    public (int, Card.Card[]) RoyalFlushScore(Card.Card[] cards)
    {
        var combination = new List<int>();
        var hearts = 0;
        var spades = 0;
        var diamonds = 0;
        var clubs = 0;

        foreach (var card in cards)
        {
            switch (card.Suit)
            {
                case Suit.Hearts:
                    hearts += 1;
                    break;
                case Suit.Spades:
                    spades += 1;
                    break;
                case Suit.Diamonds:
                    diamonds += 1;
                    break;
                case Suit.Clubs:
                    clubs += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (hearts < 5 && spades < 5 && diamonds < 5 && clubs < 5)
        {
            return Empty;
        }

        var list = new List<int> { hearts, spades, diamonds, clubs };
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

        var cardsInCombo = new List<Card.Card>();
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
                    combination.Add((int)card.Rank);
                    cardsInCombo.Add(card);
                    break;
            }
        }

        if (combination.Count == 5 && combination.Sum() == 60)
        {
            return (combination.Sum() + HandScoreConst.RoyalFlush, cardsInCombo.ToArray());
        }

        return Empty;
    }

    public (int, Card.Card[]) HighCardScore(Card.Card[] cards)
    {
        var maxRank = cards.Select(x => x.Rank).Max();
        var firstCard = cards.FirstOrDefault(x=>x.Rank == maxRank);
        return ((int)firstCard!.Rank, new []{firstCard});
    }

    public (int, Card.Card[]) OnePairScore(Card.Card[] cards)
    {
        var arrayOnePair = cards.OrderBy(x => (int)x.Rank).ToList();
        var onePairList = new List<Card.Card>();

        for (var i = 0; i < arrayOnePair.Count - 1; i++)
        {
            var difference = (int)arrayOnePair[i + 1].Rank - (int)arrayOnePair[i].Rank;

            if (difference != 0) continue;
            onePairList.Add(arrayOnePair[i]);
            onePairList.Add(arrayOnePair[i+1]);
        }

        var max = onePairList.Max(x => x.Rank);

        return ((int)max * 2 + HandScoreConst.OnePair, onePairList.Where(x => x.Rank == max).ToArray());
    }

    public (int, Card.Card[]) FullHouseScore(Card.Card[] cards)
    {
        var arrayFullHouse = new List<Card.Card>(cards);
        var arrayPossibleSet = arrayFullHouse.GroupBy(x => x.Rank)
            .Where(x => x.Count() > 2)
            .Aggregate(new List<Card.Card>(), (x, y) =>
            {
                x.AddRange(y);
                return x;
            }).ToList();

        var maxSetRank = arrayPossibleSet.Select(t => (int)t.Rank).Prepend(0).Max();

        var setCards = arrayPossibleSet.Where(x => (int)x.Rank == maxSetRank).Take(3).ToList();

        var arrayPossiblePair = arrayFullHouse.Where(x => (int)x.Rank != maxSetRank).GroupBy(x => x.Rank)
            .Where(x => x.Count() > 1)
            .Aggregate(new List<Card.Card>(), (x, y) =>
            {
                x.AddRange(y);
                return x;
            }).ToList();

        var maxPairRank = arrayPossiblePair.Select(card => (int)card.Rank).Prepend(0).Max();

        var pairCards = arrayPossiblePair.Where(x => (int)x.Rank == maxPairRank).Take(2).ToList();

        var fullHouse = setCards.Concat(pairCards).ToList();

        var resultSum = fullHouse.Sum(x => (int)x.Rank) + HandScoreConst.FullHouse;

        return (resultSum, fullHouse.ToArray());
    }

    public (int, Card.Card[] cards) TwoPairsScore(Card.Card[] cards)
    {
        var arrayTwoPairs = cards.OrderBy(x=>(int)x.Rank).ToList();
        var countTwoPairs = new List<Card.Card>();
        var cardResult = new List<Card.Card>();
        for (var i = 0; i < arrayTwoPairs.Count - 1; i++)
        {
            var difference = (int)arrayTwoPairs[i + 1].Rank - (int)arrayTwoPairs[i].Rank;

            if (difference != 0) continue;
            countTwoPairs.Add(arrayTwoPairs[i]);
            cardResult.Add(arrayTwoPairs[i]);
            cardResult.Add(arrayTwoPairs[i+1]);
        }
        var maxRank = countTwoPairs.Max(x => x.Rank);
        var max = countTwoPairs.FirstOrDefault(x=>x.Rank == maxRank);
        countTwoPairs.Remove(max);
        var beforeMaxRank = countTwoPairs.Max(x=>x.Rank);
        var score = ((int)maxRank * 2) + ((int)beforeMaxRank * 2) + HandScoreConst.TwoPairs;
        cardResult = cardResult.Where(x=>x.Rank == maxRank || x.Rank == beforeMaxRank).ToList();
        return (score, cardResult.ToArray());
    }

    public (int, Card.Card[] cards) SetScore(Card.Card[] cards)
    {
        var arraySet = cards.OrderBy(x => (int)x.Rank).ToList();

        var countSet = new List<Card.Card>();
        for (var i = 0; i < arraySet.Count - 1; i++)
        {
            var difference = (int)arraySet[i + 1].Rank - (int)arraySet[i].Rank;

            if (difference != 0) continue;
            countSet.Add(arraySet[i]);
        }

        var countSet2 = new List<Card.Card>();
        for (var i = 0; i < countSet.Count - 1; i++)
        {
            var difference = (int)countSet[i + 1].Rank - (int)countSet[i].Rank;

            if (difference != 0) continue;
            countSet2.Add(countSet[i]);
        }

        var max = countSet2.Max(x => x.Rank);
        var resCards = arraySet.Where(x => x.Rank == max).ToArray();
        var score = (int)max * 3 + HandScoreConst.Set;
        return (score, resCards);
    }

    public (int, Card.Card[] cards) StraightScore(Card.Card[] cards)
    {
        var street = cards.OrderBy(x => (int)x.Rank).ToList();

        var result = new List<Card.Card>();
        var ace = street.FirstOrDefault(x => (int)x.Rank == (int)Rank.Ace);
        var two = street.FirstOrDefault(x => (int)x.Rank == (int)Rank.Two);
        var three = street.FirstOrDefault(x => (int)x.Rank == (int)Rank.Three);
        var four = street.FirstOrDefault(x => (int)x.Rank == (int)Rank.Four);
        var five = street.FirstOrDefault(x => (int)x.Rank == (int)Rank.Five);

        if (ace != null && two != null && three != null && four != null && five != null)
        {
            return (215, [ace, two, three, four, five]);
        }

        for (var i = 0; i < street.Count - 1; i++)
        {
            var res = (int)street[i + 1].Rank - (int)street[i].Rank;
            if (res != 1)
                continue;

            result.Add(street[i]);
            if (result.Count == 4)
            {
                result.Add(street[i + 1]);
            }
        }

        for (var i = 0; i < result.Count - 1; i++)
        {
            if ((int)result[i].Rank > (int)result[i + 1].Rank)
            {
                return Empty;
            }
        }

        return (result.Sum(x => (int)x.Rank) + HandScoreConst.Straight, result.ToArray());
    }

    public (int, Card.Card[] cards) FlushScore(Card.Card[] cards)
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

        if (hearts < 5 && spades < 5 && diamonds < 5 && clubs < 5) return Empty;

        var list = new List<int> { hearts, spades, diamonds, clubs };
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

        var combination = new List<Card.Card>();

        foreach (var card in cards)
        {
            if (card.Suit != suit)
            {
                continue;
            }

            combination.Add(card);
        }

        if (combination.Count < 5) return Empty;
        combination = combination.OrderBy(x => (int)x.Rank).ToList();
        combination.Reverse();
        var sum = 0;
        for (var i = 0; i < 5; i++)
        {
            sum += (int)combination[i].Rank;
        }

        return (sum + HandScoreConst.Flush, combination.ToArray());
    }

    public (int, Card.Card[] cards) StraightFlushScore(Card.Card[] cards)
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

        if (hearts < 5 && spades < 5 && diamonds < 5 && clubs < 5) return Empty;
        var list = new List<int> { hearts, spades, diamonds, clubs };
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

        var combination = new List<Card.Card>();

        foreach (var card in cards)
        {
            if (card.Suit != suit)
            {
                continue;
            }

            combination.Add(card);
        }

        combination = combination.OrderBy(x => (int)x.Rank).ToList();
        combination.Reverse();

        var sum = 0;
        var ace = combination.FirstOrDefault(x => x.Rank == Rank.Ace);
        var two = combination.FirstOrDefault(x => x.Rank == Rank.Two);
        var three = combination.FirstOrDefault(x => x.Rank == Rank.Three);
        var four = combination.FirstOrDefault(x => x.Rank == Rank.Four);
        var five = combination.FirstOrDefault(x => x.Rank == Rank.Five);
        if (ace != null && two != null && three != null && four != null && five != null)
        {
            sum += 15;
        }

        var result = new List<Card.Card>();

        for (var i = 0; i < combination.Count - 1; i++)
        {
            var res = (int)combination[i].Rank - (int)combination[i + 1].Rank;

            if (res == 1)
            {
                result.Add(combination[i]);

                if (result.Count == 4)
                {
                    result.Add(combination[i + 1]);
                    break;
                }
            }
            else
            {
                result.Clear();
            }
        }

        if (result.Count != 5 && sum == 0) return Empty;

        if (result.Count != 5) return (sum + HandScoreConst.StraightFlush, result.ToArray());

        sum = 0;
        for (var i = 0; i < 5; i++)
        {
            sum += (int)result[i].Rank;
        }

        return (sum + HandScoreConst.StraightFlush, result.ToArray());
    }

    public (int, Card.Card[]) QuadsScore(Card.Card[] cards)
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
                result.Add(caree[i]);

                if (result.Count == 3)
                {
                }
            }
        }

        var max = result.Max();
        var r = (max * 4) + HandScoreConst.Quads;
        var cardResult = new List<Card.Card>();

        for (var i = 0; i < (int)Suit.Spades; i++)
        {
            var suit = (Suit)i;
            var rank = (Rank)max;
            var a = new Card.Card(suit, rank);
            cardResult.Add(a);
        }

        return (r, cardResult.ToArray());
    }
}