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

public class Player
{
    private readonly string _name;
    public int Money { get; private set; }
    public HandStatus Status{ get; private set; }
    
    private List<Card> _hand; // this hand contains both the players hand
                              // and the cards at table, will be 7 cards on the turn
    public List<Card> BestHand { get; private set; } // this is the best hand player can make with 5 cards in his hand and board
                                                     // Aces may appear as 1 in these
    public Player(string name, int buyIn)
    {
        _name = name;
        Money = buyIn;
        _hand = new List<Card>();
        BestHand = new List<Card>();
        Status = HandStatus.HighCard;
    }
    public bool IsBusted => Money == 0;

    public void Bet(int bet)
    {
        if (bet > Money)
        {
            Debug.Log("Invalid bet");
            return;
        }
        Money -= bet;
    }
    
    public void WinPot(int pot)
    {
        Money += pot;
    }

    public void AddCard(Card card)
    {
        _hand.Add(card);
    }

    void SortHand() => _hand = _hand.OrderBy(c => -c.Number).ToList();

    private List<Card> FillKickers(List<Card> currentBestHand, int kickersNeeded)
    {
        var remainedCards = _hand.Except(currentBestHand).OrderBy(c => -c.Number).ToList();
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

        BestHand = _hand.GetRange(0, (_hand.Count >= 5 ? 5 : _hand.Count));
        
    }
    
    private void CheckPair()
    {
        foreach (var card in _hand)
        {
            var identicals = _hand.Where(c => c.Number == card.Number).ToList();
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
        foreach (var card in _hand)
        {
            var identicals = _hand.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count != 2) continue;

            var rest = _hand.Except(identicals).OrderBy(c => -c.Number).ToList();
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
        foreach (var card in _hand)
        {
            var identicals = _hand.Where(c => c.Number == card.Number).ToList();
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
        foreach (var card in _hand)
        {
            straight.Clear();
            straight.Add(card);
            short n = (card.Number == 14) ? (short)1 : card.Number;
            if (n > 10) continue;

            bool isStraight = true;
            for (int i = 1; i < 5; i++)
            {
                var nextCard = _hand.Find(c => c.Number == n + i);

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
        var spades = _hand.Where(c => c.Kind == Kind.Spade).OrderBy(c => -c.Number).ToList();
        var diamonds = _hand.Where(c => c.Kind == Kind.Diamond).OrderBy(c => -c.Number).ToList();
        var hearts = _hand.Where(c => c.Kind == Kind.Heart).OrderBy(c => -c.Number).ToList();
        var clubs = _hand.Where(c => c.Kind == Kind.Club).OrderBy(c => -c.Number).ToList();

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
        foreach (var card in _hand)
        {
            var identicals = _hand.Where(c => c.Number == card.Number).ToList();
            if (identicals.Count != 3) continue;

            var rest = _hand.Except(identicals).OrderBy(c => -c.Number).ToList();
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
        foreach (var card in _hand)
        {
            var identicals = _hand.Where(c => c.Number == card.Number).ToList();
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
        foreach (var card in _hand)
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

            if (shouldExistList.TrueForAll(c => _hand.Contains(c)))
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
