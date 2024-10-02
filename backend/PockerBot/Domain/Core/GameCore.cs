using PockerBot.Domain.Combinations;
using PockerBot.Structures;

namespace PockerBot.Domain.Core;

public class GameCore
{
    private int _smallBlind = 10;
    private int _bigBlind = 20;
    private int _minRaise = 0;
    private int _circleIndex = 0;
    private int _dealerIndex = 0;
    public List<Card.Card> Table => _table;

    public Player Dealer => Players[DealerIndex];

    private int DealerIndex
    {
        get => _dealerIndex % Players.Count;
        set => _dealerIndex = value;
    }

    private Deck _deck = new();
    protected List<Card.Card> _table = [];

    private void UpdateLastActionIndex(Player player)
    {
        var index = Players.IndexOf(player);
        _playersActionOrder.PushBack(index);
    }

    private readonly CircularBuffer<int> _playersActionOrder;

    public List<Player> Players { get; set; }

    public List<Player> ActivePlayers => Players.Where(x => !x.IsFolded && x.Bank > 0).ToList();

    private Player PreviousActivePlayer => Players[_playersActionOrder.Back()];

    public int CurrentMaxBet => Players.Where(x => !x.IsFolded).Max(x => x.CurrentBet);

    public Player CurrentActivePlayer
    {
        get
        {
            var prevIndex = Players.IndexOf(PreviousActivePlayer);
            foreach (var _ in Players)
            {
                var nextIndex = (prevIndex + 1) % Players.Count;
                var player = Players[nextIndex];
                if (player is { IsFolded: false, Bank: > 0 })
                {
                    return player;
                }

                prevIndex = nextIndex;
            }

            //unreachable code
            throw new NotImplementedException("All players are folded or all-in");
        }
    }

    public int GameBank { get; set; }

    public GameCore(List<Player> players)
    {
        Players = players;
        _playersActionOrder = new CircularBuffer<int>(Players.Count);
    }

    public PokerMove[] GetAvailableMoves(Player player)
    {
        var toCall = CurrentMaxBet - player.CurrentBet;
        var b = GetAvailableRaise();
        var moves = new List<PokerMove> { PokerMove.Fold };

        if (player.Bank > 0)
        {
            moves.Add(PokerMove.AllIn);
        }

        if (player.Bank > toCall + b.minRaise)
        {
            moves.Add(PokerMove.Raise);
        }

        if (player.CurrentBet < CurrentMaxBet && player.Bank > toCall)
        {
            moves.Add(PokerMove.Call);
        }

        if (player.CurrentBet == CurrentMaxBet)
        {
            moves.Add(PokerMove.Check);
        }

        return moves.ToArray();
    }

    public void StartRound()
    {
        _table = [];
        if (IsGameEnded()) return;
        GameBank = 0;
        RenewDeck();
        foreach (var player in Players)
        {
            player.Hand = new List<Card.Card>();

            var receivedCard1 = _deck.GetFirstFromDeck();
            player.Hand.Add(receivedCard1);

            var receivedCard2 = _deck.GetFirstFromDeck();
            player.Hand.Add(receivedCard2);
        }

        var player1 = Players[(DealerIndex + 1) % Players.Count];
        TakeSmallBlind(player1);

        var player2 = Players[(DealerIndex + 2) % Players.Count];
        TakeBigBlind(player2);

        _minRaise = _bigBlind;
        StartNewCircle();
    }

    private void PostAction(Player player)
    {
        player.IsCircleActionTaken = true;

        if (player.Bank == 0 || player.IsFolded) return;
        UpdateLastActionIndex(player);
    }

    public void TakeSmallBlind(Player player)
    {
        player.Bank -= _smallBlind;
        GameBank += _smallBlind;
        player.CurrentBet += _smallBlind;
        PostAction(player);
    }

    public void TakeBigBlind(Player player)
    {
        player.Bank -= _bigBlind;
        GameBank += _bigBlind;
        player.CurrentBet += _bigBlind;
        PostAction(player);
    }

    public void Call(Player player)
    {
        var toCall = CurrentMaxBet - player.CurrentBet;

        if (toCall > player.Bank)
        {
            toCall = player.Bank;
        }

        player.Bank -= toCall;
        GameBank += toCall;
        player.CurrentBet += toCall;
        PostAction(player);
    }

    public void Check(Player player)
    {
        PostAction(player);
    }

    public void Fold(Player player)
    {
        player.IsFolded = true;

        if (ActivePlayers.Count == 1)
        {
            StartNewCircle();
            return;
        }

        PostAction(player);
    }

    public void AllIn(Player player)
    {
        var allInSum = player.Bank;
        player.CurrentBet += allInSum;
        player.Bank = 0;
        GameBank += allInSum;
        PostAction(player);
    }

    /// <summary>
    /// Raise sum should be more than previous player bet
    /// </summary>
    /// <param name="player"></param>
    /// <param name="raiseSum">Additional sum over current max bet</param>
    public void Raise(Player player, int raiseSum)
    {
        var toCall = CurrentMaxBet - player.CurrentBet;
        var raiseOver = toCall + raiseSum;

        if (raiseOver <= player.Bank)
        {
            _minRaise = raiseSum;
        }
        else
        {
            raiseOver = player.Bank;
        }

        player.CurrentBet += raiseOver;
        player.Bank -= raiseOver;
        GameBank += raiseOver;

        PostAction(player);
    }

    public (int minRaise, int step) GetAvailableRaise()
    {
        var step = _bigBlind;
        return (_minRaise, step);
    }

    public bool IsCircleEnded()
    {
        if (ActivePlayers.Count == 0) return true;
        var actionTaken = ActivePlayers.All(x => x.IsCircleActionTaken);
        var maxBet = ActivePlayers.Select(x => x.CurrentBet).Max();
        var betEquals = ActivePlayers.All(x => x.CurrentBet == maxBet);
        return betEquals && actionTaken;
    }

    public void StartNewCircle()
    {
        foreach (var player in Players)
            player.IsCircleActionTaken = false;
        _circleIndex++;

        if (ActivePlayers.Count == 0 || (ActivePlayers.Count == 1 && ActivePlayers[0].CurrentBet == CurrentMaxBet))
        {
            if (_circleIndex == 2)
            {
                for (var i = 0; i < 5; i++)
                    _table.Add(_deck.GetFirstFromDeck());
            }

            if (_circleIndex == 3)
            {
                for (var i = 0; i < 2; i++)
                    _table.Add(_deck.GetFirstFromDeck());
            }

            if (_circleIndex == 4)
            {
                _table.Add(_deck.GetFirstFromDeck());
            }

            EndRound();
            _circleIndex = 1;
            return;
        }


        if (_circleIndex > 4)
        {
            EndRound();
            _circleIndex = 1;
            return;
        }

        if (_circleIndex == 2)
        {
            _table.Add(_deck.GetFirstFromDeck());
            _table.Add(_deck.GetFirstFromDeck());
            _table.Add(_deck.GetFirstFromDeck());
        }

        if (_circleIndex is 3 or 4)
        {
            _table.Add(_deck.GetFirstFromDeck());
        }
    }

    public event Action<string> NotifyPlayerAboutRoundEnds;

    // TODO implement logic with blind raise by time

    public void EndRound()
    {
        #region Give Bank to the Winner

        var score = new ComboScoreCalculator();
        var maxScore = 0;
        var possibleWinners = Players.Where(x => !x.IsFolded).ToArray();
        foreach (var player in possibleWinners)
        {
            var cards = _table.Concat(player.Hand).ToArray();
            var calcScore = score.CalcScore(cards);
            var handScore = calcScore.Combo == Combo.HighCard
                ? score.CalcScore(player.Hand.ToArray()).Score
                : calcScore.Score;
            player.Score = handScore;
            if (maxScore < handScore) maxScore = handScore;
        }

        var winners = possibleWinners.Where(x => x.Score == maxScore).ToArray();
        var winnersCount = winners.Length;
        if (winnersCount == 1)
        {
            var winner = winners[0];
            winner.Bank += GameBank;

            NotifyPlayerAboutRoundEnds?.Invoke(
                $"Победитель раунда={winner.Name},Hand: {string.Join(",", winner.Hand)}, Table: {string.Join(",", _table)},Score: {GetCombinationByHand(winner.Hand.ToArray())}, Выигрыш составил {GameBank}");
        }
        else
        {
            var firstWinner = winners.FirstOrDefault();
            var cards = _table.Concat(firstWinner.Hand).ToArray();
            var winnerCombo = score.CalcScore(cards);
            if (winnerCombo.Combo == Combo.OnePair || winnerCombo.Combo == Combo.TwoPairs ||
                winnerCombo.Combo == Combo.Set || winnerCombo.Combo == Combo.Quads)
            {
                maxScore = 0;
                bool shouldHandleTwoHighCardCase = false;
                foreach (var player in winners)
                {
                    var playerCards = player.Hand.Concat(_table).ToArray();
                    var winnerScore = score.CalcScore(playerCards);
                    var copyHand = new List<Card.Card>(player.Hand);

                    foreach (var card in winnerScore.Cards)
                    {
                        // удалить карту участвующую в комбо
                        copyHand.Remove(card);
                    }

                    if (copyHand.Count == 1)
                    {
                        player.Score += (int)copyHand[0].Rank;
                    }
                    
                    
                    
                    if (copyHand.Count == 2 && winnerCombo.Combo != Combo.Quads)
                    { 
                        shouldHandleTwoHighCardCase = true;
                        player.Score += Math.Max((int)copyHand[0].Rank, (int)copyHand[1].Rank);
                    }

                    if (maxScore < player.Score) maxScore = player.Score;
                }

                var trueWinners = winners.Where(x => x.Score == maxScore).ToArray();
                if (shouldHandleTwoHighCardCase && trueWinners.Length > 1)
                {
                    foreach (var player in trueWinners)
                    {
                        player.Score += Math.Min((int)player.Hand[0].Rank, (int)player.Hand[1].Rank);
                        if (maxScore < player.Score) maxScore = player.Score;
                    }

                    trueWinners = trueWinners.Where(x => x.Score == maxScore).ToArray();
                }

                if (trueWinners.Length == 1)
                {
                    var winner = trueWinners[0];
                    winner.Bank += GameBank;
                    NotifyPlayerAboutRoundEnds?.Invoke(
                        $"Победитель раунда={winner.Name},Hand: {string.Join(",", winner.Hand)}, Table: {string.Join(",", _table)},Score: {GetCombinationByHand(winner.Hand.ToArray())}, Выигрыш составил {GameBank}");
                }
                else
                {
                    var dividedBank = GameBank / trueWinners.Length;
                    foreach (var winner in trueWinners)
                    {
                        winner.Bank += dividedBank;
                        NotifyPlayerAboutRoundEnds?.Invoke(
                            $"Победитель раунда={winner.Name},Hand: {string.Join(",", winner.Hand)}, Table: {string.Join(",", _table)},Score: {GetCombinationByHand(winner.Hand.ToArray())}, Выигрыш составил {dividedBank}");
                    }
                }
            }
            else
            {
                var dividedBank = GameBank / winnersCount;
                foreach (var winner in winners)
                {
                    winner.Bank += dividedBank;
                    NotifyPlayerAboutRoundEnds?.Invoke(
                        $"Победитель раунда={winner.Name},Hand: {string.Join(",", winner.Hand)}, Table: {string.Join(",", _table)},Score: {GetCombinationByHand(winner.Hand.ToArray())}, Выигрыш составил {dividedBank}");
                }
            }
        }

        #endregion

        #region Remove Loser with Bank==0

        var losers = Players.Where(x => x.Bank == 0).ToArray();
        foreach (var loser in losers)
        {
            Players.Remove(loser);
        }

        //Console.WriteLine("Игроков осталось:" + Players.Count);

        #endregion

        _circleIndex = 0;
        DealerIndex++;
        UpdateLastActionIndex(Players[DealerIndex]);
        foreach (var player in Players)
        {
            player.IsFolded = false;
            player.CurrentBet = 0;
            player.IsCircleActionTaken = false;
        }

        if (Players.Count != 1)
        {
            StartRound();
        }
    }

    public string GetCombinationByHand(Card.Card[] hand)
    {
        var comboCalc = new ComboScoreCalculator();
        var result = comboCalc.CalcScore(_table.Concat(hand).ToArray());
        var comboCards = string.Join(" ", result.Cards.ToList());
        switch (result.Combo)
        {
            case Combo.Unknown:
                return $"Нет комбо";
            case Combo.OnePair:
                return $"Одна пара {comboCards} ({result.Score})";
            case Combo.HighCard:
                var card = hand[0].Rank > hand[1].Rank ? hand[0] : hand[1];
                return $"Кикер(Ст) {card} ({(int)card.Rank})";
            case Combo.TwoPairs:
                return $"Две пары {comboCards} ({result.Score})";
            case Combo.Set:
                return $"Сет {comboCards} ({result.Score})";
            case Combo.Straight:
                return $"Стрит {comboCards} ({result.Score})";
            case Combo.Flush:
                return $"Флеш {comboCards} ({result.Score})";
            case Combo.FullHouse:
                return $"Фулл-Хаус {comboCards} ({result.Score})";
            case Combo.Quads:
                return $"Каре {comboCards} ({result.Score})";
            case Combo.StraightFlush:
                return $"Стрит-Флеш {comboCards} ({result.Score})";
            case Combo.RoyalFlush:
                return $"Флеш-Рояль {comboCards} ({result.Score})";
        }

        return "";
    }
    
    public (string name, ComboResult result) GetCombinationWithCardsByHand(Card.Card[] hand)
    {
        var comboCalc = new ComboScoreCalculator();
        var result = comboCalc.CalcScore(_table.Concat(hand).ToArray());
        var comboCards = string.Join(" ", result.Cards.ToList());
        switch (result.Combo)
        {
            case Combo.Unknown:
                return ($"Нет комбо", result);
            case Combo.OnePair:
                return ($"Одна пара {comboCards} ({result.Score})", result);
            case Combo.HighCard:
                var card = hand[0].Rank > hand[1].Rank ? hand[0] : hand[1];
                return ($"Кикер(Ст) {card} ({(int)card.Rank})", result);
            case Combo.TwoPairs:
                return ($"Две пары {comboCards} ({result.Score})", result);
            case Combo.Set:
                return ($"Сет {comboCards} ({result.Score})", result);
            case Combo.Straight:
                return ($"Стрит {comboCards} ({result.Score})", result);
            case Combo.Flush:
                return ($"Флеш {comboCards} ({result.Score})", result);
            case Combo.FullHouse:
                return ($"Фулл-Хаус {comboCards} ({result.Score})", result);
            case Combo.Quads:
                return ($"Каре {comboCards} ({result.Score})", result);
            case Combo.StraightFlush:
                return ($"Стрит-Флеш {comboCards} ({result.Score})", result);
            case Combo.RoyalFlush:
                return ($"Флеш-Рояль {comboCards} ({result.Score})", result);
        }

        return ("", result);
    }

    public bool IsGameEnded()
    {
        return Players.Count == 1;
    }


    public void RenewDeck()
    {
        _deck = new Deck();
        _deck.Shuffle();
    }
}