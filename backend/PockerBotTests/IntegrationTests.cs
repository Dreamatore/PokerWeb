using PockerBot.Domain.Card;
using PockerBot.Domain.Core;
using Xunit;

namespace PockerBotTests;

public class IntegrationTests
{
    [Fact]
    public void BasicWin()
    {
        var player1 = new Player("Петя", 1000, 1)
        {
            Hand =
            [
                new Card(Suit.Hearts, Rank.Three),
                new Card(Suit.Hearts, Rank.Four)
            ]
        };
        var player2 = new Player("Вася", 1000, 2)
        {
            Hand =
            [
                new Card(Suit.Spades, Rank.Three),
                new Card(Suit.Diamonds, Rank.Three)
            ]
        };
        List<Card> table =
        [
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Spades, Rank.King),
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Clubs, Rank.Two),
            new Card(Suit.Hearts, Rank.Ten)
        ];
        var gameCore = new TestGameCore([player1, player2]);
        gameCore.SetTable(table);
        gameCore.GameBank = 20;
        gameCore.EndRound();
        // 1020 - 20 (big blind)
        Assert.True(player2.Bank == 1000);
    }

    [Fact]
    public void BasicSplitTest()
    {
        var player1 = new Player("Петя", 1000, 1)
        {
            Hand =
            [
                new Card(Suit.Hearts, Rank.Three),
                new Card(Suit.Spades, Rank.Three)
            ]
        };
        var player2 = new Player("Вася", 1000, 2)
        {
            Hand =
            [
                new Card(Suit.Diamonds, Rank.Three),
                new Card(Suit.Clubs, Rank.Three)
            ]
        };
        List<Card> table =
        [
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Spades, Rank.King),
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Clubs, Rank.Two),
            new Card(Suit.Hearts, Rank.Ten)
        ];
        var gameCore = new TestGameCore([player1, player2]);
        gameCore.SetTable(table);
        gameCore.GameBank = 20;
        gameCore.EndRound();
        // 1010 - 10 (small blind)
        Assert.True(player1.Bank == 1000);
        // 1010 - 20 (big blind)
        Assert.True(player2.Bank == 990);
    }
    [Fact]
    public void HighCardWinsThenSameCombo()
    {
        var player1 = new Player("Петя", 1000, 1)
        {
            Hand =
            [
                new Card(Suit.Hearts, Rank.King),
                new Card(Suit.Spades, Rank.Queen)
            ]
        };
        var player2 = new Player("Вася", 1000, 2)
        {
            Hand =
            [
                new Card(Suit.Diamonds, Rank.Ace),
                new Card(Suit.Clubs, Rank.Two)
            ]
        };
        List<Card> table =
        [
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Spades, Rank.Five),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Hearts, Rank.Jack)
        ];
        var gameCore = new TestGameCore([player1, player2]);
        gameCore.SetTable(table);
        gameCore.GameBank = 20;
        gameCore.EndRound();
        // 1000 - 10 (small blind)
        Assert.True(player1.Bank == 990);
        // 1020 - 20 (big blind)
        Assert.True(player2.Bank == 1000);
    }
    [Fact]
    public void SecondHighCardWinsThenSameCombo()
    {
        var player1 = new Player("Петя", 1000, 1)
        {
            Hand =
            [
                new Card(Suit.Hearts, Rank.Ace),
                new Card(Suit.Spades, Rank.Queen)
            ]
        };
        var player2 = new Player("Вася", 1000, 2)
        {
            Hand =
            [
                new Card(Suit.Diamonds, Rank.Ace),
                new Card(Suit.Clubs, Rank.Two)
            ]
        };
        List<Card> table =
        [
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Spades, Rank.Five),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Hearts, Rank.Jack)
        ];
        var gameCore = new TestGameCore([player1, player2]);
        gameCore.SetTable(table);
        gameCore.GameBank = 20;
        gameCore.EndRound();
        // 1020 - 10 (small blind)
        Assert.True(player1.Bank == 1010);
        // 1000 - 20 (big blind)
        Assert.True(player2.Bank == 980);
    }
}

public class TestGameCore : GameCore
{
    public TestGameCore(List<Player> players) : base(players)
    {
    }

    public void SetTable(List<Card> cards)
    {
        base._table = cards;
    }
}