using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class GameManager
{
    private List<Card> _deck;
    private readonly Random _random;
    private List<Player> _players;
    private GameConfig _config;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    
    public void SetConfig(GameConfig config) => _config = config;

    private GameManager()
    {
        _deck = new List<Card>();
        _random = new Random();
        _players = new List<Player>();

        for (short i = 2; i <= 14; i++)
        {
            _deck.Add(new Card(i, Kind.Club));
            _deck.Add(new Card(i, Kind.Spade));
            _deck.Add(new Card(i, Kind.Diamond));
            _deck.Add(new Card(i, Kind.Heart));
        }

        Shuffle();
    }

    public void Test()
    {
        var flag = 100;
        while (flag-- > 0)
        {
            Debug.LogWarning("Hand:");
            Shuffle();

            var efe = new Player("efe", 250);
            
            //there is problem with straightFlush
            efe.AddCard(new Card(14, Kind.Diamond));
            efe.AddCard(new Card(13, Kind.Club));
            efe.AddCard(new Card(12, Kind.Club));
            efe.AddCard(new Card(11, Kind.Club));
            efe.AddCard(new Card(10, Kind.Club));
            efe.AddCard(new Card(9, Kind.Club));
            efe.AddCard(new Card(8, Kind.Club));
            
            efe.Evaluate();
            if (efe.Status is HandStatus.StraightFlush or HandStatus.RoyalFlush or HandStatus.Straight) flag = 0;

            Debug.LogError(efe.Status);
            foreach (var card in efe.BestHand)
            {
                Debug.Log(card);
            }
            
        }
        
    }
    private void Shuffle()
    {
        _deck = _deck.OrderBy(_ => _random.Next()).ToList();
    }
    
    
    
}
