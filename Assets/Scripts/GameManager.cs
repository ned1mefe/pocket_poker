using System;
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

    public void InitializeGame(GameConfig config, List<String> nameList) // should initialize playerList and start the gameLoop
    {
        _config = config;

        foreach (var name in nameList)
        {
            var pl = new Player(name, config.BuyIn);
            _players.Add(pl);
        }
        
        
        Test();
    }

    public void createPot() // should create a pot starting the game and when a player bets more than another player's money
    {
        
    }
    

    public void Test()
    {
        Debug.LogWarning("Hand:");
        Shuffle();

        var efe = new Player("efe", 250);
        
        foreach (var card in _deck.GetRange(0,7))
        {
            Debug.Log(card);
            efe.AddCard(card);
        }
        
        efe.Evaluate();

        Debug.LogError(efe.Status);
        foreach (var card in efe.BestHand)
        {
            Debug.Log(card);
        }
        
    }
    private void Shuffle()
    {
        _deck = _deck.OrderBy(_ => _random.Next()).ToList();
    }
    
    
    
}
