using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HandStatus
{
    HighCard = 0,
    OnePair = 1,
    TwoPair = 2,
    ThreeOfaKind = 3,
    Straight = 4,
    Flush = 5,
    FullHouse = 6,
    FourOfaKind = 7,
    StraightFlush = 8,
    RoyalFlush = 9,
}

public enum TurnStatus
{
    Waiting = 1,
    Acted = 2,
    Folded = 3,
    AllIn = 4
}

public class Player
{
    public readonly string Name;
    public int Stack { get; private set; }
    public HandStatus Status{ get; private set; }
    public TurnStatus TurnStatus;
    
    // this allCards contains both the hole cards
    // and the community cards, will be 7 cards on the turn
    private List<Card> _allCards;

    // hole cards only
    public List<Card> Hand;
    
    // this is the best hand player can make with 5 cards in his hand and board
     // Aces may appear as 1 in these in the case of wheel straight                          
    public List<Card> BestHand { get; private set; } 
    
    
    public Player(string name, int buyIn)
    {
        Name = name;
        Stack = buyIn;
        _allCards = new List<Card>();
        Hand = new List<Card>();
        BestHand = new List<Card>();
        Status = HandStatus.HighCard;
        TurnStatus = TurnStatus.Waiting;
    }
    public bool IsBusted => Stack == 0;

    public void Bet(int bet)
    {
        if (bet > Stack)
        {
            Debug.Log("Invalid bet");
            return;
        }
        Stack -= bet;
    }
    
    public void WinPot(int pot)
    {
        Stack += pot;
    }

    public void AddHoleCards(Card card1, Card card2)
    {
        Hand.Add(card1);
        Hand.Add(card2);
        _allCards.Add(card1);
        _allCards.Add(card2);
    }

    public void AddFlopCards(Card card1, Card card2, Card card3)
    {
        _allCards.Add(card1);
        _allCards.Add(card2);
        _allCards.Add(card3);
    }

    public void AddCard(Card c) // only for test
    {
        _allCards.Add(c);
    }
    public void AddTurnRiverCard(Card card)
    {
        _allCards.Add(card);
    }

    private void SortHand() => _allCards = _allCards.OrderBy(c => -c.Number).ToList();

    private List<Card> FillKickers(List<Card> currentBestHand, int kickersNeeded)
    {
        var remainedCards = _allCards.Except(currentBestHand).OrderBy(c => -c.Number).ToList();
        var kickers = new List<Card>();
        
        for (int i = 0; i < kickersNeeded; i++)
        {
            if (i >= remainedCards.Count) return kickers;
            
            kickers.Add(remainedCards[i]);
            
        }
        return kickers;
    }
    
    public void Evaluate()
    {
        if(Status == HandStatus.RoyalFlush) return;
        
        SortHand();
        
        CheckRoyalFlush(); // also checks straight flush so it is passed

        if(Status == HandStatus.RoyalFlush) return;
        if(Status == HandStatus.StraightFlush) return;

        CheckQuads();
        if(Status == HandStatus.FourOfaKind) return;
        
        CheckFullHouse();
        if(Status == HandStatus.FullHouse) return;

        CheckFlush();
        if(Status == HandStatus.Flush) return;

        CheckStraight();
        if(Status == HandStatus.Straight) return;

        CheckSet();
        if(Status == HandStatus.ThreeOfaKind) return;

        CheckTwoPair();
        if(Status == HandStatus.TwoPair) return;

        CheckPair();
        if(Status == HandStatus.OnePair) return;

        BestHand = _allCards.GetRange(0, (_allCards.Count >= 5 ? 5 : _allCards.Count));
        
    }
    
    private void CheckPair()
    {
        foreach (var card in _allCards)
        {
            var identicals = _allCards.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count == 2)
            {
                Status = HandStatus.OnePair;
                BestHand = identicals.Concat(FillKickers(identicals, 3)).ToList();
                return;
            }
        }
    }
    private void CheckTwoPair()
    {
        foreach (var card in _allCards)
        {
            var identicals = _allCards.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count != 2) continue;

            var rest = _allCards.Except(identicals).OrderBy(c => -c.Number).ToList();
            foreach (var ca in rest)
            {
                var restIdenticals = rest.Where(c => ca.Number == c.Number).ToList();
                
                if (restIdenticals.Count() == 2)
                {
                    Status = HandStatus.TwoPair;
                    BestHand = identicals.Concat(restIdenticals).ToList();
                    BestHand = BestHand.Concat(FillKickers(BestHand,1)).ToList();
                    return;
                }
            }
        }
    }
    private void CheckSet()
    {
        foreach (var card in _allCards)
        {
            var identicals = _allCards.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count == 3)
            {
                Status = HandStatus.ThreeOfaKind;
                BestHand = identicals.Concat(FillKickers(identicals, 2)).ToList();
                return;
            }
        }
    }
    private void CheckStraight()
    {
        var straight = new List<Card>();
        foreach (var card in _allCards)
        {
            straight.Clear();
            straight.Add(card);
            short n = (card.Number == 14) ? (short)1 : card.Number;
            if (n > 10) continue;

            bool isStraight = true;
            for (int i = 1; i < 5; i++)
            {
                var nextCard = _allCards.Find(c => c.Number == n + i);

                if (nextCard is not null)
                {
                    straight.Add(nextCard);
                    continue;
                }

                isStraight = false;
                break;
            }

            if (isStraight)
            {
                BestHand = straight.OrderBy(c => -c.Number).ToList();
                Status = HandStatus.Straight;
                if(n != 1)return;
            }
        }
    }
    private void CheckFlush()
    {
        var spades = _allCards.Where(c => c.Kind == Kind.Spade).OrderBy(c => -c.Number).ToList();
        var diamonds = _allCards.Where(c => c.Kind == Kind.Diamond).OrderBy(c => -c.Number).ToList();
        var hearts = _allCards.Where(c => c.Kind == Kind.Heart).OrderBy(c => -c.Number).ToList();
        var clubs = _allCards.Where(c => c.Kind == Kind.Club).OrderBy(c => -c.Number).ToList();

        if (spades.Count() >= 5)
        {
            Status = HandStatus.Flush;
            BestHand = spades.GetRange(0, 5);
            return;
        }
        if (diamonds.Count() >= 5)
        {
            Status = HandStatus.Flush;
            BestHand = diamonds.GetRange(0, 5);
            return;
        }
        if (hearts.Count() >= 5)
        {
            Status = HandStatus.Flush;
            BestHand = hearts.GetRange(0, 5);
            return;
        }
        if (clubs.Count() >= 5)
        {
            Status = HandStatus.Flush;
            BestHand = clubs.GetRange(0, 5);
        }
    }
    private void CheckFullHouse()
    {
        foreach (var card in _allCards)
        {
            var identicals = _allCards.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count != 3) continue;

            var rest = _allCards.Except(identicals).OrderBy(c => -c.Number).ToList();
            foreach (var ca in rest)
            {
                var restIdenticals = rest.Where(c => ca.Number == c.Number).ToList();
                
                if (restIdenticals.Count() == 2)
                {
                    Status = HandStatus.FullHouse;
                    BestHand = identicals.Concat(restIdenticals).ToList();
                    return;
                }
            }
        }
    }
    private void CheckQuads()
    {
        foreach (var card in _allCards)
        {
            var identicals = _allCards.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count == 4)
            {
                Status = HandStatus.FourOfaKind;
                BestHand = identicals.Concat(FillKickers(identicals, 1)).ToList();
                return;
            }
        }
    }
    private void CheckStraightFlush()
    {
        foreach (var card in _allCards)
        {
            short n = (card.Number == 14) ? (short)1 : card.Number;
            var k = card.Kind;
            
            if (n > 10) continue;
            
            var shouldExistList = new List<Card>()
            {
                new Card((short)(n + 4), k),
                new Card((short)(n + 3), k),
                new Card((short)(n + 2), k),
                new Card((short)(n + 1), k)
            };

            if (shouldExistList.TrueForAll(c => _allCards.Contains(c)))
            {
                shouldExistList.Add(new Card(n, k));
                BestHand = shouldExistList;
                Status = HandStatus.StraightFlush;
                if (n != 1) return;
            }
        }
    }
    private void CheckRoyalFlush()
    {
        CheckStraightFlush();
        if (Status == HandStatus.StraightFlush && BestHand.Exists(c => c.Number == 14))
        {
            Status = HandStatus.RoyalFlush;
        }
    }
}
