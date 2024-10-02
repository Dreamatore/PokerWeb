using PockerBot.Domain.Card;
using PockerBot.Domain.Combinations;
using PockerBot.Domain.Core;

namespace PockerBot;

public class Generator
{
    public Deck Deck;

    public Generator()
    {
        Deck = new Deck();
    }

    public Card GetCard(Suit suit = Suit.Clubs, Rank rank = Rank.Ace)
    {
        return Deck.GetCard(suit, rank);
    }

    /// <summary>
    /// Метод для получения пары из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetPair()
    {
        if (Deck.Storage.Count < 2) return GeneratorResult.Empty;
        var result = new List<Card>();
        var firstCard = Deck.Storage[0];
        result.Add(firstCard);

        Deck.GetCard(firstCard.Suit, firstCard.Rank);
        foreach (var card in Deck.Storage)
        {
            if (card.Rank == firstCard.Rank)
            {
                Deck.Storage.Remove(card);
                result.Add(card);
                var sum = (int)result[0].Rank * 2 + HandScoreConst.OnePair;
                var res = new GeneratorResult
                {
                    Sum = sum,
                    Cards = result
                };
                return res;
            }
        }

        return GeneratorResult.Empty;
    }

    /// <summary>
    /// Метод для получения двух пар из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetTwoPairs()
    {
        if (Deck.Storage.Count < 4) return GeneratorResult.Empty;

        var resultCards = new List<Card>();
        var secondResultCards = new List<Card>();

        foreach (var firstComboCard in Deck.Storage)
        {
            var sum = (int)firstComboCard.Rank;
            resultCards.Add(firstComboCard);

            foreach (var secondComboCard in Deck.Storage)
            {
                if (secondComboCard == firstComboCard)
                {
                    continue;
                }

                if (secondComboCard.Rank == firstComboCard.Rank)
                {
                    resultCards.Add(secondComboCard);
                    sum += (int)secondComboCard.Rank;
                    break;
                }
            }

            if (resultCards.Count > 1)
            {
                foreach (var thirdComboCard in Deck.Storage)
                {
                    var secondSum = 0;

                    if (thirdComboCard.Rank != firstComboCard.Rank)
                    {
                        secondResultCards.Add(thirdComboCard);
                        secondSum += (int)thirdComboCard.Rank;
                    }
                    else
                    {
                        continue;
                    }

                    foreach (var fourthComboCard in Deck.Storage)
                    {
                        if (fourthComboCard == thirdComboCard)
                        {
                            continue;
                        }

                        if (fourthComboCard.Rank == thirdComboCard.Rank)
                        {
                            secondResultCards.Add(fourthComboCard);
                            secondSum += (int)fourthComboCard.Rank;

                            resultCards.Add(secondResultCards[0]);
                            resultCards.Add(secondResultCards[1]);
                            sum += secondSum;

                            for (int i = 0; i < 4; i++)
                            {
                                var a = resultCards[i];
                                Deck.Storage.Remove(a);
                            }

                            var res = new GeneratorResult
                            {
                                Cards = resultCards,
                                Sum = sum + HandScoreConst.TwoPairs
                            };
                            return res;
                        }
                    }

                    secondResultCards.Clear();
                }
            }

            resultCards.Clear();
        }

        return GeneratorResult.Empty;
    }

    /// <summary>
    /// Метод для получения стрита из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetStraight()
    {
        if (Deck.Storage.Count < 5) return GeneratorResult.Empty;

        var array = Deck.Storage.OrderBy(x => (int)x.Rank).ToList();

        // var sequence = array
        //     .Select((value, index) => new { Value = value, Index = index })
        //     .Where(x => x.Index == 0 || (int)array[x.Index].Rank == (int)array[x.Index - 1].Rank + 1)
        //     .Select(x => x.Value).Take(5).ToList();
        
        var sequence = array
            .Select((value, index) => new { Value = value, Index = index })
            .Aggregate(
                new List<Card>(),
                (acc, x) =>
                {
                    if (acc.Count == 0 || (int)x.Value.Rank == (int)acc.Last().Rank + 1)
                    {
                        acc.Add(x.Value);
                    }
                    else
                    {
                        if (acc.Count > 1)
                        {
                            return acc;
                        }
                        acc.Clear();
                        acc.Add(x.Value);
                    }
                    return acc;
                });

        if (sequence.Count < 5)
        {
            if (sequence.Count == 4 && sequence[0].Rank == Rank.Two && sequence[^1].Rank == Rank.Five &&
                array[^1].Rank == Rank.Ace)
            {
                sequence.Insert(0, array[^1]);
            }
            else
            {
                return GeneratorResult.Empty;
            }
        }

        foreach (var card in sequence)
        {
            Deck.GetCard(card.Suit, card.Rank);
        }

        var preSum = sequence.Select(x => (int)x.Rank).Sum();

        //в колесе туз стоит 1
        const int aceCostInWheel = 13;
        var sum = sequence[0].Rank != Rank.Ace ? preSum : preSum - aceCostInWheel;
        var res = sum + HandScoreConst.Straight;
        return new GeneratorResult { Cards = sequence, Sum = res };
    }


    /// <summary>
    /// Метод для получения флеша из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetFlush()
    {
        if (Deck.Storage.Count < 5) return GeneratorResult.Empty;

        var resultCards = new List<Card>();

        foreach (var currentcard in Deck.Storage)
        {
            var sum = (int)currentcard.Rank;
            resultCards.Add(currentcard);

            foreach (var card in Deck.Storage)
            {
                if (card == currentcard)
                {
                    continue;
                }

                if (card.Suit == currentcard.Suit)
                {
                    resultCards.Add(card);
                    sum += (int)card.Rank;

                    if (resultCards.Count == 5)
                    {
                        var res = new GeneratorResult
                        {
                            Cards = resultCards,
                            Sum = sum + HandScoreConst.Flush
                        };

                        for (int i = 0; i < 5; i++)
                        {
                            var a = resultCards[i];
                            Deck.Storage.Remove(a);
                        }

                        return res;
                    }
                }
            }

            resultCards.Clear();
        }

        return GeneratorResult.Empty;
    }

    /// <summary>
    /// Метод для получения стрит-флеша из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetStraightFlush()
    {
        if (Deck.Storage.Count < 5) return GeneratorResult.Empty;

        for (var i = (int)Suit.Hearts; i <= (int)Suit.Spades; i++)
        {
            var array = Deck.Storage.Where(x => (int)x.Suit == i).OrderBy(x => (int)x.Rank).ToList();

            var sequence = array
                .Select((value, index) => new { Value = value, Index = index })
                .Where(x => x.Index == 0 || (int)array[x.Index].Rank == (int)array[x.Index - 1].Rank + 1)
                .Select(x => x.Value).Take(5).ToList();

            if (sequence.Count < 5)
            {
                if (sequence.Count == 4 && sequence[0].Rank == Rank.Two && sequence[^1].Rank == Rank.Five &&
                    array[^1].Rank == Rank.Ace)
                {
                    sequence.Insert(0, array[^1]);
                }
                else
                {
                    continue;
                }
            }

            foreach (var card in sequence)
            {
                Deck.GetCard(card.Suit, card.Rank);
            }

            var preSum = sequence.Select(x => (int)x.Rank).Sum();

            //в колесе туз стоит 1
            const int aceCostInWheel = 13;
            var sum = sequence[0].Rank != Rank.Ace ? preSum : preSum - aceCostInWheel;
            var res = sum + HandScoreConst.StraightFlush;
            return new GeneratorResult { Cards = sequence, Sum = res };
        }

        return GeneratorResult.Empty;
    }


    /// <summary>
    /// Метод для получения сета из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetSet()
    {
        if (Deck.Storage.Count < 3)
            return GeneratorResult.Empty;

        var possibleSet = Deck.Storage.GroupBy(x => x.Rank)
            .Where(x => x.Count() > 2)
            .ToList();

        if (possibleSet.Count == 0) return GeneratorResult.Empty;

        var setCards = possibleSet.First().ToList()[..3];

        foreach (var card in setCards)
        {
            Deck.GetCard(card.Suit, card.Rank);
        }

        return new GeneratorResult
        {
            Cards = setCards,
            Sum = (int)setCards.First().Rank * 3 + HandScoreConst.Set
        };
    }

    /// <summary>
    /// Метод для получения каре из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetQuads()
    {
        if (Deck.Storage.Count < 4) return GeneratorResult.Empty;

        var cardCounts = new Dictionary<Rank, int>();
        foreach (var card in Deck.Storage)
        {
            if (!cardCounts.ContainsKey(card.Rank))
            {
                cardCounts.Add(card.Rank, 0);
            }

            cardCounts[card.Rank]++;
        }

        foreach (var cardValue in cardCounts.Keys)
        {
            if (cardCounts[cardValue] == 4)
            {
                var quadsCards = Deck.Storage.Where(card => card.Rank == cardValue).Take(4).ToList();

                foreach (var card in quadsCards)
                {
                    Deck.GetCard(card.Suit, card.Rank);
                }

                var sum = (int)cardValue * 4 + HandScoreConst.Quads;
                var res = new GeneratorResult
                {
                    Sum = sum,
                    Cards = quadsCards
                };
                return res;
            }
        }

        return GeneratorResult.Empty;
    }

    /// <summary>
    /// Метод для получения стрит-флеша из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetRoyalFlush()
    {
        if (Deck.Storage.Count < 5) return GeneratorResult.Empty;

        for (var i = (int)Suit.Hearts; i <= (int)Suit.Spades; i++)
        {
            var array = Deck.Storage.Where(x => (int)x.Suit == i).OrderBy(x => (int)x.Rank).ToList();

            var sequence = array
                .Select((value, index) => new { Value = value, Index = index })
                .Where(x => x.Index == 0 || (int)array[x.Index].Rank == (int)array[x.Index - 1].Rank + 1)
                .Select(x => x.Value).ToList();

            var result = sequence.Skip(sequence.Count - 5).Take(5).ToList();

            if (result.Count < 5 || result.FirstOrDefault()?.Rank != Rank.Ten || result.LastOrDefault()?.Rank != Rank.Ace)
            {
                continue;
            }

            foreach (var card in result)
            {
                Deck.GetCard(card.Suit, card.Rank);
            }

            var sum = result.Sum(x => (int)x.Rank);
            
            return new GeneratorResult { Cards = result, Sum = HandScoreConst.RoyalFlush + sum};
        }

        return GeneratorResult.Empty;
    }

    /// <summary>
    /// Метод для получения фулхауса из колоды
    /// </summary>
    /// <returns></returns>
    public GeneratorResult GetFullHouse()
    {
        if (Deck.Storage.Count < 5) return GeneratorResult.Empty;
        var arrayFullHouse = new List<Card>();
        var arrayPossibleSet = Deck.Storage.GroupBy(x => x.Rank)
            .Where(x => x.Count() > 2)
            .Aggregate(new List<Card>(), (x, y) =>
            {
                x.AddRange(y);
                return x;
            }).ToList();
        if (arrayPossibleSet.Count == 0) return GeneratorResult.Empty;
        var setCard = arrayPossibleSet.FirstOrDefault();
        var arrayPossiblePair = Deck.Storage.Where(x => x.Rank != setCard!.Rank).GroupBy(x => x.Rank)
            .Where(x => x.Count() > 1)
            .Aggregate(new List<Card>(), (x, y) =>
            {
                x.AddRange(y);
                return x;
            }).ToList();


        var pairCard = arrayPossiblePair.FirstOrDefault();
        if (pairCard == null) return GeneratorResult.Empty;


        arrayFullHouse.Add(pairCard);
        arrayFullHouse.Add(setCard);


        Deck.GetCard(pairCard.Suit, pairCard.Rank);
        Deck.GetCard(setCard.Suit, setCard.Rank);

        foreach (var card in Deck.Storage)
        {
            if (card.Rank == pairCard.Rank)
            {
                Deck.Storage.Remove(card);
                arrayFullHouse.Add(card);
                break;
            }
        }

        var setCount = 1;
        var copyDeck = new List<Card>(Deck.Storage);
        foreach (var card in copyDeck)
        {
            if (card.Rank == setCard.Rank)
            {
                Deck.Storage.Remove(card);
                arrayFullHouse.Add(card);
                setCount++;
                if (setCount == 3) break;
            }
        }

        var sum = arrayFullHouse.Select(x => (int)x.Rank).Sum();
        var res = sum + HandScoreConst.FullHouse;
        return new GeneratorResult
        {
            Cards = arrayFullHouse, Sum = res
        };
    }

    public class GeneratorResult
    {
        public List<Card> Cards { get; set; }
        public int Sum { get; set; }

        public static GeneratorResult Empty => new GeneratorResult { Cards = [] };
    }
}