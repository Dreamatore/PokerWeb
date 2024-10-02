using PockerBot;
using PockerBot.Domain.Card;
using PockerBot.Domain.Combinations;
using Xunit;

namespace PockerBotTests;

public class ComboGeneratorTests
{
    private Generator Gen { get; } = new();
    
    [Fact]
    public void IsRoyalFlushWorks()
    {
        var combo = new ComboScoreCalculator();

        var royal = Gen.GetRoyalFlush();
        var fullHouse = Gen.GetFullHouse();
        
        var score1 = combo.HandScore(royal.Cards.ToArray());
        var score2 = combo.HandScore(fullHouse.Cards.ToArray());
        
        Assert.True(score1 > score2);
        
        Assert.True(royal.Sum == score1);
        Assert.True(fullHouse.Sum == score2);
    }
    
    [Fact]
    public void IsStraightFlushWorks()
    {
        var combo = new ComboScoreCalculator();

        Gen.GetCard(Suit.Clubs, Rank.Ace);
        Gen.GetCard(Suit.Diamonds, Rank.Ace);
        Gen.GetCard(Suit.Hearts, Rank.Ace);
        Gen.GetCard(Suit.Spades, Rank.Ace);

        
        // It's possible to get straightFlush and fullHouse from 1 deck maximum 2 times --> i<2
        for (int i = 0; i < 2; i++)
        {
            var straightFlush = Gen.GetStraightFlush();
            var fullHouse = Gen.GetFullHouse();
        
            var score1 = combo.HandScore(straightFlush.Cards.ToArray());
            var score2 = combo.HandScore(fullHouse.Cards.ToArray());
            Assert.True(score1 > score2);
        
            Assert.True(straightFlush.Sum == score1);
            Assert.True(fullHouse.Sum == score2);
        }
    }
    
    [Fact]
    public void IsQuadsWorks()
    {
        var combo = new ComboScoreCalculator();
        
            var fourOfKind = Gen.GetQuads();
            var fullHouse = Gen.GetFullHouse();
        
            var score1 = combo.HandScore(fourOfKind.Cards.ToArray());
            var score2 = combo.HandScore(fullHouse.Cards.ToArray());
        
            Assert.True(score1 > score2);
        
            Assert.True(fourOfKind.Sum == score1);
            Assert.True(fullHouse.Sum == score2);
        
    }
    
    [Fact]
    public void IsFullHouseWorks()
    {
        var combo = new ComboScoreCalculator();

        var fullHouse = Gen.GetFullHouse();
        var threeOfKind = Gen.GetSet();
        
        var score1 = combo.HandScore(fullHouse.Cards.ToArray());
        var score2 = combo.HandScore(threeOfKind.Cards.ToArray());
        Assert.True(score1 > score2);
        
        Assert.True(fullHouse.Sum == score1);
        Assert.True(threeOfKind.Sum == score2);
    }
    
    [Fact]
    public void IsFlushWorks()
    {
        var combo = new ComboScoreCalculator();

        Gen.GetCard(Suit.Clubs, Rank.Six);
        Gen.GetCard(Suit.Diamonds, Rank.Six);
        Gen.GetCard(Suit.Hearts, Rank.Six);
        Gen.GetCard(Suit.Spades, Rank.Six);
        
        Gen.GetCard(Suit.Clubs, Rank.Jack);
        Gen.GetCard(Suit.Diamonds, Rank.Jack);
        Gen.GetCard(Suit.Hearts, Rank.Jack);
        Gen.GetCard(Suit.Spades, Rank.Jack);

        
        // It's possible to get flush and set from 1 deck maximum 4 times --> i<4
        for (int i = 0; i < 4; i++)
        {
            var flush = Gen.GetFlush();
            var threeOfKind = Gen.GetSet();
        
            var score1 = combo.HandScore(flush.Cards.ToArray());
            var score2 = combo.HandScore(threeOfKind.Cards.ToArray());

            Assert.True(score1 > score2);
            Assert.True(flush.Sum == score1);
            Assert.True(threeOfKind.Sum == score2);
        }
    }
    
    [Fact]
    public void IsStraightWorks()
    {
        var combo = new ComboScoreCalculator();
        
        Gen.GetCard(Suit.Clubs, Rank.Six);
        Gen.GetCard(Suit.Diamonds, Rank.Six);
        Gen.GetCard(Suit.Hearts, Rank.Six);
        Gen.GetCard(Suit.Spades, Rank.Six);
        
        Gen.GetCard(Suit.Clubs, Rank.Jack);
        Gen.GetCard(Suit.Diamonds, Rank.Jack);
        Gen.GetCard(Suit.Hearts, Rank.Jack);
        Gen.GetCard(Suit.Spades, Rank.Jack);

        var straight = Gen.GetStraight();
        var threeOfKind = Gen.GetSet();
        
        var score1 = combo.HandScore(straight.Cards.ToArray());
        var score2 = combo.HandScore(threeOfKind.Cards.ToArray());
        
        Assert.True(score1 > score2);
        
        Assert.True(straight.Sum == score1);
        Assert.True(threeOfKind.Sum == score2);
    }
    
    [Fact]
    public void IsSetWorks()
    {
        var combo = new ComboScoreCalculator();


        // It's possible to get two pairs and set from 1 deck maximum 7 times --> i<7
        for (int i = 0; i < 7; i++)
        {
            var threeOfKind = Gen.GetSet();
            var twoPairs = Gen.GetTwoPairs();
        
            var score1 = combo.HandScore(threeOfKind.Cards.ToArray());
            var score2 = combo.HandScore(twoPairs.Cards.ToArray());
        
        
            Assert.True(score1 > score2);
        
            Assert.True(threeOfKind.Sum == score1);
            Assert.True(twoPairs.Sum == score2);
        }
    }
    
    [Fact]
    public void IsTwoPairsWorks()
    {
        var combo = new ComboScoreCalculator();


        // It's possible to get two pairs + one pair from 1 deck maximum 8 times --> i<8
        for (int i = 0; i < 8; i++)
        {
            var twoPairs = Gen.GetTwoPairs();
            var pair = Gen.GetPair();

            var score1 = combo.HandScore(twoPairs.Cards.ToArray());
            var score2 = combo.HandScore(pair.Cards.ToArray());

            Assert.True(score1 > score2);

            Assert.True(twoPairs.Sum == score1);
            Assert.True(pair.Sum == score2);
        }
    }
}