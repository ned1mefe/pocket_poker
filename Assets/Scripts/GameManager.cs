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
    private Pot _activePot;
    private List<Pot> _pots;
    
    private int sbIndex;
    private int queueIndex;
    public Player ActivePlayer { get; private set; }
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
        _pots = new List<Pot>();
        sbIndex = 0;
        for (short i = 2; i <= 14; i++)
        {
            _deck.Add(new Card(i, Kind.Club));
            _deck.Add(new Card(i, Kind.Spade));
            _deck.Add(new Card(i, Kind.Diamond));
            _deck.Add(new Card(i, Kind.Heart));
        }

        Shuffle();
    }

    public void InitializeGame(GameConfig config, List<String> nameList) // should initialize playerList and start the game loop
    {
        _config = config;

        foreach (var name in nameList)
        {
            var pl = new Player(name, config.BuyIn);
            _players.Add(pl);
        }

        //Test();
    }

    private void StartMainPot()
    {
        _activePot = new Pot(
            _players[sbIndex+1 % _players.Count], 
            _players[sbIndex]
            );
        
        _pots.Add(_activePot);
        ActivePlayer = _players[sbIndex];
        queueIndex = sbIndex;
        
        HandleBlinds();
        
    }
    

    public void Test()
    {
        Debug.LogWarning("Hand:");
        Shuffle();

        var efe = new Player("efe", 250);
        
        foreach (var card in _deck.GetRange(0,7))
        {
            Debug.Log(card);
            //efe.AddCard(card);
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

    private void HandleCheck()
    {
        ActivePlayer.TurnStatus = TurnStatus.Acted;
        do
        {
            queueIndex = ++queueIndex % _players.Count;
            ActivePlayer = _players[queueIndex];
        } while (ActivePlayer.TurnStatus == TurnStatus.Folded);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            // start new phase
        }
    }
    private void HandleRaise(int bet)
    {
        _activePot.HandlePlayerBet(ActivePlayer, bet);
        
        foreach (var player in _players.Where(player => player.TurnStatus == TurnStatus.Acted))
        {
            player.TurnStatus = TurnStatus.Waiting;
        }
        
        ActivePlayer.TurnStatus = TurnStatus.Acted;
        
        do
        {
            queueIndex = ++queueIndex % _players.Count;
            ActivePlayer = _players[queueIndex];
        } while (ActivePlayer.TurnStatus == TurnStatus.Folded);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            // start new phase
        }
        
    }
    private void HandleFold()
    {
        _activePot.HandlePlayerFold(ActivePlayer);
        
        ActivePlayer.TurnStatus = TurnStatus.Folded;
        
        do
        {
            queueIndex = ++queueIndex % _players.Count;
            ActivePlayer = _players[queueIndex];
        } while (ActivePlayer.TurnStatus == TurnStatus.Folded);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            // start new phase
        }
        
    }

    private void HandleBlinds()
    {
        _activePot.HandlePlayerBet(ActivePlayer, _config.BigBlind/2);
        
        queueIndex = ++queueIndex % _players.Count;
        ActivePlayer = _players[queueIndex];
        
        _activePot.HandlePlayerBet(ActivePlayer, _config.BigBlind);
        
        queueIndex = ++queueIndex % _players.Count;
        ActivePlayer = _players[queueIndex];
    }
    
    
}
