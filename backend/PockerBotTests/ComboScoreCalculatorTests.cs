using PockerBot;
using PockerBot.Domain.Card;
using PockerBot.Domain.Combinations;
using Xunit;

namespace PockerBotTests;

public class ComboScoreCalculatorTests
{
    [Fact]
    public void IsRoyalFlush_Greater_Than_Other()
    {
        var boardRoyalFlush = new[]
        {
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Diamonds, Rank.King),
            new Card(Suit.Diamonds, Rank.Ace)
        };
        var boardNotRoyalFlush = new[]
        {
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Clubs, Rank.Four),
            new Card(Suit.Spades, Rank.Four),
            new Card(Suit.Hearts, Rank.Five),
            new Card(Suit.Spades, Rank.Five)
        };
        var comdo = new ComboScoreCalculator();
        var score1 = comdo.HandScore(boardRoyalFlush);
        var score2 = comdo.HandScore(boardNotRoyalFlush);
        Assert.True(score1 > score2);
    }
    [Fact]
    public void IsFullHouse_Greater_Than_Other()
    {
        var board1Weakest = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Clubs, Rank.Two),
            new Card(Suit.Spades, Rank.Two),
            new Card(Suit.Hearts, Rank.Three),
            new Card(Suit.Spades, Rank.Three)
        };
        var board2Normal = new[]
        {
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Clubs, Rank.Four),
            new Card(Suit.Spades, Rank.Four),
            new Card(Suit.Hearts, Rank.Five),
            new Card(Suit.Spades, Rank.Five)
        }; 
        var board3Equal = new[]
        {
            new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Clubs, Rank.Seven),
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Hearts, Rank.Eight),
            new Card(Suit.Spades, Rank.Eight)
        };
        var board33Equal = new[]
        {
            new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Clubs, Rank.Seven),
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Hearts, Rank.Eight),
            new Card(Suit.Spades, Rank.Eight)
        };
        var board4NotFHouse = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Spades, Rank.Three)
        }; 
        var board5Strongest = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Clubs, Rank.Ace),
            new Card(Suit.Spades, Rank.Ace),
            new Card(Suit.Hearts, Rank.King),
            new Card(Suit.Spades, Rank.King)
        };
        var combo = new ComboScoreCalculator();
        var score1 = combo.HandScore(board1Weakest);
        var score2 = combo.HandScore(board2Normal);
        var score3 = combo.HandScore(board3Equal);
        var score4 = combo.HandScore(board4NotFHouse);
        var score5 = combo.HandScore(board5Strongest);
        var score6 = combo.HandScore((board33Equal));
        Assert.True( score3 == score6);
        Assert.True(score1 < score2);
        Assert.True(score5 > score4);
        Assert.All(new[] { score1, score2, score3, score6, score5 }, x => Assert.True(x > score4));
        Assert.All(new[] { score1, score2, score3, score6,score4 }, x => Assert.True(x < score5));
    }
    [Fact]
    public void IsTwoPair_Greater_Than_Other()
    {
        var board1 = new[]
        {
            new Card(Suit.Clubs, Rank.Two),
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Hearts, Rank.Six),
            new Card(Suit.Spades, Rank.Six),
        };
        var board2 = new[]
        {
            new Card(Suit.Clubs, Rank.Four),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Hearts, Rank.Seven),
            new Card(Suit.Spades, Rank.Seven)

        };
        var board3 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Hearts, Rank.Two),
            new Card(Suit.Clubs, Rank.Six),
            new Card(Suit.Spades, Rank.Six),
        };
        var board4 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Hearts, Rank.Three),
            new Card(Suit.Clubs, Rank.Six),
            new Card(Suit.Spades, Rank.Six),
        };
        var board5 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Hearts, Rank.Two),
            new Card(Suit.Clubs, Rank.Six),
            new Card(Suit.Spades, Rank.Six),
        };
        var board6 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Clubs, Rank.King),
            new Card(Suit.Spades, Rank.King),
        };
        var combo = new ComboScoreCalculator();

        var score1 = combo.HandScore(board1);
        var score2 = combo.HandScore(board2);
        var score3 = combo.HandScore(board3);
        var score4 = combo.HandScore(board4);
        var score5 = combo.HandScore(board5);
        var score6 = combo.HandScore(board6);

        Assert.True(score1 < score2);
        Assert.True(score2 > score3);
        Assert.True(score3 == score1);
        Assert.All(new[] { score1, score2, score3, score5, }, x => Assert.True(x > score4));
        Assert.All(new[] { score1, score2, score3, score4, score5 }, x => Assert.True(x < score6));
    }

    [Fact]
    public void IsOnePair_Greater_Than_Other()
    {
        var hand1 = new[]
        {
            new Card(Suit.Hearts, Rank.Two),
            new Card(Suit.Diamonds, Rank.Two)
        };

        var hand2 = new[]
        {
            new Card(Suit.Spades, Rank.Three),
            new Card(Suit.Spades, Rank.Three)
        };

        var hand22 = new[]
        {
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Three)
        };

        var hand3 = new[]
        {
            new Card(Suit.Spades, Rank.Four),
            new Card(Suit.Diamonds, Rank.Four)
        };

        var hand4 = new[]
        {
            new Card(Suit.Diamonds, Rank.Nine),
            new Card(Suit.Spades, Rank.Eight)
        };

        var combo = new ComboScoreCalculator();

        var score1 = combo.HandScore(hand1);
        var score2 = combo.HandScore(hand2);
        var score22 = combo.HandScore(hand22);
        var score3 = combo.HandScore(hand3);
        var score4 = combo.HandScore(hand4);

        Assert.True(score1 < score2);
        Assert.True(score2 < score3);
        Assert.True(score2 == score22);
        Assert.All(new[] { score1, score2 }, x => Assert.True(x < score3));
        Assert.All(new[] { score1, score2, score3 }, x => Assert.True(x > score4));
    }

    [Fact]
    public void IsSet_Greater_Than_Other()
    {
        var board1 = new[]
        {
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Clubs, Rank.Ace)
        };
        var board2 = new[]
        {
            new Card(Suit.Hearts, Rank.King),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Clubs, Rank.Jack)
        };
        var board3 = new[]
        {
            new Card(Suit.Hearts, Rank.Two),
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Clubs, Rank.Two)
        };
        var board4 = new[]
        {
            new Card(Suit.Hearts, Rank.Three),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Clubs, Rank.Three)
        };
        var board5 = new[]
        {
            new Card(Suit.Spades, Rank.Three),
            new Card(Suit.Hearts, Rank.Three),
            new Card(Suit.Clubs, Rank.Three)
        };

        var combination = new ComboScoreCalculator();

        var score1 = combination.HandScore(board1);
        var score2 = combination.HandScore(board2);
        var score3 = combination.HandScore(board3);
        var score4 = combination.HandScore(board4);
        var score5 = combination.HandScore(board5);

        Assert.True(score3 > score2);
        Assert.True(score1 > score3);
        Assert.True(score3 < score4);
        Assert.True(score4 == score5);
        Assert.All(new[] { score1, score3, score4, score5, }, x => Assert.True(x > score2));
        Assert.All(new[] { score2, score3, score4, score5 }, x => Assert.True(x < score1));


    }

    [Fact]
    public void IsStreet_Greater_Then_Other()
    {
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace), //Ace = 1
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Spades, Rank.Three),
            new Card(Suit.Hearts, Rank.Four),
            new Card(Suit.Spades, Rank.Five)
        };
        var board2 = new[]
        {
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Clubs, Rank.Five),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Hearts, Rank.Eight)
        };
        var board3 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Spades, Rank.King),
            new Card(Suit.Hearts, Rank.Ace)
        };
        var board22 = new[]
        {
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Clubs, Rank.Five),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Spades, Rank.Seven),
            new Card(Suit.Hearts, Rank.Eight)
        };
        var board4 = new[]
        {
            new Card(Suit.Diamonds, Rank.King),
            new Card(Suit.Clubs, Rank.Eight),
            new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Spades, Rank.Ace),
            new Card(Suit.Hearts, Rank.Six)
        };
         var comboz = new ComboScoreCalculator();

         var score1 = comboz.HandScore(board1);
         var score2 = comboz.HandScore(board2);
         var score3 = comboz.HandScore(board3);
         var score22 = comboz.HandScore(board22);
         var score4 = comboz.HandScore(board4);

         Assert.True(score1 < score2);
         Assert.True(score2 < score3);
         Assert.True(score22 == score2);
         Assert.True(score4 < score1);
         Assert.All(new [] {score1, score2, score22 }, x => Assert.True(x < score3));
         Assert.All(new []{score3, score2, score22 }, x => Assert.True(x > score1));
         Assert.All(new []{score3, score2, score22,score1 }, x => Assert.True(x > score4));
  
    }
    
    [Fact]
    public void IsFlush_Greater_Then_Other()
    {
        
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace), 
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four)
        };
        var board2 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Diamonds, Rank.King),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Diamonds, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Nine),
            new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Diamonds, Rank.Two)
        };
        var board3 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Spades, Rank.King),
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Diamonds, Rank.Seven)
        };
        var board4 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Hearts, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Spades, Rank.King),
            new Card(Suit.Clubs, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Diamonds, Rank.Seven)
        };
        var board5 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Hearts, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Spades, Rank.King),
            new Card(Suit.Clubs, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Diamonds, Rank.Seven)
        };
        var board6 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Hearts, Rank.Ace)
        };
        
         var comboz = new ComboScoreCalculator();

         var score1NotFlush = comboz.HandScore(board1);
         var score2MaxFlush = comboz.HandScore(board2);
         var score3 = comboz.HandScore(board3);
         var score4 = comboz.HandScore(board4);
         var score5 = comboz.HandScore(board5);
         var score6MinFlush = comboz.HandScore(board6);

         
         Assert.All(new []{score3, score2MaxFlush }, x => Assert.True(x > score6MinFlush));
         Assert.All(new []{score3, score5, score4 ,score1NotFlush }, x => Assert.True(x < score2MaxFlush));
         Assert.All(new []{score3, score2MaxFlush }, x => Assert.True(x > score5));
        
    }

    [Fact]
    public void IsStraightFlush_Greater_Then_Other()
    {
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace), 
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four)
        };
        var board2 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Diamonds, Rank.Nine),
            new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Diamonds, Rank.Ten)
        };
        var board3 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Diamonds, Rank.Seven)
        };
        var board4 = new[]
        {
            new Card(Suit.Hearts, Rank.Ten),
            new Card(Suit.Hearts, Rank.Jack),
            new Card(Suit.Hearts, Rank.Queen),
            new Card(Suit.Hearts, Rank.Ace),
            new Card(Suit.Clubs, Rank.Ace),
            new Card(Suit.Hearts, Rank.Nine),
            new Card(Suit.Hearts, Rank.Eight)
        };
        var board5 = new[]
        {
            new Card(Suit.Clubs, Rank.Four),
            new Card(Suit.Clubs, Rank.Five),
            new Card(Suit.Clubs, Rank.Six),
            new Card(Suit.Clubs, Rank.Seven),
            new Card(Suit.Diamonds, Rank.Six),
            new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Clubs, Rank.Three)
        };
        var board6 = new[]
        {
            new Card(Suit.Diamonds, Rank.King),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Diamonds, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Nine),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Hearts, Rank.Ace)
        };
        
         var comboz = new ComboScoreCalculator();

         var score1NotStraightFlush = comboz.HandScore(board1);
         var score2MinStraigthFlush = comboz.HandScore(board2);
         var score3 = comboz.HandScore(board3);
         var score4 = comboz.HandScore(board4);
         var score5 = comboz.HandScore(board5);
         var score6MaxFlush = comboz.HandScore(board6);

         
         Assert.All(new []{score3, score4 }, x => Assert.True(x > score2MinStraigthFlush));
         Assert.All(new []{score3, score5, score4 ,score1NotStraightFlush }, x => Assert.True(x < score6MaxFlush));
    }

    [Fact]
    public void IsQuads_Greater_Then_Other()
    {
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two), 
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Two)
        };
        var board2 = new[]
        {
            new Card(Suit.Hearts, Rank.Ace), 
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Spades, Rank.Ace),
            new Card(Suit.Clubs, Rank.Ace)
        };
        var board3 = new[]
        {
            new Card(Suit.Diamonds, Rank.Three), 
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Three)
        };
        var board4 = new[]
        {
            new Card(Suit.Hearts, Rank.Six), 
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Spades, Rank.Eight),
            new Card(Suit.Clubs, Rank.King)
        };
        var comboz = new ComboScoreCalculator();
        
        var score1Min = comboz.HandScore(board1);
        var score2Max = comboz.HandScore(board2);
        var score3 = comboz.HandScore(board3);
        var score4 = comboz.HandScore(board4);

         
        Assert.All(new []{score3, score1Min, score4 }, x => Assert.True(x < score2Max ));
        Assert.All(new []{score2Max, score3 , score1Min}, x => Assert.True(x > score4 ));
        Assert.All(new []{score2Max, score3 }, x => Assert.True(x > score1Min ));
     
        
    }

    [Fact]
    public void IsOnePair_Working()
    {
        //2+2+3+3+4+4+9 = 27
        //2+2+3+4+4+5+9 = 29
        var firstCard = new Card(Suit.Spades, Rank.Two);
        var secondCard = new Card(Suit.Spades, Rank.Two);


        Card[] cards = [firstCard, secondCard];

        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(cards);

        Assert.True(res == 34);
    }

    [Fact]
    public void IsTwoPair_Working()
    {
        var firstCard = new Card(Suit.Diamonds, Rank.Two);
        var secondCard = new Card(Suit.Clubs, Rank.Two);
        var thirdCard = new Card(Suit.Hearts, Rank.Three);
        var fourthCard = new Card(Suit.Spades, Rank.Three);

        Card[] cards = [firstCard, secondCard, thirdCard, fourthCard];

        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(cards);
        
        Assert.True(res == 80);
    }
    [Fact]
    public void Two_Pairs_Check_With_Three_pairs()
    {
        var firstCard = new Card(Suit.Diamonds, Rank.Five);
        var secondCard = new Card(Suit.Clubs, Rank.Five);
        var thirdCard = new Card(Suit.Hearts, Rank.Three);
        var fourthCard = new Card(Suit.Spades, Rank.Three);
        var fifthCard = new Card(Suit.Hearts, Rank.Jack);
        var sixthCard = new Card(Suit.Spades, Rank.Jack);

        Card[] cards = [firstCard, secondCard, thirdCard, fourthCard, fifthCard, sixthCard];

        var combo = new ComboScoreCalculator();

        var res = combo.CalcScore(cards);

        var expectedResult = new ComboResult()
        {
            Cards = [firstCard, secondCard, fifthCard, sixthCard],
            Combo = Combo.TwoPairs,
            Score = 102
        };
        
        Assert.True(res == expectedResult);
    }
    [Fact]
    public void IsFullHouse_Working()
    {
        var firstCard = new Card(Suit.Diamonds, Rank.Two);
        var secondCard = new Card(Suit.Clubs, Rank.Two);
        var thirdCard = new Card(Suit.Hearts, Rank.Two);
        var fourthCard = new Card(Suit.Spades, Rank.Three);
        var fifthCard = new Card(Suit.Diamonds, Rank.Three);
        var sixthCard = new Card(Suit.Clubs, Rank.Three);

        Card[] cards = [firstCard, secondCard, thirdCard, fourthCard, fifthCard, sixthCard];

        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(cards);

        Assert.True(res == 343);
    }

    [Fact]
    public void IsGreatCard_Working()
    {
        var firstCard = new Card(Suit.Spades, Rank.Two);
        var secondCard = new Card(Suit.Spades, Rank.Three);
        var thirdCard = new Card(Suit.Spades, Rank.Four);

        Card[] cards = [firstCard, secondCard, thirdCard];

        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(cards);

        Assert.True(res == 4);
    }

    [Fact]
    public void IsSet_Working()
    {
        var board1 = new[]
        {
            new Card(Suit.Hearts, Rank.Two), // 2+2+2 => 6 + 140 => 146
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Clubs, Rank.Two)
        };


        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(board1);

        Assert.True(res == 146);
    }
    
    [Fact]
    public void IsStreet_Working()
    {
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two), // 15 
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Spades, Rank.Four),
            new Card(Suit.Hearts, Rank.Five),
            new Card(Suit.Spades, Rank.Ace)
        };
        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(board1);

        Assert.True(res == 215);

    }
    
    [Fact]
    public void IsFlush_Working()
    {
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Hearts, Rank.Ace)
        };
        
        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(board1);

        Assert.True(res == 291);
        
    }
    
    [Fact]
    public void IsStraightFlush_Working()
    {
        var board1 = new[]
        {
            new Card(Suit.Diamonds, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Two),
            new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four),
            new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Clubs, Rank.Six),
            new Card(Suit.Hearts, Rank.Ace)
        };
        
        var combo = new ComboScoreCalculator();

        var res = combo.HandScore(board1);

        Assert.True(res == 475);
        
    }
    [Fact]
    public void IsQuads_Working()
    { 
        var board1 = new[]
        {
                new Card(Suit.Clubs, Rank.Two), 
                new Card(Suit.Hearts, Rank.Two),
                new Card(Suit.Spades, Rank.Two),
                new Card(Suit.Diamonds, Rank.Two)
        };
        var combo = new ComboScoreCalculator();
        
        var res = combo.HandScore(board1);
        Assert.True(res == 408);
        
    }

    [Fact]
    public void IsRoyalFlush_Working()
    {
        var boardRoyalFlush = new[]
        {
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Diamonds, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen),
            new Card(Suit.Diamonds, Rank.King),
            new Card(Suit.Diamonds, Rank.Ace)
        };
        
        var combo = new ComboScoreCalculator();
        
        var res = combo.HandScore(boardRoyalFlush);
        Assert.True(res == 580);
    }
    

}